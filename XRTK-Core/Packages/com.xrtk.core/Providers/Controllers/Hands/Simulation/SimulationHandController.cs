// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands.Simulation;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    /// <summary>
    /// The default hand controller implementation for the hand simulation.
    /// </summary>
    public class SimulationHandController : BaseHandController
    {
        private Vector3? lastMousePosition = null;
        private readonly SimulationTimeStampStopWatch handUpdateStopWatch;
        private readonly SimulationHandControllerDataProviderProfile profile;

        /// <summary>
        /// The simulation state of the left hand.
        /// </summary>
        private SimulationHandState HandState { get; set; }

        /// <summary>
        /// The most current simulation input information for the left hand.
        /// </summary>
        private SimulationInput HandSimulationInput { get; } = new SimulationInput();

        public SimulationHandController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            profile = GetProfile();
            if (profile == null)
            {
                Debug.LogError($"Expected {typeof(SimulationHandControllerDataProvider).Name} to be the active hand controller data provider.");
            }

            // Initialize available simulated hand poses and find the
            // cofigured default pose.
            SimulationHandPose.Initialize(profile.PoseDefinitions);

            // Simulation cannot work without a default pose.
            if (SimulationHandPose.DefaultHandPose == null)
            {
                Debug.LogError($"There is no default simulated hand pose defined. Check the {GetType().Name}!");
            }

            HandState = new SimulationHandState(profile, ControllerHandedness, SimulationHandPose.GetPoseByName(SimulationHandPose.DefaultHandPose.Id));
            HandState.Reset();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
        }

        private SimulationHandControllerDataProviderProfile GetProfile()
        {
            if (MixedRealityToolkit.GetService<IMixedRealityHandControllerDataProvider>() is SimulationHandControllerDataProvider simulationHandControllerDataProvider)
            {
                return simulationHandControllerDataProvider.Profile;
            }

            return null;
        }

        /// <inheritdoc />
        public override void UpdateController()
        {
            base.UpdateController();

            // Read keyboard / mouse input for hand simulation.
            UpdateSimulationInput();

            // Calculate pose changes and compute timestamp for hand tracking update.
            float poseAnimationDelta = profile.HandPoseAnimationSpeed * Time.deltaTime;
            long timeStamp = handUpdateStopWatch.TimeStamp;

            // Update simualted hand states using collected data.
            HandState.Update(timeStamp, HandSimulationInput, poseAnimationDelta);

            lastMousePosition = Input.mousePosition;

            UpdateBase(HandState.HandData);
        }

        /// <summary>
        /// Reads keyboard input to update whether hands should be tracked or always visible.
        /// </summary>
        private void UpdateSimulationInput()
        {
            HandSimulationInput.HandPositionDelta = GetHandPositionDelta();
            HandSimulationInput.HandRotationDelta = GetHandRotationDelta();
            HandSimulationInput.HandPose = GetHandPose();
        }

        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <returns>Updated hand rotation angles.</returns>
        private Vector3 GetHandRotationDelta()
        {
            float rotationDelta = profile.HandRotationSpeed * Time.deltaTime;
            Vector3 rotationDeltaEulerAngles = Vector3.zero;

            if (Input.GetKey(profile.YawHandCCWKey))
            {
                rotationDeltaEulerAngles.y = -rotationDelta;
            }
            if (Input.GetKey(profile.YawHandCWKey))
            {
                rotationDeltaEulerAngles.y = rotationDelta;
            }
            if (Input.GetKey(profile.PitchHandCCWKey))
            {
                rotationDeltaEulerAngles.x = rotationDelta;
            }
            if (Input.GetKey(profile.PitchHandCWKey))
            {
                rotationDeltaEulerAngles.x = -rotationDelta;
            }
            if (Input.GetKey(profile.RollHandCCWKey))
            {
                rotationDeltaEulerAngles.z = rotationDelta;
            }
            if (Input.GetKey(profile.RollHandCWKey))
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
            mouseDelta.z += Input.GetAxis("Mouse ScrollWheel") * profile.HandDepthMultiplier;

            return mouseDelta;
        }

        /// <summary>
        /// Selects a hand pose to simulate, while its input keycode is pressed.
        /// </summary>
        /// <returns>Default pose if no other fitting user input.</returns>
        private SimulationHandPose GetHandPose()
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData pose = profile.PoseDefinitions[i];
                if (Input.GetKey(pose.KeyCode))
                {
                    return SimulationHandPose.GetPoseByName(pose.Id);
                }
            }

            return SimulationHandPose.GetPoseByName(SimulationHandPose.DefaultHandPose.Id);
        }
    }
}