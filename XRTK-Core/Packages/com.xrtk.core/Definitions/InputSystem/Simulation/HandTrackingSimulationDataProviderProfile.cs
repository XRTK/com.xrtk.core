// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Simulation/Hand Tracking Simulation Profile", fileName = "MixedRealityHandTrackingSimulationProfile", order = (int)CreateProfileMenuItemIndices.Simulation)]
    public class HandTrackingSimulationDataProviderProfile : BaseMixedRealityProfile
    {
        [Header("Hand Simulation")]
        [SerializeField]
        [Tooltip("Enable hand simulation")]
        private bool simulateHandTracking = false;
        public bool SimulateHandTracking => simulateHandTracking;

        [SerializeField]
        private HandSimulationMode handSimulationMode = HandSimulationMode.Articulated;
        public HandSimulationMode HandSimulationMode => handSimulationMode;

        [Header("Hand Control Settings")]
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left hand")]
        private KeyCode toggleLeftHandKey = KeyCode.T;
        public KeyCode ToggleLeftHandKey => toggleLeftHandKey;
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right hand")]
        private KeyCode toggleRightHandKey = KeyCode.Y;
        public KeyCode ToggleRightHandKey => toggleRightHandKey;
        [SerializeField]
        [Tooltip("Time after which uncontrolled hands are hidden")]
        private float handHideTimeout = 0.2f;
        public float HandHideTimeout => handHideTimeout;
        [SerializeField]
        [Tooltip("Key to manipulate the left hand")]
        private KeyCode leftHandManipulationKey = KeyCode.LeftShift;
        public KeyCode LeftHandManipulationKey => leftHandManipulationKey;
        [SerializeField]
        [Tooltip("Key to manipulate the right hand")]
        private KeyCode rightHandManipulationKey = KeyCode.Space;
        public KeyCode RightHandManipulationKey => rightHandManipulationKey;

        [Header("Hand Gesture Settings")]
        [SerializeField]
        private ArticulatedHandPose.GestureId defaultHandGesture = ArticulatedHandPose.GestureId.Open;
        public ArticulatedHandPose.GestureId DefaultHandGesture => defaultHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId leftMouseHandGesture = ArticulatedHandPose.GestureId.Pinch;
        public ArticulatedHandPose.GestureId LeftMouseHandGesture => leftMouseHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId middleMouseHandGesture = ArticulatedHandPose.GestureId.None;
        public ArticulatedHandPose.GestureId MiddleMouseHandGesture => middleMouseHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId rightMouseHandGesture = ArticulatedHandPose.GestureId.None;
        public ArticulatedHandPose.GestureId RightMouseHandGesture => rightMouseHandGesture;
        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handGestureAnimationSpeed = 8.0f;
        public float HandGestureAnimationSpeed => handGestureAnimationSpeed;

        [SerializeField]
        [Tooltip("Time until hold gesture starts")]
        private float holdStartDuration = 0.5f;
        public float HoldStartDuration => holdStartDuration;
        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to start a manipulation")]
        private float manipulationStartThreshold = 0.03f;
        public float ManipulationStartThreshold => manipulationStartThreshold;

        [Header("Hand Placement Settings")]
        [SerializeField]
        [Tooltip("Default distance of the hand from the camera")]
        private float defaultHandDistance = 0.5f;
        public float DefaultHandDistance => defaultHandDistance;
        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float handDepthMultiplier = 0.1f;
        public float HandDepthMultiplier => handDepthMultiplier;
        [SerializeField]
        [Tooltip("Apply random offset to the hand position")]
        private float handJitterAmount = 0.0f;
        public float HandJitterAmount => handJitterAmount;

        [Header("Hand Rotation Settings")]
        [SerializeField]
        [Tooltip("Key to turn the hand clockwise")]
        private KeyCode yawHandCWKey = KeyCode.E;
        public KeyCode YawHandCWKey => yawHandCWKey;
        [SerializeField]
        [Tooltip("Key to turn the hand counter-clockwise")]
        private KeyCode yawHandCCWKey = KeyCode.Q;
        public KeyCode YawHandCCWKey => yawHandCCWKey;
        [SerializeField]
        [Tooltip("Key to pitch the hand upward")]
        private KeyCode pitchHandCWKey = KeyCode.F;
        public KeyCode PitchHandCWKey => pitchHandCWKey;
        [SerializeField]
        [Tooltip("Key to pitch the hand downward")]
        private KeyCode pitchHandCCWKey = KeyCode.R;
        public KeyCode PitchHandCCWKey => pitchHandCCWKey;
        [SerializeField]
        [Tooltip("Key to roll the hand right")]
        private KeyCode rollHandCWKey = KeyCode.X;
        public KeyCode RollHandCWKey => rollHandCWKey;
        [SerializeField]
        [Tooltip("Key to roll the hand left")]
        private KeyCode rollHandCCWKey = KeyCode.Z;
        public KeyCode RollHandCCWKey => rollHandCCWKey;
        [SerializeField]
        [Tooltip("Angle per second when rotating the hand")]
        private float handRotationSpeed = 100.0f;
        public float HandRotationSpeed => handRotationSpeed;
    }
}