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
            IReadOnlyList<SimulatedHandControllerPoseData> trackedPoses,
            float handPoseAnimationSpeed,
            IReadOnlyList<SimulatedHandControllerPoseData> handPoseDefinitions,
            float jitterAmount,
            float defaultDistance)
            : base(handedness, trackedPoses)
        {
            this.handPoseAnimationSpeed = handPoseAnimationSpeed;
            this.handPoseDefinitions = handPoseDefinitions;
            this.jitterAmount = jitterAmount;
            this.defaultDistance = defaultDistance;

            // Initialize available simulated hand poses and find the configured default pose.
            SimulatedHandControllerPose.Initialize(handPoseDefinitions);

            // Simulation cannot work without a default pose.
            if (SimulatedHandControllerPose.DefaultHandPose == null)
            {
                Debug.LogError($"There is no default simulated hand pose defined. Check the {GetType().Name}!");
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
        private readonly IReadOnlyList<SimulatedHandControllerPoseData> handPoseDefinitions;
        private readonly float jitterAmount;
        private readonly float defaultDistance;
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

        public HandData GetSimulatedHandData(Vector3 deltaPosition, Vector3 deltaRotation)
        {
            // Read keyboard / mouse input to determine the root pose delta since last frame.
            var rootPoseDelta = new MixedRealityPose(deltaPosition, Quaternion.Euler(deltaRotation));

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
                PostProcess(HandData);
            }

            return HandData;
        }

        private void HandleSimulationInput(MixedRealityPose rootPoseDelta)
        {
            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked)
            {
                Vector3 mousePos = Input.mousePosition;
                screenPosition = new Vector3(mousePos.x, mousePos.y, defaultDistance);
            }

            // Apply mouse delta x/y in screen space, but depth offset in world space
            screenPosition.x += rootPoseDelta.Position.x;
            screenPosition.y += rootPoseDelta.Position.y;
            Vector3 newWorldPoint = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.forward * rootPoseDelta.Position.z;
            screenPosition = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rootPoseDelta.Rotation.eulerAngles;
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
            var position = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            Pose.ComputeJointPoses(Handedness, rotation, position, HandData.Joints);
        }

        /// <summary>
        /// Selects a hand pose to simulate, while its input keycode is pressed.
        /// </summary>
        /// <returns>Default pose if no other fitting user input.</returns>
        private SimulatedHandControllerPose GetTargetHandPose()
        {
            for (int i = 0; i < handPoseDefinitions.Count; i++)
            {
                var result = handPoseDefinitions[i];

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