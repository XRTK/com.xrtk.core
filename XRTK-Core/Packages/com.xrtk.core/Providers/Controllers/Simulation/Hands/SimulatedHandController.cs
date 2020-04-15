// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Providers.Controllers.Hands;
using XRTK.Services;
using XRTK.Utilities;
using Random = UnityEngine.Random;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public class SimulatedHandController : BaseHandController, IMixedRealitySimulatedController
    {
        public SimulatedHandController() : base() { }

        /// <inheritdoc />
        public SimulatedHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            simulatedHandControllerDataProvider = (ISimulatedHandControllerDataProvider)controllerDataProvider;

            // Initialize available simulated hand poses and find the configured default pose.
            SimulatedHandControllerPose.Initialize(simulatedHandControllerDataProvider.HandPoseDefinitions);

            // Simulation cannot work without a default pose.
            if (SimulatedHandControllerPose.DefaultHandPose == null)
            {
                Debug.LogError($"There is no default simulated hand pose defined. Check the {GetType().Name}!");
            }

            initialPose = SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id);
            pose = new SimulatedHandControllerPose(initialPose);
            lastUpdatedStopWatch = new StopWatch();
            Reset();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new StopWatch();
            handUpdateStopWatch.Reset();

            converter = new SimulatedHandDataConverter(ControllerHandedness, simulatedHandControllerDataProvider.TrackedPoses);
        }

        private readonly SimulatedHandDataConverter converter;
        private readonly ISimulatedHandControllerDataProvider simulatedHandControllerDataProvider;
        private readonly StopWatch handUpdateStopWatch;
        private readonly StopWatch lastUpdatedStopWatch;
        private Vector3? lastMousePosition = null;

        private float currentPoseBlending = 0.0f;
        private float targetPoseBlending = 0.0f;
        private Vector3 screenPosition;
        private readonly SimulatedHandControllerPose initialPose;
        private SimulatedHandControllerPose previousPose;
        private SimulatedHandControllerPose targetPose;

        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <returns>Updated hand rotation angles.</returns>
        private Vector3 DeltaRotation
        {
            get
            {
                UpdateSimulationMappings();

                float rotationDelta = simulatedHandControllerDataProvider.RotationSpeed * Time.deltaTime;
                Vector3 rotationDeltaEulerAngles = Vector3.zero;

                if (Interactions[0].BoolData)
                {
                    rotationDeltaEulerAngles.y = rotationDelta;
                }

                if (Interactions[1].BoolData)
                {
                    rotationDeltaEulerAngles.y = -rotationDelta;
                }

                if (Interactions[2].BoolData)
                {
                    rotationDeltaEulerAngles.x = -rotationDelta;
                }

                if (Interactions[3].BoolData)
                {
                    rotationDeltaEulerAngles.x = rotationDelta;
                }

                if (Interactions[4].BoolData)
                {
                    rotationDeltaEulerAngles.z = -rotationDelta;
                }

                if (Interactions[5].BoolData)
                {
                    rotationDeltaEulerAngles.z = rotationDelta;
                }

                return rotationDeltaEulerAngles;
            }
        }

        /// <summary>
        /// Gets a simulated depth tracking (hands closer / further from tracking device) update, as well
        /// as the hands simulated (x,y) position.
        /// </summary>
        /// <returns>Hand movement delta.</returns>
        private Vector3 DeltaPosition
        {
            get
            {
                UpdateSimulationMappings();

                Vector3 mouseDelta = lastMousePosition.HasValue ? Input.mousePosition - lastMousePosition.Value : Vector3.zero;

                if (Interactions[6].BoolData)
                {
                    mouseDelta.z += Time.deltaTime * simulatedHandControllerDataProvider.DepthMultiplier;
                }

                if (Interactions[7].BoolData)
                {
                    mouseDelta.z -= Time.deltaTime * simulatedHandControllerDataProvider.DepthMultiplier;
                }

                return mouseDelta;
            }
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping("Yaw Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.E),
            new MixedRealityInteractionMapping("Yaw Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Q),
            new MixedRealityInteractionMapping("Pitch Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.F),
            new MixedRealityInteractionMapping("Pitch Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.R),
            new MixedRealityInteractionMapping("Roll Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.X),
            new MixedRealityInteractionMapping("Roll Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Z),
            new MixedRealityInteractionMapping("Move Away (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.PageUp),
            new MixedRealityInteractionMapping("Move Closer (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.PageDown)
        };

        /// <summary>
        /// The Default Left Handed interactions for this controller.
        /// </summary>
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <summary>
        /// The Default Right Handed interactions for this controller.
        /// </summary>
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <summary>
        /// Gets the hands position in screen space.
        /// </summary>
        public Vector3 ScreenPosition => screenPosition;

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

        /// <inheritdoc />
        public override void UpdateController()
        {
            if (TryGetSimulatedHandData(out var handData))
            {
                converter.Finalize(handData);
                UpdateController(handData);
            }
        }

        private void UpdateSimulationMappings()
        {
            for (int i = 0; i < Interactions?.Length; i++)
            {
                var interactionMapping = Interactions[i];

                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ButtonPress:
                        interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                        interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
                        break;
                }
            }
        }

        /// <summary>
        /// Selects a hand pose to simulate, while its input keycode is pressed.
        /// </summary>
        /// <returns>Default pose if no other fitting user input.</returns>
        private SimulatedHandControllerPose GetTargetHandPose()
        {
            for (int i = 0; i < simulatedHandControllerDataProvider.HandPoseDefinitions.Count; i++)
            {
                var result = simulatedHandControllerDataProvider.HandPoseDefinitions[i];

                if (Input.GetKey(result.KeyCode))
                {
                    return SimulatedHandControllerPose.GetPoseByName(result.Id);
                }
            }

            return SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id);
        }

        /// <summary>
        /// Resets the simulated hand controller.
        /// </summary>
        public void Reset()
        {
            screenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;
            HandData.TimeStamp = 0;
            HandData.IsTracked = false;
            lastUpdatedStopWatch.Reset();

            // reset to the initial pose.
            TargetPoseBlending = 1.0f;

            if (SimulatedHandControllerPose.TryGetPoseByName(initialPose.Id, out var result))
            {
                pose = new SimulatedHandControllerPose(result);
                previousPose = pose;
                TargetPose = pose;
            }
        }

        private bool TryGetSimulatedHandData(out HandData handData)
        {
            // Read keyboard / mouse input to determine the root pose delta since last frame.
            var rootPoseDelta = new MixedRealityPose(DeltaPosition, Quaternion.Euler(DeltaRotation));

            // Calculate pose changes and compute timestamp for hand tracking update.
            var poseAnimationDelta = simulatedHandControllerDataProvider.HandPoseAnimationSpeed * Time.deltaTime;
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

            lastMousePosition = Input.mousePosition;

            if (handDataChanged)
            {
                handData = HandData;
                return true;
            }

            handData = null;
            return false;
        }

        private void HandleSimulationInput(MixedRealityPose rootPoseDelta)
        {
            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked)
            {
                Vector3 mousePos = Input.mousePosition;
                screenPosition = new Vector3(mousePos.x, mousePos.y, simulatedHandControllerDataProvider.DefaultDistance);
            }

            // Apply mouse delta x/y in screen space, but depth offset in world space
            screenPosition.x += rootPoseDelta.Position.x;
            screenPosition.y += rootPoseDelta.Position.y;
            Vector3 newWorldPoint = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.transform.forward * rootPoseDelta.Position.z;
            screenPosition = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayerCamera.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rootPoseDelta.Rotation.eulerAngles;
            JitterOffset = Random.insideUnitSphere * simulatedHandControllerDataProvider.JitterAmount;

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
            Pose.ComputeJointPoses(ControllerHandedness, rotation, position, HandData.Joints);
        }
    }
}