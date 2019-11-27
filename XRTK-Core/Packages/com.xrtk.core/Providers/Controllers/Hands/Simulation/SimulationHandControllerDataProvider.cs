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
        /// If true then the left hand is always visible,, even if it's currently
        /// not being tracked.
        /// </summary>
        private bool IsAlwaysVisibleLeft { get; set; } = false;

        /// <summary>
        /// If true then the right hand is always visible, even if it's currently
        /// not being tracked.
        /// </summary>
        private bool IsAlwaysVisibleRight { get; set; } = false;

        /// <summary>
        /// If true, the left hand is currently being tracked.
        /// </summary>
        private bool IsTrackingLeftHand { get; set; } = false;

        /// <summary>
        /// If true, the right hand is currently being tracked.
        /// </summary>
        private bool IsTrackingRightHand { get; set; } = false;

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
            SimulatedHandPose.Initialize(profile.PoseDefinitions);
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
            LeftHandState = new SimulationHandState(profile, Handedness.Left, SimulatedHandPose.GetPoseByName(defaultHandPose.GestureName));
            RightHandState = new SimulationHandState(profile, Handedness.Right, SimulatedHandPose.GetPoseByName(defaultHandPose.GestureName));
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
                UpdateSimulationInput();

                LeftHandState.Update(IsTrackingLeftHand, IsAlwaysVisibleLeft, GetHandPositionDelta(), GetHandRotationDelta());
                RightHandState.Update(IsTrackingRightHand, IsAlwaysVisibleRight, GetHandPositionDelta(), GetHandRotationDelta());

                float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
                LeftHandState.GestureBlending += gestureAnimDelta;
                RightHandState.GestureBlending += gestureAnimDelta;

                lastMousePosition = Input.mousePosition;

                long timeStamp = handUpdateStopWatch.TimeStamp;
                LeftHandState.UpdateWithTimeStamp(timeStamp, LeftHandState.HandData.IsTracked);
                RightHandState.UpdateWithTimeStamp(timeStamp, RightHandState.HandData.IsTracked);
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
            if (Input.GetKeyDown(profile.ToggleLeftHandKey))
            {
                IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
            }

            if (Input.GetKeyDown(profile.ToggleRightHandKey))
            {
                IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
            }

            if (Input.GetKeyDown(profile.LeftHandTrackedKey))
            {
                IsTrackingLeftHand = true;
            }

            if (Input.GetKeyUp(profile.LeftHandTrackedKey))
            {
                IsTrackingLeftHand = false;
            }

            if (Input.GetKeyDown(profile.RightHandTrackedKey))
            {
                IsTrackingRightHand = true;
            }

            if (Input.GetKeyUp(profile.RightHandTrackedKey))
            {
                IsTrackingRightHand = false;
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
        /// <returns><see cref="defaultHandPose"/> if no other fitting user input.</returns>
        private string SelectHandPose()
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData gesture = profile.PoseDefinitions[i];
                if (Input.GetKeyDown(gesture.KeyCode))
                {
                    return gesture.GestureName;
                }
            }

            return defaultHandPose.GestureName;
        }

        /// <summary>
        /// Toggles in between the hands default pose and a specified pose whenever the pose input keyode
        /// is pressed.
        /// </summary>
        /// <param name="poseName">The name of the pose to toggle.</param>
        /// <returns>Pose name of the pose toggled on. Either <see cref="defaultHandPose"/> or <paramref name="poseName"/></returns>
        private string ToggleHandPose(string poseName)
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                SimulationHandPoseData gesture = profile.PoseDefinitions[i];
                if (Input.GetKeyDown(gesture.KeyCode))
                {
                    return poseName != gesture.GestureName ? gesture.GestureName : defaultHandPose.GestureName;
                }
            }

            return defaultHandPose.GestureName;
        }
    }
}