// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityControllerVisualizationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Controller Visualizer component to use on the rendered controller model.")]
        private SystemType controllerVisualizationType = null;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller model
        /// </summary>
        public SystemType ControllerVisualizationType
        {
            get => controllerVisualizationType;
            private set => controllerVisualizationType = value;
        }

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller models.")]
        private bool useDefaultModels = false;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModels
        {
            get => useDefaultModels;
            private set => useDefaultModels = value;
        }

        [SerializeField]
        private GameObject leftHandModel = null;

        public GameObject LeftHandModel
        {
            get => leftHandModel;
            private set => leftHandModel = value;
        }

        [SerializeField]
        private GameObject rightHandModel = null;

        public GameObject RightHandModel
        {
            get => rightHandModel;
            private set => rightHandModel = value;
        }

        [SerializeField]
        [Tooltip("This is the controller pose that this visualization will synchronize it's position and rotation with.")]
        private MixedRealityInputAction pointerPose = MixedRealityInputAction.None;

        /// <summary>
        /// This is the controller pose that this visualization will synchronize it's position and rotation with.
        /// </summary>
        public MixedRealityInputAction PointerPose
        {
            get => pointerPose;
            private set => pointerPose = value;
        }
    }
}
