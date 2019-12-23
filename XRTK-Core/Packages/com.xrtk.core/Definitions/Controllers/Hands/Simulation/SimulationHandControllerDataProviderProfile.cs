// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands.Simulation;

namespace XRTK.Definitions.Controllers.Hands.UnityEditor
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Hands/Simulation Hand Controller Data Provider Profile", fileName = "SimulationHandControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class SimulationHandControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        [Header("Hand Simulation Settings")]

        [SerializeField]
        [Tooltip("Enable hand simulation")]
        private bool handTrackingEnabled = false;

        /// <summary>
        /// Is hand simulated hand tracking enabled?
        /// </summary>
        public bool HandTrackingEnabled => handTrackingEnabled;

        [SerializeField]
        [Tooltip("Simulated update frequency for hand data in milliseconds. 0ms is every frame.")]
        private double simulatedUpdateFrequency = 0;

        /// <summary>
        /// The simulated update frequency in milliseconds mimicks the hardware's ability to
        /// update hand tracking data. A value of 0ms will provide simulated hand data
        /// updates every frame.
        /// </summary>
        public double SimulatedUpdateFrequency => simulatedUpdateFrequency;

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
        private List<SimulationHandPoseData> poseDefinitions = new List<SimulationHandPoseData>();

        /// <summary>
        /// Hand pose definitions.
        /// </summary>
        public IReadOnlyList<SimulationHandPoseData> PoseDefinitions => poseDefinitions;

        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handPoseAnimationSpeed = 8.0f;

        /// <summary>
        /// Pose interpolation per second.
        /// </summary>
        public float HandPoseAnimationSpeed => handPoseAnimationSpeed;

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