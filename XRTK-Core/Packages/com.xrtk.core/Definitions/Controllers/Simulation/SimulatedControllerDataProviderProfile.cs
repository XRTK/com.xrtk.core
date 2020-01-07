// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers.Simulation;

namespace XRTK.Definitions.Controllers.Simulation
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Simulated Controller Data Provider Profile", fileName = "SimulatedControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class SimulatedControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        #region General Settings

        [Header("General Settings")]

        [SerializeField]
        [Tooltip("Enable controller simulation")]
        private bool controllerSimulationEnabled = false;

        /// <summary>
        /// Is hand simulated hand tracking enabled?
        /// </summary>
        public bool ControllerSimulationEnabled => controllerSimulationEnabled;

        [SerializeField]
        [Implements(typeof(IMixedRealitySimulatedController), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete controller type to simulate.")]
        private SystemType simulatedControllerType = null;

        /// <summary>
        /// The concrete controller type to simulate.
        /// </summary>
        public SystemType SimulatedControllerType => simulatedControllerType;

        [SerializeField]
        [Tooltip("Simulated update frequency for tracking data in milliseconds. 0ms is every frame.")]
        private double simulatedUpdateFrequency = 0;

        /// <summary>
        /// The simulated update frequency in milliseconds mimicks the hardware's ability to
        /// update controller tracking data. A value of 0ms will provide data
        /// updates every frame.
        /// </summary>
        public double SimulatedUpdateFrequency => simulatedUpdateFrequency;

        [SerializeField]
        [Tooltip("Time after which uncontrolled controllers are hidden")]
        private float controllerHideTimeout = 0.2f;

        /// <summary>
        /// ime after which uncontrolled controllers are hidden
        /// </summary>
        public float ControllerHideTimeout => controllerHideTimeout;

        #endregion

        #region Placement Settings

        [Header("Placement Settings")]

        [SerializeField]
        [Tooltip("Default distance of the controller from the camera")]
        private float defaultDistance = 0.5f;

        /// <summary>
        /// Default distance of the controller from the camera.
        /// </summary>
        public float DefaultDistance => defaultDistance;

        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float depthMultiplier = 0.1f;

        /// <summary>
        /// Depth change when scrolling the mouse wheel.
        /// </summary>
        public float HandDepthMultiplier => depthMultiplier;

        [SerializeField]
        [Tooltip("Apply random offset to the controller position")]
        private float jitterAmount = 0.0f;

        /// <summary>
        /// Apply random offset to the controller position.
        /// </summary>
        public float JitterAmount => jitterAmount;

        #endregion

        #region Controls Settings

        [Header("Controls Settings")]

        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left controller")]
        private KeyCode toggleLeftPersistentKey = KeyCode.T;

        /// <summary>
        /// Key to toggle persistent mode for the left controller.
        /// </summary>
        public KeyCode ToggleLeftPersistentKey => toggleLeftPersistentKey;

        [SerializeField]
        [Tooltip("Key to simulate tracking of the left controller")]
        private KeyCode leftControllerTrackedKey = KeyCode.LeftShift;

        /// <summary>
        /// Key to simulate tracking of the left controller.
        /// </summary>
        public KeyCode LeftControllerTrackedKey => leftControllerTrackedKey;

        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right controller")]
        private KeyCode toggleRightPersistentKey = KeyCode.Y;

        /// <summary>
        /// Key to toggle persistent mode for the right controller
        /// </summary>
        public KeyCode ToggleRightPersistentKey => toggleRightPersistentKey;

        [SerializeField]
        [Tooltip("Key to simulate tracking of the right controller")]
        private KeyCode rightControllerTrackedKey = KeyCode.Space;

        /// <summary>
        /// Key to simulate tracking of the right controller.
        /// </summary>
        public KeyCode RightControllerTrackedKey => rightControllerTrackedKey;

        [SerializeField]
        [Tooltip("Key to turn the controller clockwise")]
        private KeyCode yawCWKey = KeyCode.E;

        /// <summary>
        /// Key to turn the controller clockwise.
        /// </summary>
        public KeyCode YawCWKey => yawCWKey;

        [SerializeField]
        [Tooltip("Key to turn the controller counter-clockwise")]
        private KeyCode yawCCWKey = KeyCode.Q;

        /// <summary>
        /// Key to turn the controller counter-clockwise.
        /// </summary>
        public KeyCode YawCCWKey => yawCCWKey;

        [SerializeField]
        [Tooltip("Key to pitch the controller upward")]
        private KeyCode pitchCWKey = KeyCode.F;

        /// <summary>
        /// Key to pitch the controller upward.
        /// </summary>
        public KeyCode PitchCWKey => pitchCWKey;

        [SerializeField]
        [Tooltip("Key to pitch the controller downward")]
        private KeyCode pitchCCWKey = KeyCode.R;

        /// <summary>
        /// Key to pitch the controller downward.
        /// </summary>
        public KeyCode PitchCCWKey => pitchCCWKey;

        [SerializeField]
        [Tooltip("Key to roll the controller right")]
        private KeyCode rollCWKey = KeyCode.X;

        /// <summary>
        /// Key to roll the controller right.
        /// </summary>
        public KeyCode RollCWKey => rollCWKey;

        [SerializeField]
        [Tooltip("Key to roll the controller left")]
        private KeyCode rollCCWKey = KeyCode.Z;

        /// <summary>
        /// Key to roll the controller left.
        /// </summary>
        public KeyCode RollCCWKey => rollCCWKey;

        [SerializeField]
        [Tooltip("Angle per second when rotating the controller")]
        private float rotationSpeed = 100.0f;

        /// <summary>
        /// Angle per second when rotating the controller.
        /// </summary>
        public float RotationSpeed => rotationSpeed;

        #endregion

        #region Simulated Hand Controller Settings

        [Header("Simulated Hand Controller Settings")]

        [SerializeField]
        [Tooltip("Hand pose definitions.")]
        private List<SimulatedHandControllerPoseData> poseDefinitions = new List<SimulatedHandControllerPoseData>();

        /// <summary>
        /// Hand pose definitions.
        /// </summary>
        public IReadOnlyList<SimulatedHandControllerPoseData> PoseDefinitions => poseDefinitions;

        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handPoseAnimationSpeed = 8.0f;

        /// <summary>
        /// Pose interpolation per second.
        /// </summary>
        public float HandPoseAnimationSpeed => handPoseAnimationSpeed;

        #endregion
    }
}