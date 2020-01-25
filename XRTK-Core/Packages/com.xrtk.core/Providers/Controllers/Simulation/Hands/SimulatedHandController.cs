// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Simulation;
using XRTK.Providers.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// Hand controller type for simulated hand controllers.
    /// </summary>
    public class SimulatedHandController : BaseHandController, IMixedRealitySimulatedController
    {
        /// <summary>
        /// Controller constructor.
        /// </summary>
        /// <param name="trackingState">The controller's tracking state.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="inputSource">Optional input source of the controller.</param>
        /// <param name="interactions">Optional controller interactions mappings.</param>
        public SimulatedHandController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            dataProvider = GetProfile();
            if (dataProvider == null)
            {
                Debug.LogError($"Could not get active {nameof(SimulatedControllerDataProvider)}.");
            }

            // Initialize available simulated hand poses and find the
            // cofigured default pose.
            SimulatedHandControllerPose.Initialize(dataProvider.HandPoseDefinitions);

            // Simulation cannot work without a default pose.
            if (SimulatedHandControllerPose.DefaultHandPose == null)
            {
                Debug.LogError($"There is no default simulated hand pose defined. Check the {GetType().Name}!");
            }

            initialPose = SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id);
            Pose = initialPose;
            lastUpdatedStopWatch = new SimulationTimeStampStopWatch();
            Reset();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
        }

        private readonly SimulationTimeStampStopWatch handUpdateStopWatch;
        private readonly SimulatedControllerDataProvider dataProvider;
        private readonly SimulationTimeStampStopWatch lastUpdatedStopWatch;
        private Vector3? lastMousePosition = null;
        private long lastSimulatedTimeStamp = 0;
        private float currentPoseBlending = 0.0f;
        private float targetPoseBlending = 0.0f;
        private Vector3 screenPosition;
        private SimulatedHandControllerPose initialPose;
        private SimulatedHandControllerPose previousPose;
        private SimulatedHandControllerPose targetPose;

        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Yaw Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.E),
            new MixedRealityInteractionMapping(1, "Yaw Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Q),
            new MixedRealityInteractionMapping(2, "Pitch Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.F),
            new MixedRealityInteractionMapping(3, "Pitch Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.R),
            new MixedRealityInteractionMapping(4, "Roll Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.X),
            new MixedRealityInteractionMapping(5, "Roll Counter Clockwise", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Z),
            new MixedRealityInteractionMapping(6, "Move Away (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.PageUp),
            new MixedRealityInteractionMapping(7, "Move Closer (Depth)", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.PageDown),
            new MixedRealityInteractionMapping(8, "Move", AxisType.ThreeDofPosition, DeviceInputType.PointerPosition, MixedRealityInputAction.None)
        };

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

        /// <summary>
        /// Currently used simulation hand pose.
        /// </summary>
        public SimulatedHandControllerPose Pose { get; private set; }

        /// <summary>
        /// The currently targeted hand pose, reached when <see cref="TargetPoseBlending"/>
        /// reaches 1.
        /// </summary>
        private SimulatedHandControllerPose TargetPose
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

        private SimulatedControllerDataProvider GetProfile()
        {
            List<IMixedRealityService> controllerDataProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityControllerDataProvider>();
            for (int i = 0; i < controllerDataProviders.Count; i++)
            {
                if (controllerDataProviders[i] is SimulatedControllerDataProvider simulationHandControllerDataProvider)
                {
                    return simulationHandControllerDataProvider;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public override void UpdateController()
        {
            base.UpdateController();

            if (TryGetSimulatedHandData(out HandData handData))
            {
                UpdateController(handData);
            }
        }

        /// <inheritdoc />
        protected override void UpdateInteraction(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ButtonPress:
                    interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                    break;
                case DeviceInputType.PointerPosition:
                    interactionMapping.PositionData = Input.mousePosition;
                    break;
            }
        }

        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <returns>Updated hand rotation angles.</returns>
        private Vector3 GetHandRotationDelta()
        {
            float rotationDelta = dataProvider.RotationSpeed * Time.deltaTime;
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

        /// <summary>
        /// Gets a simulated depth tracking (hands closer / further from tracking device) update, as well
        /// as the hands simulated (x,y) position.
        /// </summary>
        /// <returns>Hand movement delta.</returns>
        private Vector3 GetHandPositionDelta()
        {
            Vector3 mouseDelta = lastMousePosition.HasValue ? Interactions[8].PositionData - lastMousePosition.Value : Vector3.zero;

            if (Interactions[6].BoolData)
            {
                mouseDelta.z += Time.deltaTime * dataProvider.HandDepthMultiplier;
            }
            if (Interactions[7].BoolData)
            {
                mouseDelta.z -= Time.deltaTime * dataProvider.HandDepthMultiplier;
            }

            return mouseDelta;
        }

        /// <summary>
        /// Selects a hand pose to simulate, while its input keycode is pressed.
        /// </summary>
        /// <returns>Default pose if no other fitting user input.</returns>
        private SimulatedHandControllerPose GetTargetHandPose()
        {
            for (int i = 0; i < dataProvider.HandPoseDefinitions.Count; i++)
            {
                SimulatedHandControllerPoseData pose = dataProvider.HandPoseDefinitions[i];
                if (Input.GetKey(pose.KeyCode))
                {
                    return SimulatedHandControllerPose.GetPoseByName(pose.Id);
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
            Pose = initialPose;
            HandData.TimeStamp = 0;
            HandData.IsTracked = false;
            lastUpdatedStopWatch.Reset();

            ResetPose();
        }

        /// <summary>
        /// Reset the current hand pose.
        /// </summary>
        private void ResetPose()
        {
            TargetPoseBlending = 1.0f;
            if (SimulatedHandControllerPose.TryGetPoseByName(Pose.Id, out SimulatedHandControllerPose pose))
            {
                Pose.Copy(pose);
                previousPose = Pose;
                TargetPose = Pose;
            }
        }

        private bool TryGetSimulatedHandData(out HandData handData)
        {
            // Read keyboard / mouse input to determine the root pose delta since last frame.
            MixedRealityPose rootPoseDelta = new MixedRealityPose(
                GetHandPositionDelta(), Quaternion.Euler(GetHandRotationDelta()));

            // Calculate pose changes and compute timestamp for hand tracking update.
            float poseAnimationDelta = dataProvider.HandPoseAnimationSpeed * Time.deltaTime;
            long timeStamp = handUpdateStopWatch.TimeStamp;

            // Update simualted hand states using collected data.
            SimulatedHandControllerPose newTargetPose = GetTargetHandPose();
            bool isTrackedOld = HandData.IsTracked;

            HandleSimulationInput(ref lastSimulatedTimeStamp, rootPoseDelta);

            if (!string.Equals(newTargetPose.Id, Pose.Id) && HandData.IsTracked)
            {
                previousPose = Pose;
                TargetPose = newTargetPose;
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

            lastMousePosition = Input.mousePosition;

            if (handDataChanged)
            {
                handData = HandData;
                return true;
            }

            handData = null;
            return false;
        }

        private void HandleSimulationInput(ref long lastSimulatedTimeStamp, MixedRealityPose rootPoseDelta)
        {
            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!HandData.IsTracked)
            {
                Vector3 mousePos = Input.mousePosition;
                screenPosition = new Vector3(mousePos.x, mousePos.y, dataProvider.DefaultDistance);
            }

            // Apply mouse delta x/y in screen space, but depth offset in world space
            screenPosition.x += rootPoseDelta.Position.x;
            screenPosition.y += rootPoseDelta.Position.y;
            Vector3 newWorldPoint = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.forward * rootPoseDelta.Position.z;
            screenPosition = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rootPoseDelta.Rotation.eulerAngles;
            JitterOffset = Random.insideUnitSphere * dataProvider.JitterAmount;

            HandData.IsTracked = true;
            lastSimulatedTimeStamp = lastUpdatedStopWatch.TimeStamp;
        }

        private void UpdatePoseFrame()
        {
            if (TargetPoseBlending > currentPoseBlending)
            {
                float range = Mathf.Clamp01(1.0f - currentPoseBlending);
                float lerpFactor = range > 0.0f ? (TargetPoseBlending - currentPoseBlending) / range : 1.0f;
                Pose = SimulatedHandControllerPose.Lerp(previousPose, TargetPose, lerpFactor);
            }

            currentPoseBlending = TargetPoseBlending;
            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            Pose.ComputeJointPoses(ControllerHandedness, rotation, position, HandData.Joints);
        }
    }
}