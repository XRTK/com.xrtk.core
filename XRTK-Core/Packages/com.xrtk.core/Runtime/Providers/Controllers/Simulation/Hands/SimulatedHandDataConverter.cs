// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    public sealed class SimulatedHandDataConverter : BaseHandDataConverter
    {
        public SimulatedHandDataConverter(Handedness handedness,
            IReadOnlyList<HandControllerPoseDefinition> trackedPoses,
            float handPoseAnimationSpeed,
            float jitterAmount,
            float defaultDistance)
            : base(handedness, trackedPoses)
        {
            this.handPoseAnimationSpeed = handPoseAnimationSpeed;
            this.jitterAmount = jitterAmount;
            this.defaultDistance = defaultDistance;
            this.poseDefinitions = trackedPoses;

            // Initialize available simulated hand poses and find the configured default pose.
            SimulatedHandControllerPose.Initialize(trackedPoses);

            // Simulation cannot work without a default pose.
            if (SimulatedHandControllerPose.DefaultHandPose == null)
            {
                Debug.LogError("There is no default simulated hand pose defined!");
                return;
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

        private readonly float handPoseAnimationSpeed;
        private readonly float jitterAmount;
        private readonly float defaultDistance;
        private readonly IReadOnlyList<HandControllerPoseDefinition> poseDefinitions;
        private readonly StopWatch handUpdateStopWatch;
        private readonly StopWatch lastUpdatedStopWatch;

        private float currentPoseBlending = 0.0f;
        private float targetPoseBlending = 0.0f;
        private Vector3 screenPosition;
        private readonly SimulatedHandControllerPose initialPose;
        private SimulatedHandControllerPose previousPose;
        private SimulatedHandControllerPose targetPose;

        /// <inheritdoc />
        protected override bool PlatformProvidesPointerPose => false;

        /// <inheritdoc />
        protected override bool PlatformProvidesIsPinching => false;

        /// <inheritdoc />
        protected override bool PlatformProvidesPinchStrength => false;

        /// <inheritdoc />
        protected override bool PlatformProvidesIsPointing => false;

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
        /// Gets simulated hand data for a <see cref="MixedRealityHandController"/>.
        /// </summary>
        /// <param name="position">The simulated camera space position of the hand controller.</param>
        /// <param name="deltaRotation">The rotation delta applied to the hand since last update.</param>
        /// <returns>Updated simulated hand data.</returns>
        public HandData GetSimulatedHandData(Vector3 position, Vector3 deltaRotation)
        {
            // Reset the cached pointer pose.
            HandData.PointerPose = MixedRealityPose.ZeroIdentity;

            // Read keyboard / mouse input to determine the root pose delta since last frame.
            var rootPoseDelta = new MixedRealityPose(position, Quaternion.Euler(deltaRotation));

            // Calculate pose changes and compute timestamp for hand tracking update.
            var poseAnimationDelta = handPoseAnimationSpeed * Time.deltaTime;
            var timeStamp = handUpdateStopWatch.TimeStamp;

            // Update simulated hand states using collected data.
            var newTargetPose = GetTargetHandPose();
            var isTrackedOld = HandData.IsTracked;

            HandleSimulationInput(rootPoseDelta);

            if (!string.Equals(newTargetPose.Id, Pose.Id) && HandData.IsTracked)
            {
                previousPose = Pose;
                TargetPose = newTargetPose;
            }

            TargetPoseBlending += poseAnimationDelta;

            bool handDataChanged = isTrackedOld != HandData.IsTracked;

            if (HandData.TimeStamp != timeStamp)
            {
                HandData.TimeStamp = timeStamp;
                if (HandData.IsTracked)
                {
                    UpdatePoseFrame();
                    handDataChanged = true;
                }
            }

            if (handDataChanged)
            {
                Finalize(HandData);
            }

            return HandData;
        }

        private void HandleSimulationInput(MixedRealityPose handRootPose)
        {
            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked)
            {
                Vector3 mousePos = Input.mousePosition;
                screenPosition = new Vector3(mousePos.x, mousePos.y, defaultDistance);
            }

            // Apply position delta x / y in screen space, but depth (z) offset in world space
            screenPosition.x = handRootPose.Position.x;
            screenPosition.y = handRootPose.Position.y;

            var camera = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera
                : CameraCache.Main;

            Vector3 newWorldPoint = camera.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += camera.transform.forward * handRootPose.Position.z;
            screenPosition = camera.WorldToScreenPoint(newWorldPoint);

            // The provided hand root pose rotation is just a delta from the
            // previous frame, so we need to determine the final rotation still.
            HandRotateEulerAngles += handRootPose.Rotation.eulerAngles;
            JitterOffset = Random.insideUnitSphere * jitterAmount;

            HandData.IsTracked = true;
        }

        private void UpdatePoseFrame()
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

            // At this point we know the hand's root pose. All joint poses
            // will be relative to this root pose.
            HandData.RootPose = new MixedRealityPose(position, rotation);

            // Compute joint poses relative to root pose.
            ComputeJointPoses(Pose, Handedness, HandData.RootPose);
        }

        /// <summary>
        /// Computes world space poses from camera-space joint data.
        /// </summary>
        private void ComputeJointPoses(SimulatedHandControllerPose pose, Handedness handedness, MixedRealityPose rootPose)
        {
            Quaternion playspaceRotation = MixedRealityToolkit.CameraSystem != null
                ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform.rotation
                : Quaternion.identity;

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

                // Apply playspace transform
                localPosition = playspaceRotation * localPosition;
                localRotation = playspaceRotation * localRotation;

                // Apply root pose rotation transform
                localRotation = rootPose.Rotation * localRotation;

                HandData.Joints[i] = new MixedRealityPose(localPosition, localRotation);
            }
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
            HandData.TimeStamp = 0;
            HandData.IsTracked = false;

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