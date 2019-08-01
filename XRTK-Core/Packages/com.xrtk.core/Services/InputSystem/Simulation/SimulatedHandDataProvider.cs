// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Definitions.Utilities;

/// <summary>
/// Provides per-frame data access to simulated hand data
/// 
/// Controls for mouse/keyboard simulation:
/// - Press spacebar to turn right hand on/off
/// - Left mouse button brings index and thumb together
/// - Mouse moves left and right hand.
/// </summary>
namespace XRTK.Services.InputSystem.Simulation
{
    /// <summary>
    /// Produces simulated data every frame that defines joint positions.
    /// </summary>
    public class SimulatedHandDataProvider
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        private SimulatedHandGesture defaultGesture;
        protected HandTrackingSimulationDataProviderProfile profile;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleLeft = false;
        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleRight = false;

        private SimulatedHandState HandStateLeft;
        private SimulatedHandState HandStateRight;

        // If true then hands are controlled by user input
        private bool isSimulatingLeft = false;
        private bool isSimulatingRight = false;
        // Last frame's mouse position for computing delta
        private Vector3? lastMousePosition = null;
        // Last timestamp when hands were tracked
        private long lastSimulatedTimestampLeft = 0;
        private long lastSimulatedTimestampRight = 0;
        // Cached delegates for hand joint generation
        private SimulatedHandData.HandJointDataGenerator generatorLeft;
        private SimulatedHandData.HandJointDataGenerator generatorRight;

        public SimulatedHandDataProvider(HandTrackingSimulationDataProviderProfile profile)
        {
            this.profile = profile;

            HandStateLeft = new SimulatedHandState(Handedness.Left);
            HandStateRight = new SimulatedHandState(Handedness.Right);

            for (int i = 0; i < this.profile.GestureDefinitions.Count; i++)
            {
                SimulatedHandGesture gesture = this.profile.GestureDefinitions[i];
                if (gesture.IsDefault)
                {
                    defaultGesture = gesture;
                    HandStateLeft.GestureName = defaultGesture.GestureName;
                    HandStateRight.GestureName = defaultGesture.GestureName;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(HandStateLeft.GestureName))
            {
                Debug.LogError("There is no default simulated hand gesture defined.");
            }

            HandStateLeft.Reset();
            HandStateRight.Reset();
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        public bool UpdateHandData(SimulatedHandData handDataLeft, SimulatedHandData handDataRight)
        {
            SimulateUserInput();

            bool handDataChanged = false;
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            long timestamp = DateTime.UtcNow.Ticks;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = HandStateLeft.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = HandStateRight.FillCurrentFrame;
            }

            handDataChanged |= handDataLeft.UpdateWithTimestamp(timestamp, HandStateLeft.IsTracked, HandStateLeft.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.UpdateWithTimestamp(timestamp, HandStateRight.IsTracked, HandStateRight.IsPinching, generatorRight);

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

            SimulateHandInput(ref lastSimulatedTimestampLeft, HandStateLeft, isSimulatingLeft, IsAlwaysVisibleLeft, mouseDelta, rotationDeltaEulerAngles);
            SimulateHandInput(ref lastSimulatedTimestampRight, HandStateRight, isSimulatingRight, IsAlwaysVisibleRight, mouseDelta, rotationDeltaEulerAngles);

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            HandStateLeft.GestureBlending += gestureAnimDelta;
            HandStateRight.GestureBlending += gestureAnimDelta;

            lastMousePosition = Input.mousePosition;
        }

        /// Apply changes to one hand and update tracking
        private void SimulateHandInput(
            ref long lastSimulatedTimestamp,
            SimulatedHandState state,
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
            for (int i = 0; i < profile.GestureDefinitions.Count; i++)
            {
                SimulatedHandGesture gesture = profile.GestureDefinitions[i];
                if (Input.GetKeyDown(gesture.KeyCode))
                {
                    return gesture.GestureName;
                }
            }

            return defaultGesture.GestureName;
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
            return defaultGesture.GestureName;
        }
    }
}
