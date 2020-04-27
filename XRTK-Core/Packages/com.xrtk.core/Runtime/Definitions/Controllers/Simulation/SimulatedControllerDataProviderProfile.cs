// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Controllers.Simulation
{
    public class SimulatedControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        #region General Settings

        [SerializeField]
        [Tooltip("Simulated update frequency for tracking data in milliseconds. 0ms is every frame.")]
        private double simulatedUpdateFrequency = 0;

        /// <summary>
        /// The simulated update frequency in milliseconds mimics the hardware's ability to
        /// update controller tracking data. A value of 0ms will provide data
        /// updates every frame.
        /// </summary>
        public double SimulatedUpdateFrequency => simulatedUpdateFrequency;

        [SerializeField]
        [Tooltip("Time after which uncontrolled controllers are hidden")]
        private float controllerHideTimeout = 0.2f;

        /// <summary>
        /// Time after which uncontrolled controllers are hidden
        /// </summary>
        public float ControllerHideTimeout => controllerHideTimeout;

        #endregion

        #region Placement Settings

        [SerializeField]
        [Tooltip("Default distance of the controller from the camera")]
        private float defaultDistance = 0.5f;

        /// <summary>
        /// Default distance of the controller from the camera.
        /// </summary>
        public float DefaultDistance => defaultDistance;

        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float depthMultiplier = 0.5f;

        /// <summary>
        /// Depth change when scrolling the mouse wheel.
        /// </summary>
        public float DepthMultiplier => depthMultiplier;

        [SerializeField]
        [Tooltip("Apply random offset to the controller position")]
        private float jitterAmount = 0.0f;

        /// <summary>
        /// Apply random offset to the controller position.
        /// </summary>
        public float JitterAmount => jitterAmount;

        #endregion

        #region Controls Settings

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
        [Tooltip("Angle per second when rotating the controller")]
        private float rotationSpeed = 100.0f;

        /// <summary>
        /// Angle per second when rotating the controller.
        /// </summary>
        public float RotationSpeed => rotationSpeed;

        #endregion

        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return null;
        }
    }
}