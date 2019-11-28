// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.UnityEditor;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.Simulation
{
    public class SimulationHandControllerDataProvider : BaseHandControllerDataProvider
    {
        private SimulationHandControllerDataProviderProfile profile;
        private SimulationTimeStampStopWatch handUpdateStopWatch;
        private long lastHandUpdateTimeStamp = 0;
        private Vector3? lastMousePosition = null;
        private SimulationHandPoseData defaultHandPose;

        /// <summary>
        /// The simulation state of the left hand.
        /// </summary>
        private SimulationHandState LeftHandState { get; set; }

        /// <summary>
        /// The simulation state of the right hand.
        /// </summary>
        private SimulationHandState RightHandState { get; set; }

        /// <summary>
        /// The most current simulation input information for the left hand.
        /// </summary>
        private SimulationInput LeftHandSimulationInput { get; } = new SimulationInput();

        /// <summary>
        /// The most current simulation input information for the right hand.
        /// </summary>
        private SimulationInput RightHandSimulationInput { get; } = new SimulationInput();

        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Hand controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public SimulationHandControllerDataProvider(string name, uint priority, SimulationHandControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying)
            {
                return;
            }

            // Initialize available simulated hand poses and find the
            // cofigured default pose.
            SimulationHandPose.Initialize(profile.PoseDefinitions);
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData pose = profile.PoseDefinitions[i];
                if (pose.IsDefault)
                {
                    defaultHandPose = pose;
                    break;
                }
            }

            // Simulation cannot work without a default pose.
            if (defaultHandPose == null)
            {
                Debug.LogError($"There is no default simulated hand pose defined. Check the {typeof(SimulationHandControllerDataProviderProfile).Name}!");
            }

            // Initialize states to default.
            LeftHandState = new SimulationHandState(profile, Handedness.Left, SimulationHandPose.GetPoseByName(defaultHandPose.GestureName));
            RightHandState = new SimulationHandState(profile, Handedness.Right, SimulationHandPose.GetPoseByName(defaultHandPose.GestureName));
            LeftHandState.Reset();
            RightHandState.Reset();

            // Start the timestamp stopwatch
            handUpdateStopWatch = new SimulationTimeStampStopWatch();
            handUpdateStopWatch.Reset();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (profile.IsSimulateHandTrackingEnabled)
            {
                // Read keyboard / mouse input for hand simulation.
                UpdateSimulationInput();

                // Calculate pose changes and compute timestamp for hand tracking update.
                float poseAnimationDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
                long timeStamp = handUpdateStopWatch.TimeStamp;

                // Update simualted hand states using collected data.
                LeftHandState.Update(timeStamp, LeftHandSimulationInput, poseAnimationDelta);
                RightHandState.Update(timeStamp, RightHandSimulationInput, poseAnimationDelta);

                lastMousePosition = Input.mousePosition;
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft / Right might have been modified after the data provider's Update() call.
            if (profile.IsSimulateHandTrackingEnabled)
            {
                DateTime currentTime = handUpdateStopWatch.Current;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandUpdateTimeStamp)).TotalMilliseconds;
                if (msSinceLastHandUpdate > profile.SimulatedUpdateFrequency)
                {
                    // Only update hands if the simulation state has changed since the last update.
                    if (LeftHandState.HandData.TimeStamp > lastHandUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Left, LeftHandState.HandData);
                    }

                    if (RightHandState.HandData.TimeStamp > lastHandUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Right, RightHandState.HandData);
                    }

                    lastHandUpdateTimeStamp = currentTime.Ticks;
                }
            }
        }

        /// <summary>
        /// Reads keyboard input to update whether hands should be tracked or always visible.
        /// </summary>
        private void UpdateSimulationInput()
        {
            // Left hand.
            if (Input.GetKeyDown(profile.ToggleLeftHandKey))
            {
                LeftHandSimulationInput.IsAlwaysVisible = !LeftHandSimulationInput.IsAlwaysVisible;
            }

            if (Input.GetKeyDown(profile.LeftHandTrackedKey))
            {
                LeftHandSimulationInput.IsTracking = true;
            }

            if (Input.GetKeyUp(profile.LeftHandTrackedKey))
            {
                LeftHandSimulationInput.IsTracking = false;
            }

            LeftHandSimulationInput.HandPositionDelta = GetHandPositionDelta();
            LeftHandSimulationInput.HandRotationDelta = GetHandRotationDelta();
            if (LeftHandSimulationInput.IsAlwaysVisible)
            {
                // When always visible is enabled, we don't want the hand
                // to return to default pose as soon as its keycode is released.
                // ToggleHandPose will instead toggle in between default and input pose.
                LeftHandSimulationInput.HandPose = ToggleHandPose(LeftHandState.Pose);
            }
            else
            {
                // When "tracking" / simulating the hand, return to default pose as soon as
                // no key is pressed.
                LeftHandSimulationInput.HandPose = SelectHandPose();
            }

            // Right hand.
            if (Input.GetKeyDown(profile.ToggleRightHandKey))
            {
                RightHandSimulationInput.IsAlwaysVisible = !RightHandSimulationInput.IsAlwaysVisible;
            }

            if (Input.GetKeyDown(profile.RightHandTrackedKey))
            {
                RightHandSimulationInput.IsTracking = true;
            }

            if (Input.GetKeyUp(profile.RightHandTrackedKey))
            {
                RightHandSimulationInput.IsTracking = false;
            }

            RightHandSimulationInput.HandPositionDelta = GetHandPositionDelta();
            RightHandSimulationInput.HandRotationDelta = GetHandRotationDelta();
            if (RightHandSimulationInput.IsAlwaysVisible)
            {
                // When always visible is enabled, we don't want the hand
                // to return to default pose as soon as its keycode is released.
                // ToggleHandPose will instead toggle in between default and input pose.
                RightHandSimulationInput.HandPose = ToggleHandPose(RightHandState.Pose);
            }
            else
            {
                // When "tracking" / simulating the hand, return to default pose as soon as
                // no key is pressed.
                RightHandSimulationInput.HandPose = SelectHandPose();
            }
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
        private SimulationHandPose SelectHandPose()
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData pose = profile.PoseDefinitions[i];
                if (Input.GetKey(pose.KeyCode))
                {
                    return SimulationHandPose.GetPoseByName(pose.GestureName);
                }
            }

            return SimulationHandPose.GetPoseByName(defaultHandPose.GestureName);
        }

        /// <summary>
        /// Toggles in between the hands default pose and a specified pose whenever the pose input keyode
        /// is pressed.
        /// </summary>
        /// <param name="currentPose">The name of the pose currently used.</param>
        /// <returns>Pose name of the pose toggled on. Either default pose or the current one.</returns>
        private SimulationHandPose ToggleHandPose(SimulationHandPose currentPose)
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData pose = profile.PoseDefinitions[i];
                if (Input.GetKeyDown(pose.KeyCode))
                {
                    return !string.Equals(currentPose.Id, pose.GestureName)
                        ? SimulationHandPose.GetPoseByName(pose.GestureName) : SimulationHandPose.GetPoseByName(defaultHandPose.GestureName);
                }
            }

            return currentPose;
        }
    }
}