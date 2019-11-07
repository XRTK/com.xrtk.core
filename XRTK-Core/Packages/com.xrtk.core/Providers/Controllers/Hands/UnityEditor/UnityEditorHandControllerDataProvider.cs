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
        private bool isTrackingLeftHand = false;
        private long lastSimulatedTimeStampRightHand = 0;
        private bool isTrackingRightHand = false;

        private UnityEditorHandState leftHandState;
        private UnityEditorHandData leftHandData;
        private UnityEditorHandState rightHandState;
        private UnityEditorHandData rightHandData;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        private bool IsAlwaysVisibleLeft { get; set; } = false;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        private bool IsAlwaysVisibleRight { get; set; } = false;

        // Cached delegates for hand joint generation
        private UnityEditorHandData.HandJointDataGenerator generatorLeft;
        private UnityEditorHandData.HandJointDataGenerator generatorRight;

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

            leftHandData = new UnityEditorHandData();
            leftHandState = new UnityEditorHandState(Handedness.Left);
            rightHandData = new UnityEditorHandData();
            rightHandState = new UnityEditorHandState(Handedness.Right);

            UnityEditorHandPose.Initialize(profile.PoseDefinitions);
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                UnityEditorHandPoseData pose = profile.PoseDefinitions[i];
                if (pose.IsDefault)
                {
                    defaultHandPose = pose;
                    leftHandState.GestureName = defaultHandPose.GestureName;
                    rightHandState.GestureName = defaultHandPose.GestureName;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(leftHandState.GestureName))
            {
                Debug.LogError("There is no default editor hand pose defined.");
            }

            leftHandState.Reset();
            rightHandState.Reset();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (profile.IsSimulateHandTrackingEnabled)
            {
                UpdateUnityEditorHandData(leftHandData, rightHandData);
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
                    if (leftHandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Left, leftHandData);
                    }
                    if (rightHandData.TimeStamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandData(Handedness.Right, rightHandData);
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

            bool handDataChanged = false;
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            long timestamp = DateTime.UtcNow.Ticks;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = leftHandState.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = rightHandState.FillCurrentFrame;
            }

            handDataChanged |= handDataLeft.UpdateWithTimeStamp(timestamp, leftHandState.IsTracked, leftHandState.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.UpdateWithTimeStamp(timestamp, rightHandState.IsTracked, rightHandState.IsPinching, generatorRight);

            return handDataChanged;
        }

        /// <summary>
        /// Update hand state based on keyboard and mouse input
        /// </summary>
        private void SimulateUserInput()
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
                isTrackingLeftHand = true;
            }
            if (Input.GetKeyUp(profile.LeftHandTrackedKey))
            {
                isTrackingLeftHand = false;
            }

            if (Input.GetKeyDown(profile.RightHandTrackedKey))
            {
                isTrackingRightHand = true;
            }
            if (Input.GetKeyUp(profile.RightHandTrackedKey))
            {
                isTrackingRightHand = false;
            }

            SimulateHand(ref lastSimulatedTimeStampLeftHand, leftHandState, isTrackingLeftHand, IsAlwaysVisibleLeft, GetHandDepthDelta(), GetHandRotationDelta());
            SimulateHand(ref lastSimulatedTimeStampRightHand, rightHandState, isTrackingRightHand, IsAlwaysVisibleRight, GetHandDepthDelta(), GetHandRotationDelta());

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            leftHandState.GestureBlending += gestureAnimDelta;
            rightHandState.GestureBlending += gestureAnimDelta;

            lastMousePosition = Input.mousePosition;
        }

        private void SimulateHand(
            ref long lastSimulatedTimeStamp,
            UnityEditorHandState state,
            bool isSimulating,
            bool isAlwaysVisible,
            Vector3 mouseDelta,
            Vector3 rotationDeltaEulerAngles)
        {
            bool enableTracking = isAlwaysVisible || isSimulating;
            if (!state.IsTracked && enableTracking)
            {
                // Start at current mouse position
                Vector3 mousePos = Input.mousePosition;
                state.ScreenPosition = new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance);
            }

            if (isSimulating)
            {
                state.SimulateInput(mouseDelta, profile.HandJitterAmount, rotationDeltaEulerAngles);

                if (isAlwaysVisible)
                {
                    // Toggle gestures on/off
                    state.GestureName = ToggleGesture(state.GestureName);
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
        /// Gets a simulated depth tracking (hands closer / further from tracking device) update.
        /// </summary>
        /// <returns>Depth position delta.</returns>
        private Vector3 GetHandDepthDelta()
        {
            Vector3 mouseDelta = (lastMousePosition.HasValue ? Input.mousePosition - lastMousePosition.Value : Vector3.zero);
            mouseDelta.z += Input.GetAxis("Mouse ScrollWheel") * profile.HandDepthMultiplier;
            return mouseDelta;
        }

        /// <summary>
        /// Selects a hand pose to simulate.
        /// </summary>
        /// <returns>Default hand pose if no other requested by user input.</returns>
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

        private string ToggleGesture(string gestureName)
        {
            // TODO:
            //if (Input.GetMouseButtonDown(0))
            //{
            //    return gestureName != profile.LeftMouseHandGesture ? profile.LeftMouseHandGesture : profile.DefaultHandGesture;
            //}
            //else if (Input.GetMouseButtonDown(1))
            //{
            //    return gestureName != profile.RightMouseHandGesture ? profile.RightMouseHandGesture : profile.DefaultHandGesture;
            //}
            //else if (Input.GetMouseButtonDown(2))
            //{
            //    return gestureName != profile.MiddleMouseHandGesture ? profile.MiddleMouseHandGesture : profile.DefaultHandGesture;
            //}

            // 'Default' will not change the gesture
            return defaultHandPose.GestureName;
        }
    }
}