// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.UnityEditor;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// Internal class to define current hand data and smoothly animate hand data points.
    /// </summary>
    [Serializable]
    public class SimulationHandState
    {
        private readonly SimulationHandControllerDataProviderProfile profile;
        private readonly Camera playerCamera;
        private readonly SimulationTimeStampStopWatch lastUpdatedStopWatch;
        private long lastSimulatedTimeStamp = 0;
        private float currentPoseBlending = 0.0f;
        private float targetPoseBlending = 0.0f;
        private Vector3 screenPosition;
        private SimulationHandPose initialPose;
        private SimulationHandPose previousPose;
        private SimulationHandPose targetPose;

        /// <summary>
        /// Gets the hands position in screen space.
        /// </summary>
        public Vector3 ScreenPosition => screenPosition;

        /// <summary>
        /// Gets the handedness for the hand simulated by this state instance.
        /// </summary>
        public Handedness Handedness { get; } = Handedness.None;

        /// <summary>
        /// The current hand data produced by the current simulation state.
        /// </summary>
        public HandData HandData { get; } = new HandData();

        /// <summary>
        /// Current rotation of the hand.
        /// </summary>
        public Vector3 HandRotateEulerAngles { get; private set; } = Vector3.zero;

        /// <summary>
        /// Current random offset to simulate tracking inaccuracy.
        /// </summary>
        public Vector3 JitterOffset { get; private set; } = Vector3.zero;

        /// <summary>
        /// Currently used simulation hand pose.
        /// </summary>
        public SimulationHandPose Pose { get; private set; }

        /// <summary>
        /// The currently targeted hand pose, reached when <see cref="TargetPoseBlending"/>
        /// reaches 1.
        /// </summary>
        private SimulationHandPose TargetPose
        {
            get => targetPose;
            set
            {
                if (!string.Equals(value?.Id, targetPose?.Id))
                {
                    targetPose = value;
                    targetPoseBlending = 0.0f;
                }
            }
        }

        /// <summary>
        /// Linear interpolation state between current pose and target pose.
        /// Will get clamped to [current,1] where 1 means the hand has reached the target pose.
        /// </summary>
        public float TargetPoseBlending
        {
            get => targetPoseBlending;
            private set => targetPoseBlending = Mathf.Clamp(value, targetPoseBlending, 1.0f);
        }

        /// <summary>
        /// Creates a new simulation hand state for a hand.
        /// </summary>
        /// <param name="profile">The active hand simulation profile.</param>
        /// <param name="handedness">Handedness of the simulated hand.</param>
        /// <param name="pose">The hand pose to start simulation with.</param>
        public SimulationHandState(SimulationHandControllerDataProviderProfile profile, Handedness handedness, SimulationHandPose pose)
        {
            this.profile = profile;
            Handedness = handedness;
            initialPose = pose;
            Pose = pose;
            lastUpdatedStopWatch = new SimulationTimeStampStopWatch();
            playerCamera = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera;
        }

        /// <summary>
        /// Resets the simulated hand state.
        /// </summary>
        public void Reset()
        {
            screenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;
            Pose = initialPose;
            HandData.TimeStamp = 0;
            HandData.IsTracked = false;
            lastUpdatedStopWatch.Reset();

            ResetPose();
        }

        /// <summary>
        /// Set the position in viewport space rather than screen space (pixels).
        /// </summary>
        public void SetViewportPosition(Vector3 point)
        {
            screenPosition = playerCamera.ViewportToScreenPoint(point);
        }

        /// <summary>
        /// Updates the simulated hand state using data provided.
        /// </summary>
        /// <param name="timeStamp">Timestamp of the hand tracking update.</param>
        /// <param name="simulationInput">Simulation input data.</param>
        /// <param name="poseAnimationDelta">Pose animation changes to apply since last update.</param>
        /// <returns>True, if the simulated hand state has changed.</returns>
        public bool Update(long timeStamp, SimulationInput simulationInput, float poseAnimationDelta)
        {
            bool isTrackedOld = HandData.IsTracked;

            HandleSimulationInput(ref lastSimulatedTimeStamp, simulationInput);

            if (!string.Equals(simulationInput.HandPose.Id, Pose.Id) && HandData.IsTracked)
            {
                previousPose = Pose;
                TargetPose = simulationInput.HandPose;
            }

            TargetPoseBlending += poseAnimationDelta;

            bool handDataChanged = false;
            if (isTrackedOld != HandData.IsTracked)
            {
                handDataChanged = true;
            }

            if (HandData.TimeStamp != timeStamp)
            {
                HandData.TimeStamp = timeStamp;
                if (HandData.IsTracked)
                {
                    UpdatePoseFrame();
                    handDataChanged = true;
                }
            }

            return handDataChanged;
        }

        /// <summary>
        /// Reset the current hand pose.
        /// </summary>
        private void ResetPose()
        {
            TargetPoseBlending = 1.0f;
            if (SimulationHandPose.TryGetPoseByName(Pose.Id, out SimulationHandPose pose))
            {
                Pose.Copy(pose);
                previousPose = Pose;
                TargetPose = Pose;
            }
        }

        private void HandleSimulationInput(ref long lastSimulatedTimeStamp, SimulationInput simulationInput)
        {
            // We are "tracking" the hand, if it's configured to always be visible or if simulation is active.
            bool isTrackedOrAlwaysVisible = simulationInput.IsAlwaysVisible || simulationInput.IsTracking;

            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked && isTrackedOrAlwaysVisible)
            {
                Vector3 mousePos = Input.mousePosition;
                screenPosition = new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance);
            }

            // If we are simulating the hand currently, use input update the hand state.
            if (simulationInput.IsTracking)
            {
                // Apply mouse delta x/y in screen space, but depth offset in world space
                screenPosition.x += simulationInput.HandPositionDelta.x;
                screenPosition.y += simulationInput.HandPositionDelta.y;
                Vector3 newWorldPoint = playerCamera.ScreenToWorldPoint(ScreenPosition);
                newWorldPoint += playerCamera.transform.forward * simulationInput.HandPositionDelta.z;
                screenPosition = playerCamera.WorldToScreenPoint(newWorldPoint);

                HandRotateEulerAngles += simulationInput.HandRotationDelta;
                JitterOffset = UnityEngine.Random.insideUnitSphere * profile.HandJitterAmount;
            }

            DateTime stopWatchCurrent = lastUpdatedStopWatch.Current;
            if (isTrackedOrAlwaysVisible)
            {
                HandData.IsTracked = true;
                lastSimulatedTimeStamp = lastUpdatedStopWatch.TimeStamp;
            }
            else
            {
                float timeSinceTracking = (float)stopWatchCurrent.Subtract(new DateTime(lastSimulatedTimeStamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    HandData.IsTracked = false;
                }
            }
        }

        private void UpdatePoseFrame()
        {
            if (TargetPoseBlending > currentPoseBlending)
            {
                float range = Mathf.Clamp01(1.0f - currentPoseBlending);
                float lerpFactor = range > 0.0f ? (TargetPoseBlending - currentPoseBlending) / range : 1.0f;
                Pose = SimulationHandPose.Lerp(previousPose, TargetPose, lerpFactor);
            }

            currentPoseBlending = TargetPoseBlending;
            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = playerCamera.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            Pose.ComputeJointPoses(Handedness, rotation, position, HandData.Joints);
        }
    }
}