// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands.UnityEditor;

namespace XRTK.Definitions.InputSystem.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Hands/Unity Editor Hand Controller Data Provider Profile", fileName = "UnityEditorHandControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class UnityEditorHandControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        [Header("Hand Simulation")]

        [SerializeField]
        [Tooltip("Enable hand simulation")]
        private bool isSimulateHandTrackingEnabled = false;

        /// <summary>
        /// Is hand simulation enabled?
        /// </summary>
        public bool IsSimulateHandTrackingEnabled => isSimulateHandTrackingEnabled;

        [Header("Hand Control Settings")]

        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left hand")]
        private KeyCode toggleLeftHandKey = KeyCode.T;

        /// <summary>
        /// Key to toggle persistent mode for the left hand
        /// </summary>
        public KeyCode ToggleLeftHandKey => toggleLeftHandKey;

        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right hand")]
        private KeyCode toggleRightHandKey = KeyCode.Y;

        /// <summary>
        /// Key to toggle persistent mode for the right hand
        /// </summary>
        public KeyCode ToggleRightHandKey => toggleRightHandKey;

        [SerializeField]
        [Tooltip("Time after which uncontrolled hands are hidden")]
        private float handHideTimeout = 0.2f;

        /// <summary>
        /// ime after which uncontrolled hands are hidden
        /// </summary>
        public float HandHideTimeout => handHideTimeout;

        [SerializeField]
        [Tooltip("Key to simulate tracking of the left hand")]
        private KeyCode leftHandTrackedKey = KeyCode.LeftShift;

        /// <summary>
        /// Key to simulate tracking of the left hand.
        /// </summary>
        public KeyCode LeftHandTrackedKey => leftHandTrackedKey;

        [SerializeField]
        [Tooltip("Key to simulate tracking of the right hand")]
        private KeyCode rightHandTrackedKey = KeyCode.Space;

        /// <summary>
        /// Key to simulate tracking of the right hand.
        /// </summary>
        public KeyCode RightHandTrackedKey => rightHandTrackedKey;

        [Header("Hand Pose Settings")]

        [SerializeField]
        [Tooltip("Hand pose definitions.")]
        private List<UnityEditorHandPoseData> poseDefinitions = new List<UnityEditorHandPoseData>();

        /// <summary>
        /// Hand pose definitions.
        /// </summary>
        public IReadOnlyList<UnityEditorHandPoseData> PoseDefinitions => poseDefinitions;

        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handGestureAnimationSpeed = 8.0f;

        /// <summary>
        /// Gesture interpolation per second
        /// </summary>
        public float HandGestureAnimationSpeed => handGestureAnimationSpeed;

        [SerializeField]
        [Tooltip("Time until hold gesture starts")]
        private float holdStartDuration = 0.5f;

        /// <summary>
        /// Time until hold gesture starts
        /// </summary>
        public float HoldStartDuration => holdStartDuration;

        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to start a manipulation")]
        private float manipulationStartThreshold = 0.03f;

        /// <summary>
        /// The total amount of input source movement that needs to happen to start a manipulation
        /// </summary>
        public float ManipulationStartThreshold => manipulationStartThreshold;

        [Header("Hand Placement Settings")]

        [SerializeField]
        [Tooltip("Default distance of the hand from the camera")]
        private float defaultHandDistance = 0.5f;

        /// <summary>
        /// Default distance of the hand from the camera
        /// </summary>
        public float DefaultHandDistance => defaultHandDistance;

        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float handDepthMultiplier = 0.1f;

        /// <summary>
        /// Depth change when scrolling the mouse wheel
        /// </summary>
        public float HandDepthMultiplier => handDepthMultiplier;

        [SerializeField]
        [Tooltip("Apply random offset to the hand position")]
        private float handJitterAmount = 0.0f;

        /// <summary>
        /// Apply random offset to the hand position
        /// </summary>
        public float HandJitterAmount => handJitterAmount;

        [Header("Hand Rotation Settings")]

        [SerializeField]
        [Tooltip("Key to turn the hand clockwise")]
        private KeyCode yawHandCWKey = KeyCode.E;

        /// <summary>
        /// Key to turn the hand clockwise
        /// </summary>
        public KeyCode YawHandCWKey => yawHandCWKey;

        [SerializeField]
        [Tooltip("Key to turn the hand counter-clockwise")]
        private KeyCode yawHandCCWKey = KeyCode.Q;

        /// <summary>
        /// Key to turn the hand counter-clockwise
        /// </summary>
        public KeyCode YawHandCCWKey => yawHandCCWKey;

        [SerializeField]
        [Tooltip("Key to pitch the hand upward")]
        private KeyCode pitchHandCWKey = KeyCode.F;

        /// <summary>
        /// Key to pitch the hand upward
        /// </summary>
        public KeyCode PitchHandCWKey => pitchHandCWKey;

        [SerializeField]
        [Tooltip("Key to pitch the hand downward")]
        private KeyCode pitchHandCCWKey = KeyCode.R;

        /// <summary>
        /// Key to pitch the hand downward
        /// </summary>
        public KeyCode PitchHandCCWKey => pitchHandCCWKey;

        [SerializeField]
        [Tooltip("Key to roll the hand right")]
        private KeyCode rollHandCWKey = KeyCode.X;

        /// <summary>
        /// Key to roll the hand right
        /// </summary>
        public KeyCode RollHandCWKey => rollHandCWKey;

        [SerializeField]
        [Tooltip("Key to roll the hand left")]
        private KeyCode rollHandCCWKey = KeyCode.Z;

        /// <summary>
        /// Key to roll the hand left
        /// </summary>
        public KeyCode RollHandCCWKey => rollHandCCWKey;

        [SerializeField]
        [Tooltip("Angle per second when rotating the hand")]
        private float handRotationSpeed = 100.0f;

        /// <summary>
        /// Angle per second when rotating the hand
        /// </summary>
        public float HandRotationSpeed => handRotationSpeed;
    }
}