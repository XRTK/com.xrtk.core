// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Simulation;
using XRTK.Providers.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Simulation.Hands
{
    /// <summary>
    /// The default hand controller implementation for the hand simulation.
    /// </summary>
    public class SimulatedHandController : BaseHandController, IMixedRealitySimulatedController
    {
        private Vector3? lastMousePosition = null;
        private readonly SimulationTimeStampStopWatch handUpdateStopWatch;
        private readonly SimulatedControllerDataProvider dataProvider;

        /// <summary>
        /// The simulation state of the left hand.
        /// </summary>
        private SimulatedHandControllerState HandState { get; set; }

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

            HandState = new SimulatedHandControllerState(profile, ControllerHandedness, SimulatedHandControllerPose.GetPoseByName(SimulatedHandControllerPose.DefaultHandPose.Id));
            HandState.Reset();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
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

            // Read keyboard / mouse input to determine the root pose delta since last frame.
            MixedRealityPose rootPoseDelta = new MixedRealityPose(
                GetHandPositionDelta(), Quaternion.Euler(GetHandRotationDelta()));

            // Calculate pose changes and compute timestamp for hand tracking update.
            float poseAnimationDelta = dataProvider.HandPoseAnimationSpeed * Time.deltaTime;
            long timeStamp = handUpdateStopWatch.TimeStamp;

            // Update simualted hand states using collected data.
            HandState.Update(timeStamp, rootPoseDelta, GetTargetHandPose(), poseAnimationDelta);

            lastMousePosition = Input.mousePosition;

            UpdateBase(HandState.HandData);
        }

        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <returns>Updated hand rotation angles.</returns>
        private Vector3 GetHandRotationDelta()
        {
            float rotationDelta = dataProvider.RotationSpeed * Time.deltaTime;
            Vector3 rotationDeltaEulerAngles = Vector3.zero;

            if (Input.GetKey(dataProvider.YawControllerCounterClockwiseKey))
            {
                rotationDeltaEulerAngles.y = -rotationDelta;
            }
            if (Input.GetKey(dataProvider.YawControllerClockwiseKey))
            {
                rotationDeltaEulerAngles.y = rotationDelta;
            }
            if (Input.GetKey(dataProvider.PitchControllerCounterClockwiseKey))
            {
                rotationDeltaEulerAngles.x = rotationDelta;
            }
            if (Input.GetKey(dataProvider.PitchControllerClockwiseKey))
            {
                rotationDeltaEulerAngles.x = -rotationDelta;
            }
            if (Input.GetKey(dataProvider.RollControllerCouterClockwiseKey))
            {
                rotationDeltaEulerAngles.z = rotationDelta;
            }
            if (Input.GetKey(dataProvider.RollControllerClockwiseKey))
            {
                rotationDeltaEulerAngles.z = -rotationDelta;
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
            Vector3 mouseDelta = (lastMousePosition.HasValue ? Input.mousePosition - lastMousePosition.Value : Vector3.zero);
            mouseDelta.z += Input.GetAxis("Mouse ScrollWheel") * dataProvider.HandDepthMultiplier;

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
    }
}