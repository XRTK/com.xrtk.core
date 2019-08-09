// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Visualization Profile", fileName = "MixedRealityControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    public class MixedRealityControllerVisualizationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Profile for configuring hands visualization.")]
        private MixedRealityHandControllerVisualizationProfile handVisualizationProfile;

        /// <summary>
        /// Profile for configuring hands visualization.
        /// </summary>
        public MixedRealityHandControllerVisualizationProfile HandVisualizationProfile
        {
            get { return handVisualizationProfile; }
            private set { handVisualizationProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable and configure the controller rendering of the Motion Controllers on Startup.")]
        private bool renderMotionControllers = false;

        /// <summary>
        /// Enable and configure the controller rendering of the Motion Controllers on Startup.
        /// </summary>
        public bool RenderMotionControllers
        {
            get => renderMotionControllers;
            private set => renderMotionControllers = value;
        }

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
        [Tooltip("Default Left Controller Model.\nNote: If an override profile model is not found for a specific controller, the fallback is the global model for the specific hand.")]
        private GameObject globalLeftHandModel;

        /// <summary>
        /// The Default controller model, for when there is no specific controller model for the Left hand or when no hand is specified (Handedness = none)
        /// </summary>
        /// <remarks>
        /// If the default model for the left hand controller can not be found, the controller will fall back and use this setting for visualization.
        /// </remarks>
        public GameObject GlobalLeftHandModel
        {
            get => globalLeftHandModel;
            private set => globalLeftHandModel = value;
        }

        [SerializeField]
        [Tooltip("Default Right Controller Model.\nNote: If an override profile model is not found for a specific controller, the fallback is the global model for the specific hand.")]
        private GameObject globalRightHandModel;

        /// <summary>
        /// The Default controller pose action, for when there is no specific pose for specific controller / hand combination.
        /// </summary>
        /// <remarks>
        /// If the default model for the right hand controller can not be found, the controller will fall back and use this setting for visualization.
        /// </remarks>
        public GameObject GlobalRightHandModel
        {
            get => globalRightHandModel;
            private set => globalRightHandModel = value;
        }
        
        [SerializeField]
        [Tooltip("Default Pose Action.\nNote: If an override profile is not found for a specific controller, the default pose action is used.")]
        private MixedRealityInputAction globalPointerPose;

        /// <summary>
        /// The Default controller model, for when there is no specific controller model for the Left hand or when no hand is specified (Handedness = none)
        /// </summary>
        /// <remarks>
        /// If the pointer pose action for the specific controller can not be found, the controller will fall back and use this setting for visualization.
        /// </remarks>        
        /// <remarks>
        /// If the default model for the right hand controller can not be found, the controller will fall back and use this setting for visualization.
        /// </remarks>
        public MixedRealityInputAction GlobalPointerPose
        {
            get => globalPointerPose;
            private set => globalPointerPose = value;
        }

        [SerializeField]
        private MixedRealityControllerVisualizationSetting[] controllerVisualizationSettings = new MixedRealityControllerVisualizationSetting[0];

        /// <summary>
        /// The current list of controller visualization settings.
        /// </summary>
        public MixedRealityControllerVisualizationSetting[] ControllerVisualizationSettings => controllerVisualizationSettings;

        /// <summary>
        /// Gets the override model for a specific controller type and hand
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        /// <param name="overrideModel">Outputs the supplied model for an overridden visualization (if available), can return null </param>
        /// <returns>Returns whether the override visualization should use the System model (if available) or not</returns>
        public bool GetControllerModelOverride(Type controllerType, Handedness hand, out GameObject overrideModel)
        {
            overrideModel = null;

            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (controllerVisualizationSettings[i].ControllerType != null &&
                    controllerVisualizationSettings[i].ControllerType.Type == controllerType &&
                    (controllerVisualizationSettings[i].Handedness == hand || controllerVisualizationSettings[i].Handedness == Handedness.Both))
                {
                    overrideModel = controllerVisualizationSettings[i].OverrideControllerModel;
                    return controllerVisualizationSettings[i].UseDefaultModel;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the <see cref="MixedRealityInputAction"/> pose action for this controller to use when synchronizing the position of the controller's visualization.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for.</param>
        /// <param name="hand">The specific hand assigned to the controller.</param>
        /// <param name="poseAction">The <see cref="MixedRealityInputAction"/> to use for tracking pose data.</param>
        /// <returns>True, if the controller should use the pose action, otherwise false.</returns>
        public bool TryGetControllerPose(Type controllerType, Handedness hand, out MixedRealityInputAction poseAction)
        {
            poseAction = MixedRealityInputAction.None;

            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (controllerVisualizationSettings[i].ControllerType != null &&
                    controllerVisualizationSettings[i].ControllerType.Type == controllerType &&
                    (controllerVisualizationSettings[i].Handedness == hand || controllerVisualizationSettings[i].Handedness == Handedness.Both) &&
                    controllerVisualizationSettings[i].PoseAction != MixedRealityInputAction.None)
                {
                    poseAction = controllerVisualizationSettings[i].PoseAction;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the alternate <see cref="MixedRealityInputAction"/> pose action for this controller variant.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        /// <param name="alternatePoseAction">The alternate <see cref="MixedRealityInputAction"/> to use</param>
        /// <returns>True, if a valid alternative is found.</returns>
        public bool TryGetControllerPoseOverride(Type controllerType, Handedness hand, out MixedRealityInputAction alternatePoseAction)
        {
            alternatePoseAction = MixedRealityInputAction.None;

            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (controllerVisualizationSettings[i].ControllerType != null &&
                    controllerVisualizationSettings[i].ControllerType.Type == controllerType &&
                    (controllerVisualizationSettings[i].Handedness == hand || controllerVisualizationSettings[i].Handedness == Handedness.Both) &&
                    controllerVisualizationSettings[i].AlternatePoseAction != MixedRealityInputAction.None)
                {
                    alternatePoseAction = controllerVisualizationSettings[i].AlternatePoseAction;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the override <see cref="IMixedRealityControllerVisualizer"/> type for a specific controller type and hand.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public SystemType GetControllerVisualizationTypeOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (controllerVisualizationSettings[i].ControllerType != null &&
                    controllerVisualizationSettings[i].ControllerType.Type == controllerType &&
                    (controllerVisualizationSettings[i].Handedness == hand || controllerVisualizationSettings[i].Handedness == Handedness.Both))
                {
                    return controllerVisualizationSettings[i].ControllerVisualizationType;
                }
            }

            return null;
        }
    }
}