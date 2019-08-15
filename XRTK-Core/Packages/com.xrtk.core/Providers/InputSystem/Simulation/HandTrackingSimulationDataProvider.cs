// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.InputSystem.Simulation;
using XRTK.Providers.Controllers;
using XRTK.Services;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Providers.InputSystem.Simulation
{
    public class HandTrackingSimulationDataProvider : BaseControllerDataProvider, IHandTrackingSimulationDataProvider
    {
        private HandTrackingSimulationDataProviderProfile profile;
        private long lastHandControllerUpdateTimeStamp = 0;
        private readonly Dictionary<Handedness, SimulatedArticulatedHand> trackedHandControllers = new Dictionary<Handedness, SimulatedArticulatedHand>();
        
        /// <summary>
        /// Gets left hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();

        /// <summary>
        /// Gets right hand simulated data.
        /// </summary>
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; private set; } = true;

        /// <inheritdoc />
        public override IMixedRealityController[] GetActiveControllers() => trackedHandControllers.Values.ToArray();

        /////////////////////
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        private SimulatedHandGesture defaultGesture;

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
        ///////////////////

        /// <summary>
        /// Dictionary to capture all active hands detected.
        /// </summary>
        public IReadOnlyDictionary<Handedness, SimulatedArticulatedHand> TrackedHands => trackedHandControllers;

        public HandTrackingSimulationDataProvider(string name, uint priority, HandTrackingSimulationDataProviderProfile profile)
            : base(name, priority, profile)
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

        /// <inheritdoc />
        public override void Initialize()
        {
            ArticulatedHandPose.LoadGesturePoses(profile.GestureDefinitions);
        }

        /// <inheritdoc />
        public override void Disable()
        {
            RemoveAllHandControllers();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (profile.SimulateHandTracking && UserInputEnabled)
            {
                UpdateHandData(HandDataLeft, HandDataRight);
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (profile.SimulateHandTracking)
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandControllerUpdateTimeStamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    if (HandDataLeft.Timestamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Left, HandDataLeft);
                    }
                    if (HandDataRight.Timestamp > lastHandControllerUpdateTimeStamp)
                    {
                        UpdateHandInputSource(Handedness.Right, HandDataRight);
                    }

                    lastHandControllerUpdateTimeStamp = currentTime.Ticks;
                }
            }
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

        // Register input sources for hands based on changes of the data provider
        private void UpdateHandInputSource(Handedness handedness, SimulatedHandData handData)
        {
            if (!profile.SimulateHandTracking)
            {
                RemoveAllHandControllers();
            }
            else
            {
                if (handData != null && handData.IsTracked)
                {
                    SimulatedArticulatedHand controller = GetOrAddHandController(handedness);
                    if (controller != null)
                    {
                        controller.UpdateState(handData);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create {typeof(SimulatedArticulatedHand)} controller");
                    }
                }
                else
                {
                    RemoveHandController(handedness);
                }
            }
        }

        private SimulatedArticulatedHand GetOrAddHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out SimulatedArticulatedHand controller))
            {
                return controller;
            }

            IMixedRealityPointer[] pointers = RequestPointers(typeof(SimulatedArticulatedHand), handedness);
            IMixedRealityInputSource inputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource($"{handedness} Hand", pointers);
            controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);

            if (controller == null || !controller.SetupConfiguration(typeof(SimulatedArticulatedHand)))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            MixedRealityToolkit.InputSystem.RaiseSourceDetected(controller.InputSource, controller);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers)
            {
                controller.TryRenderControllerModel(typeof(SimulatedArticulatedHand));
            }

            trackedHandControllers.Add(handedness, controller);
            return controller;
        }

        private void RemoveHandController(Handedness handedness)
        {
            if (trackedHandControllers.TryGetValue(handedness, out SimulatedArticulatedHand controller))
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
                trackedHandControllers.Remove(handedness);
            }
        }

        private void RemoveAllHandControllers()
        {
            foreach (var controller in trackedHandControllers.Values)
            {
                MixedRealityToolkit.InputSystem.RaiseSourceLost(controller.InputSource, controller);
            }

            trackedHandControllers.Clear();
        }
    }
}