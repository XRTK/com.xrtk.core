// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Handlers;

namespace XRTK.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Visualization Profile", fileName = "MixedRealityControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    public class MixedRealityControllerVisualizationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Controller Visualizer component to use on the rendered controller controllerModel.")]
        private SystemType controllerVisualizationType;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller controllerModel
        /// </summary>
        public SystemType ControllerVisualizationType => controllerVisualizationType;

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller models.")]
        private bool useDefaultModels;

        /// <summary>
        /// User the controller controllerModel loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModels => useDefaultModels;

        [Prefab]
        [SerializeField]
        [FormerlySerializedAs("model")]
        private GameObject controllerModel;

        public GameObject LeftHandModel => controllerModel;

        [SerializeField]
        [Tooltip("This is the controller pose that this visualization will synchronize it's position and rotation with.")]
        private InputActionReference pointerPose = null;

        /// <summary>
        /// This is the controller pose that this visualization will synchronize it's position and rotation with.
        /// </summary>
        public InputAction PointerPose => pointerPose.action;

        [SerializeField]
        [Tooltip("This is the controller pose that this visualization will synchronize it's position and rotation with.")]
        private InputActionReference alternatePointerPose = null;

        /// <summary>
        /// This is the controller pose that this visualization will synchronize it's position and rotation with.
        /// </summary>
        public InputAction AlternatePointerPose => alternatePointerPose.action;
    }
}
