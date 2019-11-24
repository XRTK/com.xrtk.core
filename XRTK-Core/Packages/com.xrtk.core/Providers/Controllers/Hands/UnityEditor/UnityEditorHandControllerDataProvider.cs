// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.UnityEditor;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    public class UnityEditorHandControllerDataProvider : BaseHandControllerDataProvider
    {
        private UnityEditorHandControllerDataProviderProfile profile;
        private long lastHandControllerUpdateTimeStamp = 0;
        private Vector3? lastMousePosition = null;
        private UnityEditorHandPoseData defaultHandPose;
        private long lastSimulatedTimeStampLeftHand = 0;
        private long lastSimulatedTimeStampRightHand = 0;

        // Cached delegates for hand joint generation
        private UnityEditorHandData.HandJointDataGenerator generatorLeft;
        private UnityEditorHandData.HandJointDataGenerator generatorRight;

        private UnityEditorHandState LeftHandState { get; set; }

        private UnityEditorHandData LeftHandData { get; set; }

        private UnityEditorHandState RightHandState { get; set; }

        private UnityEditorHandData RightHandData { get; set; }

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

        public UnityEditorHandControllerDataProvider(string name, uint priority, UnityEditorHandControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying) { return; }

            LeftHandData = new UnityEditorHandData();
            LeftHandState = new UnityEditorHandState(Handedness.Left);
            RightHandData = new UnityEditorHandData();
            RightHandState = new UnityEditorHandState(Handedness.Right);

            SimulatedHandPose.Initialize(profile.PoseDefinitions);
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                UnityEditorHandPoseData pose = profile.PoseDefinitions[i];
                if (pose.IsDefault)
                {
                    defaultHandPose = pose;
                    LeftHandState.GestureName = defaultHandPose.GestureName;
                    RightHandState.GestureName = defaultHandPose.GestureName;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(LeftHandState.GestureName))
            {
                Debug.LogError("There is no default editor hand pose defined.");
            }

            LeftHandState.Reset();
            RightHandState.Reset();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (profile.IsSimulateHandTrackingEnabled)
            {
                UpdateUnityEditorHandData(LeftHandData, RightHandData);
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
                    if (LeftHandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Left, LeftHandData);
                    }
                    if (RightHandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Right, RightHandData);
                    }

                    lastHandControllerUpdateTimeStamp = currentTime.Ticks;
                }
            }
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        private bool UpdateUnityEditorHandData(UnityEditorHandData handDataLeft, UnityEditorHandData handDataRight)
        {
            SimulateUserInput();

            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            long timestamp = DateTime.UtcNow.Ticks;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = LeftHandState.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = RightHandState.FillCurrentFrame;
            }

            bool handDataChanged = false;
            handDataChanged |= handDataLeft.UpdateWithTimeStamp(timestamp, LeftHandState.IsTracked, LeftHandState.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.UpdateWithTimeStamp(timestamp, RightHandState.IsTracked, RightHandState.IsPinching, generatorRight);

            return handDataChanged;
        }

        /// <summary>
        /// Update hand state based on keyboard and mouse input
        /// </summary>
        private void SimulateUserInput()
        {
            UpdateSimulationState();

            SimulateHand(ref lastSimulatedTimeStampLeftHand, LeftHandState, IsTrackingLeftHand, IsAlwaysVisibleLeft);
            SimulateHand(ref lastSimulatedTimeStampRightHand, RightHandState, IsTrackingRightHand, IsAlwaysVisibleRight);

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            LeftHandState.GestureBlending += gestureAnimDelta;
            RightHandState.GestureBlending += gestureAnimDelta;

            lastMousePosition = Input.mousePosition;
        }

        private void SimulateHand(ref long lastSimulatedTimeStamp, UnityEditorHandState state, bool isSimulating, bool isAlwaysVisible)
        {
            // We are "tracking" the hand, if it's configured to always be visible or if
            // simulation is active.
            bool enableTracking = isAlwaysVisible || isSimulating;

            // If the hands state is changing from "not tracked" to being tracked, reset its position
            // to the current mouse position and default distance from the camera.
            if (!state.IsTracked && enableTracking)
            {
                Vector3 mousePos = Input.mousePosition;
                state.ScreenPosition = new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance);
            }

            // If we are simulating the hand currently, read input and update the hand state.
            if (isSimulating)
            {
                state.SimulateInput(GetHandPositionDelta(), profile.HandJitterAmount, GetHandRotationDelta());

                if (isAlwaysVisible)
                {
                    // Toggle gestures on/off
                    state.GestureName = ToggleHandPose(state.GestureName);
                }
                else
                {
                    // Enable gesture while mouse button is pressed
                    state.GestureName = SelectHandPose();
                }
            }

            // Update tracked state of a hand.
            // If hideTimeout value is null, hands will stay visible after tracking stops.
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            DateTime currentTime = DateTime.UtcNow;
            if (enableTracking)
            {
                state.IsTracked = true;
                lastSimulatedTimeStamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastSimulatedTimeStamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    state.IsTracked = false;
                }
            }
        }

        /// <summary>
        /// Updates the hand simulation state. Checks for input whether hands should be tracked or
        /// always visible.
        /// </summary>
        private void UpdateSimulationState()
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
                UnityEditorHandPoseData gesture = profile.PoseDefinitions[i];
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
                UnityEditorHandPoseData gesture = profile.PoseDefinitions[i];
                if (Input.GetKeyDown(gesture.KeyCode))
                {
                    return poseName != gesture.GestureName ? gesture.GestureName : defaultHandPose.GestureName;
                }
            }

            return defaultHandPose.GestureName;
        }
    }
}