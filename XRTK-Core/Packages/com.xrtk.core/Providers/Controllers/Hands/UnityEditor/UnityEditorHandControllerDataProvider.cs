// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    public class UnityEditorHandControllerDataProvider : BaseHandControllerDataProvider
    {
        private UnityEditorHandControllerDataProviderProfile profile;
        private long lastHandControllerUpdateTimeStamp = 0;

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; private set; } = true;

        private UnityEditorHandPoseData defaultPose;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleLeft = false;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleRight = false;

        private UnityEditorHandState leftHandState;
        private UnityEditorHandData leftHandData;
        private UnityEditorHandState rightHandState;
        private UnityEditorHandData rightHandData;

        // If true then hands are controlled by user input
        private bool isSimulatingLeft = false;
        private bool isSimulatingRight = false;
        // Last frame's mouse position for computing delta
        private Vector3? lastMousePosition = null;
        // Last timestamp when hands were tracked
        private long lastSimulatedTimestampLeft = 0;
        private long lastSimulatedTimestampRight = 0;
        // Cached delegates for hand joint generation
        private UnityEditorHandData.HandJointDataGenerator generatorLeft;
        private UnityEditorHandData.HandJointDataGenerator generatorRight;
        ///////////////////

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
                    defaultPose = pose;
                    leftHandState.GestureName = defaultPose.GestureName;
                    rightHandState.GestureName = defaultPose.GestureName;
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

            if (profile.IsSimulateHandTrackingEnabled && UserInputEnabled)
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

            handDataChanged |= handDataLeft.UpdateWithTimestamp(timestamp, leftHandState.IsTracked, leftHandState.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.UpdateWithTimestamp(timestamp, rightHandState.IsTracked, rightHandState.IsPinching, generatorRight);

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

            if (Input.GetKeyDown(profile.LeftHandManipulationKey))
            {
                isSimulatingLeft = true;
            }
            if (Input.GetKeyUp(profile.LeftHandManipulationKey))
            {
                isSimulatingLeft = false;
            }

            if (Input.GetKeyDown(profile.RightHandManipulationKey))
            {
                isSimulatingRight = true;
            }
            if (Input.GetKeyUp(profile.RightHandManipulationKey))
            {
                isSimulatingRight = false;
            }

            Vector3 mouseDelta = (lastMousePosition.HasValue ? Input.mousePosition - lastMousePosition.Value : Vector3.zero);
            mouseDelta.z += Input.GetAxis("Mouse ScrollWheel") * profile.HandDepthMultiplier;
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

            SimulateHandInput(ref lastSimulatedTimestampLeft, leftHandState, isSimulatingLeft, IsAlwaysVisibleLeft, mouseDelta, rotationDeltaEulerAngles);
            SimulateHandInput(ref lastSimulatedTimestampRight, rightHandState, isSimulatingRight, IsAlwaysVisibleRight, mouseDelta, rotationDeltaEulerAngles);

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            leftHandState.GestureBlending += gestureAnimDelta;
            rightHandState.GestureBlending += gestureAnimDelta;

            lastMousePosition = Input.mousePosition;
        }

        private void SimulateHandInput(
            ref long lastSimulatedTimestamp,
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
                    state.GestureName = SelectGesture();
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
                lastSimulatedTimestamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastSimulatedTimestamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    state.IsTracked = false;
                }
            }
        }

        private string SelectGesture()
        {
            for (int i = 0; i < profile.PoseDefinitions.Count; i++)
            {
                UnityEditorHandPoseData gesture = profile.PoseDefinitions[i];
                if (Input.GetKeyDown(gesture.KeyCode))
                {
                    return gesture.GestureName;
                }
            }

            return defaultPose.GestureName;
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
            return defaultPose.GestureName;
        }
    }
}