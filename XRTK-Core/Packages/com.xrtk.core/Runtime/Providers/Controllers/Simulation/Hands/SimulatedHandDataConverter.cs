// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;
using XRTK.Utilities;
using XRTK.Services;
using Random = UnityEngine.Random;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public sealed class SimulatedHandDataConverter
    {
        public SimulatedHandDataConverter(Handedness handedness,
            IReadOnlyList<HandControllerPoseProfile> trackedPoses,
            float handPoseAnimationSpeed,
            float jitterAmount,
            float defaultDistance)
        {
            this.handPoseAnimationSpeed = handPoseAnimationSpeed;
            this.jitterAmount = jitterAmount;
            this.defaultDistance = defaultDistance;
            this.handedness = handedness;
            poseDefinitions = trackedPoses ?? throw new ArgumentException($"{nameof(trackedPoses)} must be provided");

            // Initialize available simulated hand poses and find the configured default pose.
            SimulatedHandControllerPose.Initialize(trackedPoses);

            if (SimulatedHandControllerPose.DefaultHandPose == null)
            {
                throw new ArgumentException("There is no default simulated hand pose defined!");
            }

            initialPose = SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id);
            pose = new SimulatedHandControllerPose(initialPose);

            // Start the timestamp stopwatches
            lastUpdatedStopWatch = new StopWatch();
            lastUpdatedStopWatch.Reset();
            handUpdateStopWatch = new StopWatch();
            handUpdateStopWatch.Reset();

            ResetConverter();
        }

        private readonly Handedness handedness;
        private readonly float handPoseAnimationSpeed;
        private readonly float jitterAmount;
        private readonly float defaultDistance;
        private readonly IReadOnlyList<HandControllerPoseProfile> poseDefinitions;
        private readonly StopWatch handUpdateStopWatch;
        private readonly StopWatch lastUpdatedStopWatch;

        private float currentPoseBlending = 0.0f;
        private float targetPoseBlending = 0.0f;
        private Vector3 screenPosition;
        private readonly SimulatedHandControllerPose initialPose;
        private SimulatedHandControllerPose previousPose;
        private SimulatedHandControllerPose targetPose;

        /// <summary>
        /// Gets the hands position in screen space.
        /// </summary>
        public Vector3 ScreenPosition => screenPosition;

        private SimulatedHandControllerPose pose;

        /// <summary>
        /// Currently used simulation hand pose.
        /// </summary>
        public SimulatedHandControllerPose Pose => pose;

        /// <summary>
        /// The currently targeted hand pose, reached when <see cref="TargetPoseBlending"/>
        /// reaches 1.
        /// </summary>
        private SimulatedHandControllerPose TargetPose
        {
            get => targetPose;
            set
            {
                if (!string.Equals(value.Id, targetPose.Id))
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
        /// Current rotation of the hand.
        /// </summary>
        public Vector3 HandRotateEulerAngles { get; private set; } = Vector3.zero;

        /// <summary>
        /// Current random offset to simulate tracking inaccuracy.
        /// </summary>
        public Vector3 JitterOffset { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets simulated hand data for a <see cref="MixedRealityHandController"/>.
        /// </summary>
        /// <param name="position">The simulated camera space position of the hand controller.</param>
        /// <param name="deltaRotation">The rotation delta applied to the hand since last update.</param>
        /// <returns>Updated simulated hand data.</returns>
        public HandData GetSimulatedHandData(Vector3 position, Vector3 deltaRotation)
        {
            // Read keyboard / mouse input to determine the root pose delta since last frame.
            var rootPoseDelta = new MixedRealityPose(position, Quaternion.Euler(deltaRotation));

            // Calculate pose changes and compute timestamp for hand tracking update.
            var poseAnimationDelta = handPoseAnimationSpeed * Time.deltaTime;
            var timeStamp = handUpdateStopWatch.TimeStamp;

            // Update simulated hand states using collected data.
            var newTargetPose = GetTargetHandPose();

            HandleSimulationInput(rootPoseDelta);

            if (!string.Equals(newTargetPose.Id, Pose.Id))
            {
                previousPose = Pose;
                TargetPose = newTargetPose;
            }

            TargetPoseBlending += poseAnimationDelta;

            var handData = UpdatePoseFrame();
            handData.UpdatedAt = timeStamp;
            handData.TrackingState = Definitions.Devices.TrackingState.Tracked;

            return handData;
        }

        private void HandleSimulationInput(MixedRealityPose handRootPose)
        {
            var mousePos = Input.mousePosition;
            screenPosition = new Vector3(mousePos.x, mousePos.y, defaultDistance)
            {
                // Apply position delta x / y in screen space, but depth (z) offset in world space
                x = handRootPose.Position.x,
                y = handRootPose.Position.y
            };

            var camera = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera
                : CameraCache.Main;

            var newWorldPoint = camera.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += camera.transform.forward * handRootPose.Position.z;
            screenPosition = camera.WorldToScreenPoint(newWorldPoint);

            // The provided hand root pose rotation is just a delta from the
            // previous frame, so we need to determine the final rotation still.
            HandRotateEulerAngles += handRootPose.Rotation.eulerAngles;
            JitterOffset = Random.insideUnitSphere * jitterAmount;
        }

        private HandData UpdatePoseFrame()
        {
            if (TargetPoseBlending > currentPoseBlending)
            {
                float range = Mathf.Clamp01(1.0f - currentPoseBlending);
                float lerpFactor = range > 0.0f ? (TargetPoseBlending - currentPoseBlending) / range : 1.0f;
                SimulatedHandControllerPose.Lerp(ref pose, previousPose, TargetPose, lerpFactor);
            }

            currentPoseBlending = TargetPoseBlending;
            var rotation = Quaternion.Euler(HandRotateEulerAngles);
            var camera = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera
                : CameraCache.Main;

            var position = camera.ScreenToWorldPoint(ScreenPosition + JitterOffset);

            // At this point we know the hand's root pose in world space and
            // need to translat to playspace.
            var rootPose = new MixedRealityPose(position, rotation);
            var playspaceTransform = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform;
            rootPose.Position = playspaceTransform.InverseTransformPoint(rootPose.Position);
            rootPose.Rotation = Quaternion.Inverse(playspaceTransform.rotation) * playspaceTransform.rotation * rootPose.Rotation;

            // Compute joint poses relative to root pose.
            var jointPoses = ComputeJointPoses(Pose, handedness);

            return new HandData(rootPose, jointPoses);
        }

        /// <summary>
        /// Computes local poses from camera-space joint data.
        /// </summary>
        private MixedRealityPose[] ComputeJointPoses(SimulatedHandControllerPose pose, Handedness handedness)
        {
            var cameraRotation = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.rotation
                : CameraCache.Main.transform.rotation;

            var jointPoses = new MixedRealityPose[HandData.JointCount];
            for (int i = 0; i < HandData.JointCount; i++)
            {
                // Initialize from local offsets
                var localPosition = pose.LocalJointPoses[i].Position;
                var localRotation = pose.LocalJointPoses[i].Rotation;

                // Pose offset are for right hand, mirror on X axis if left hand is needed
                if (handedness == Handedness.Left)
                {
                    localPosition.x = -localPosition.x;
                    localRotation.y = -localRotation.y;
                    localRotation.z = -localRotation.z;
                }

                // Apply camera transform
                localPosition = cameraRotation * localPosition;
                localRotation = cameraRotation * localRotation;

                jointPoses[i] = new MixedRealityPose(localPosition, localRotation);
            }

            return jointPoses;
        }

        /// <summary>
        /// Selects a hand pose to simulate, while its input keycode is pressed.
        /// </summary>
        /// <returns>Default pose if no other fitting user input.</returns>
        private SimulatedHandControllerPose GetTargetHandPose()
        {
            for (int i = 0; i < poseDefinitions.Count; i++)
            {
                var result = poseDefinitions[i];

                if (Input.GetKey(result.KeyCode))
                {
                    return SimulatedHandControllerPose.GetPoseByName(result.Id);
                }
            }

            return SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id);
        }

        public void ResetConverter()
        {
            screenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;

            // reset to the initial pose.
            TargetPoseBlending = 1.0f;

            if (SimulatedHandControllerPose.TryGetPoseByName(initialPose.Id, out var result))
            {
                pose = new SimulatedHandControllerPose(result);
                previousPose = pose;
                TargetPose = pose;
            }
        }
    }
}