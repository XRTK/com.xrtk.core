// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.DataProviders.Controllers;

namespace XRTK.DataProviders.Controllers
{
    /// <summary>
    /// Used to define a controller's visualization settings.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerVisualizationSetting
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description">Description of the Device.</param>
        /// <param name="controllerType">Controller Type to instantiate at runtime.</param>
        /// <param name="handedness">The designated hand that the device is managing.</param>
        /// <param name="overrideModel">The controller model prefab to be rendered.</param>
        /// <param name="controllerVisualizationType">The <see cref="IMixedRealityControllerVisualizer"/> type to use for this controller setting.</param>
        /// <param name="poseAction">The <see cref="MixedRealityInputAction"/> pose to use for tracking the controller.</param>
        /// <param name="alternatePoseAction">The alternate pose action to use for certain controller variants.</param>
        public MixedRealityControllerVisualizationSetting(string description, Type controllerType, Handedness handedness = Handedness.None, GameObject overrideModel = null, SystemType controllerVisualizationType = null, MixedRealityInputAction poseAction = default, MixedRealityInputAction alternatePoseAction = default) : this()
        {
            this.description = description;
            this.controllerType = new SystemType(controllerType);
            this.handedness = handedness;
            this.overrideModel = overrideModel;
            this.controllerVisualizationType = controllerVisualizationType;

            if (poseAction == default)
            {
                poseAction = MixedRealityInputAction.None;
            }

            this.poseAction = poseAction;

            if (alternatePoseAction == default)
            {
                alternatePoseAction = MixedRealityInputAction.None;
            }

            this.alternatePoseAction = alternatePoseAction;
            useDefaultModel = false;
        }

        [SerializeField]
        private string description;

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType => controllerType;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller model for this controller.")]
        private bool useDefaultModel;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModel => useDefaultModel;

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private GameObject overrideModel;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject OverrideControllerModel => overrideModel;

        [SerializeField]
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Controller Visualizer component to use on the rendered controller model.")]
        private SystemType controllerVisualizationType;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller model
        /// </summary>
        public SystemType ControllerVisualizationType
        {
            get => controllerVisualizationType;
            private set => controllerVisualizationType = value;
        }

        [SerializeField]
        [Tooltip("The MixedRealityInputAction pose to use for tracking the controller.")]
        private MixedRealityInputAction poseAction;

        /// <summary>
        /// The <see cref="MixedRealityInputAction"/> pose to use for tracking the controller.
        /// </summary>
        public MixedRealityInputAction PoseAction => poseAction;

        [SerializeField]
        [Tooltip("The alternate pose action to use for certain controller variants.")]
        private MixedRealityInputAction alternatePoseAction;

        /// <summary>
        /// The alternate pose action to use for certain controller variants.
        /// </summary>
        public MixedRealityInputAction AlternatePoseAction => alternatePoseAction;
    }
}