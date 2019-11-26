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
        private long lastHandControllerUpdateTimeStamp = 0;
        private Vector3? lastMousePosition = null;
        private SimulationHandPoseData defaultHandPose;

        private SimulationHandState LeftHandState { get; set; }

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

            if (defaultHandPose == null)
            {
                Debug.LogError("There is no default simulated hand pose defined.");
            }

            LeftHandState = new SimulationHandState(profile, Handedness.Left, SimulatedHandPose.GetPoseByName(defaultHandPose.GestureName));
            RightHandState = new SimulationHandState(profile, Handedness.Right, SimulatedHandPose.GetPoseByName(defaultHandPose.GestureName));

            LeftHandState.Reset();
            RightHandState.Reset();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (profile.IsSimulateHandTrackingEnabled)
            {
                UpdateUnityEditorHandData();
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft / Right can be modified after the services Update() call.
            if (profile.IsSimulateHandTrackingEnabled)
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandControllerUpdateTimeStamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    if (LeftHandState.HandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Left, LeftHandState.HandData);
                    }
                    if (RightHandState.HandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Right, RightHandState.HandData);
                    }

                    lastHandControllerUpdateTimeStamp = currentTime.Ticks;
                }
            }
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        private bool UpdateUnityEditorHandData()
        {
            UpdateSimulationInput();

            LeftHandState.Update(IsTrackingLeftHand, IsAlwaysVisibleLeft, GetHandPositionDelta(), GetHandRotationDelta());
            RightHandState.Update(IsTrackingRightHand, IsAlwaysVisibleRight, GetHandPositionDelta(), GetHandRotationDelta());

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            LeftHandState.GestureBlending += gestureAnimDelta;
            RightHandState.GestureBlending += gestureAnimDelta;

            lastMousePosition = Input.mousePosition;

            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            long timestamp = DateTime.UtcNow.Ticks;

            bool handDataChanged = false;
            handDataChanged |= LeftHandState.UpdateWithTimeStamp(timestamp, LeftHandState.HandData.IsTracked);
            handDataChanged |= RightHandState.UpdateWithTimeStamp(timestamp, RightHandState.HandData.IsTracked);

            return handDataChanged;
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