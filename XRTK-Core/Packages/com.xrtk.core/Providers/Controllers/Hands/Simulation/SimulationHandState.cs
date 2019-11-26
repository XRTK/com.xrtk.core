// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.UnityEditor;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// Internal class to define current hand data and smoothly animate hand data points.
    /// </summary>
    [Serializable]
    public class SimulationHandState
    {
        private readonly SimulationHandControllerDataProviderProfile profile;
        private long lastSimulatedTimeStamp = 0;
        private float poseBlending = 0.0f;
        private SimulatedHandPose pose;

        /// <summary>
        /// Gets the handedness for the hand simulated by this state instance.
        /// </summary>
        public Handedness Handedness { get; } = Handedness.None;

        /// <summary>
        /// The current hand data produced by the current simulation state.
        /// </summary>
        public HandData HandData { get; } = new HandData();

        public Vector3 ScreenPosition;

        // Rotation of the hand
        public Vector3 HandRotateEulerAngles { get; set; } = Vector3.zero;

        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset { get; set; } = Vector3.zero;

        private string gestureName = null;
        /// <summary>
        /// Currently used gesture name.
        /// </summary>
        public string GestureName
        {
            get => gestureName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, gestureName))
                {
                    gestureName = value;
                    gestureBlending = 0.0f;
                }
            }
        }

        private float gestureBlending = 0.0f;
        /// <summary>
        /// Interpolation between current pose and target gesture.
        /// </summary>
        public float GestureBlending
        {
            get => gestureBlending;
            set => gestureBlending = Mathf.Clamp(value, gestureBlending, 1.0f);
        }

        /// <summary>
        /// Creates a new simulation hand state for a hand.
        /// </summary>
        /// <param name="profile">The active hand simulation profile.</param>
        /// <param name="handedness">Handedness of the simulated hand.</param>
        /// <param name="pose">The hand pose to start simulation with.</param>
        public SimulationHandState(SimulationHandControllerDataProviderProfile profile, Handedness handedness, SimulatedHandPose pose)
        {
            Handedness = handedness;
            this.pose = pose;
            this.profile = profile;
        }

        /// <summary>
        /// Resets the simulated hand state.
        /// </summary>
        public void Reset()
        {
            ScreenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;
            GestureName = pose.Id;

            ResetGesture();
        }

        /// <summary>
        /// Set the position in viewport space rather than screen space (pixels).
        /// </summary>
        public void SetViewportPosition(Vector3 point)
        {
            ScreenPosition = CameraCache.Main.ViewportToScreenPoint(point);
        }

        private void SimulateInput(Vector3 mouseDelta, float noiseAmount, Vector3 rotationDeltaEulerAngles)
        {
            // Apply mouse delta x/y in screen space, but depth offset in world space
            ScreenPosition.x += mouseDelta.x;
            ScreenPosition.y += mouseDelta.y;
            Vector3 newWorldPoint = CameraCache.Main.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += CameraCache.Main.transform.forward * mouseDelta.z;
            ScreenPosition = CameraCache.Main.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rotationDeltaEulerAngles;
            JitterOffset = UnityEngine.Random.insideUnitSphere * noiseAmount;
        }

        /// <summary>
        /// Reset the current hand pose.
        /// </summary>
        private void ResetGesture()
        {
            gestureBlending = 1.0f;
            if (SimulatedHandPose.TryGetPoseByName(gestureName, out SimulatedHandPose gesturePose))
            {
                pose.Copy(gesturePose);
            }
        }

        private void UpdateCurrentPoseFrame()
        {
            if (SimulatedHandPose.TryGetPoseByName(gestureName, out SimulatedHandPose targetPose))
            {
                if (gestureBlending > poseBlending)
                {
                    float range = Mathf.Clamp01(1.0f - poseBlending);
                    float lerpFactor = range > 0.0f ? (gestureBlending - poseBlending) / range : 1.0f;
                    pose = SimulatedHandPose.Lerp(pose, targetPose, lerpFactor);
                }
            }

            poseBlending = gestureBlending;
            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = CameraCache.Main.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            pose.ComputeJointPoses(Handedness, rotation, position, HandData.Joints);
        }

        public void Update(bool isSimulating, bool isAlwaysVisible, Vector3 handPositionDelta, Vector3 handRotationDelta)
        {
            SimulateHand(ref lastSimulatedTimeStamp, isSimulating, isAlwaysVisible, handPositionDelta, handRotationDelta);
        }

        public bool UpdateWithTimeStamp(long timeStamp, bool isTracked)
        {
            bool handDataChanged = false;

            if (HandData.IsTracked != isTracked)
            {
                HandData.IsTracked = isTracked;
                handDataChanged = true;
            }

            if (HandData.TimeStamp != timeStamp)
            {
                HandData.TimeStamp = timeStamp;
                if (HandData.IsTracked)
                {
                    UpdateCurrentPoseFrame();
                    handDataChanged = true;
                }
            }

            return handDataChanged;
        }

        private void SimulateHand(ref long lastSimulatedTimeStamp, bool isSimulating, bool isAlwaysVisible, Vector3 handPositionDelta, Vector3 handRotationDelta)
        {
            // We are "tracking" the hand, if it's configured to always be visible or if
            // simulation is active.
            bool isTracked = isAlwaysVisible || isSimulating;

            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked && isTracked)
            {
                Vector3 mousePos = Input.mousePosition;
                ScreenPosition = new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance);
            }

            // If we are simulating the hand currently, read input and update the hand state.
            if (isSimulating)
            {
                SimulateInput(handPositionDelta, profile.HandJitterAmount, handRotationDelta);

                //if (isAlwaysVisible)
                //{
                //    // Toggle gestures on/off
                //    GestureName = ToggleHandPose(GestureName);
                //}
                //else
                //{
                //    // Enable gesture while mouse button is pressed
                //    GestureName = SelectHandPose();
                //}
            }

            // Update tracked state of a hand.
            // If hideTimeout value is null, hands will stay visible after tracking stops.
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            DateTime currentTime = DateTime.UtcNow;
            if (isTracked)
            {
                HandData.IsTracked = true;
                lastSimulatedTimeStamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastSimulatedTimeStamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    HandData.IsTracked = false;
                }
            }
        }
    }
}