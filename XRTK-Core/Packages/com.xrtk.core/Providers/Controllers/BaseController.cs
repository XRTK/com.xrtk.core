// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;
using Object = UnityEngine.Object;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base Controller class to inherit from for all controllers.
    /// </summary>
    public abstract class BaseController : IMixedRealityController
    {
        /// <summary>
        /// Creates a new instance of a controller.
        /// </summary>
        /// <param name="controllerDataProvider">The <see cref="IMixedRealityControllerDataProvider"/> this controller belongs to.</param>
        /// <param name="trackingState">The initial tracking state of this controller.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="controllerMappingProfile"></param>
        protected BaseController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
        {
            ControllerDataProvider = controllerDataProvider;
            TrackingState = trackingState;
            ControllerHandedness = controllerHandedness;

            var handednessPrefix = string.Empty;

            if (controllerHandedness == Handedness.Left ||
                controllerHandedness == Handedness.Right)
            {
                handednessPrefix = $"{controllerHandedness} ";
            }

            var pointers = AssignControllerMappings(controllerMappingProfile.InteractionMappingProfiles);

            // If no controller mappings found, warn the user.  Does not stop the project from running.
            if (Interactions == null || Interactions.Length < 1)
            {
                throw new Exception($"No Controller interaction mappings found for {controllerMappingProfile.name}!");
            }

            InputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource($"{handednessPrefix}{GetType().Name}", pointers);

            for (int i = 0; i < InputSource?.Pointers?.Length; i++)
            {
                InputSource.Pointers[i].Controller = this;
            }

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Enabled = true;
        }

        /// <summary>
        /// The default interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultInteractions { get; } = new MixedRealityInteractionMapping[0];

        /// <summary>
        /// The Default Left Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions { get; } = new MixedRealityInteractionMapping[0];

        /// <summary>
        /// The Default Right Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions { get; } = new MixedRealityInteractionMapping[0];

        #region IMixedRealityController Implementation

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public IMixedRealityControllerDataProvider ControllerDataProvider { get; }

        /// <inheritdoc />
        public TrackingState TrackingState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; private set; }

        /// <inheritdoc />
        public IMixedRealityControllerVisualizer Visualizer { get; private set; }

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;

        /// <inheritdoc />
        public Vector3 AngularVelocity { get; protected set; } = Vector3.zero;

        /// <inheritdoc />
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        #endregion IMixedRealityController Implementation

        /// <summary>
        /// Updates the current readings for the controller.
        /// </summary>
        public virtual void UpdateController() { }

        /// <summary>
        /// Assign the default interactions based on controller handedness if necessary.
        /// </summary>
        /// <param name="controllerHandedness">The handedness of the controller.</param>
        public abstract void SetupDefaultInteractions(Handedness controllerHandedness);

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        protected void AssignControllerMappings(MixedRealityInteractionMapping[] mappings) => Interactions = mappings;

        private IMixedRealityPointer[] AssignControllerMappings(MixedRealityInteractionMappingProfile[] interactionMappingProfiles)
        {
            var pointers = new List<IMixedRealityPointer>();
            var interactions = new MixedRealityInteractionMapping[interactionMappingProfiles.Length];

            for (int i = 0; i < interactions.Length; i++)
            {
                var interactionProfile = interactionMappingProfiles[i];

                for (int j = 0; j < interactionProfile.PointerProfiles.Length; j++)
                {
                    var pointerProfile = interactionProfile.PointerProfiles[j];
                    var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform);
                    var pointer = pointerObject.GetComponent<IMixedRealityPointer>();

                    if (pointer != null)
                    {
                        pointers.Add(pointer);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to attach {pointerProfile.PointerPrefab.name} to {GetType().Name} {ControllerHandedness}.");
                    }
                }

                interactions[i] = interactionProfile.InteractionMapping;
            }

            AssignControllerMappings(interactions);

            return pointers.Count > 0 ? pointers.ToArray() : null;
        }

        /// <inheritdoc />
        public async void TryRenderControllerModel(Type controllerType, byte[] glbData = null) => await TryRenderControllerModelAsync(controllerType, glbData);

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="useAlternatePoseAction">Should the visualizer be assigned the alternate pose actions?</param>
        /// <param name="glbData">The raw binary glb data of the controller model, typically loaded from the driver.</param>
        internal async void TryRenderControllerModel(Type controllerType, bool useAlternatePoseAction, byte[] glbData = null) => await TryRenderControllerModelAsync(controllerType, glbData, useAlternatePoseAction);

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="glbData">The raw binary glb data of the controller model, typically loaded from the driver.</param>
        /// <param name="useAlternatePoseAction">Should the visualizer be assigned the alternate pose actions?</param>
        /// <returns>True, if controller model is being properly rendered.</returns>
        /// <remarks>
        /// (Given a user can, have no system default and override specific controller types with a system default, OR, enable a system system default but override that default for specific controllers)
        /// Flow is as follows:
        /// 1. Check if either there is a global setting for an system override and if there is a specific customization for that controller type
        /// 2. If either the there is a system data and either the
        ///
        /// </remarks>
#pragma warning disable 1998
        internal async Task TryRenderControllerModelAsync(Type controllerType, byte[] glbData = null, bool useAlternatePoseAction = false)
#pragma warning restore 1998
        {
            if (controllerType == null)
            {
                Debug.LogError("Unknown type of controller, cannot render");
                return;
            }

            //var visualizationProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile;

            //if (visualizationProfile == null)
            //{
            //    Debug.LogError("Missing ControllerVisualizationProfile!");
            //    return;
            //}

            //if (!visualizationProfile.RenderMotionControllers) { return; }

            //GltfObject gltfObject = null;

            //// If a specific controller template exists, check if it wants to override the global model, or use the system default specifically (in case global default is not used)
            //bool useSystemDefaultModels = visualizationProfile.GetControllerModelOverride(controllerType, ControllerHandedness, out var controllerModel);

            //// If an override is not configured for defaults and has no model, then use the system default check
            //if (!useSystemDefaultModels && controllerModel == null)
            //{
            //    useSystemDefaultModels = visualizationProfile.UseDefaultModels;
            //}

            //// if we have model data from the platform and the controller has been configured to use the default model, attempt to load the controller model from glbData.
            //if (glbData != null && useSystemDefaultModels)
            //{
            //    gltfObject = GltfUtility.GetGltfObjectFromGlb(glbData);
            //    await gltfObject.ConstructAsync();
            //    controllerModel = gltfObject.GameObjectReference;
            //}

            //// If we didn't get an override model, and we didn't load the driver model,
            //// then get the global controller model for each hand.
            //if (controllerModel == null)
            //{
            //    switch (ControllerHandedness)
            //    {
            //        case Handedness.Left when visualizationProfile.GlobalLeftHandModel != null:
            //            controllerModel = visualizationProfile.GlobalLeftHandModel;
            //            break;
            //        case Handedness.Right when visualizationProfile.GlobalRightHandModel != null:
            //            controllerModel = visualizationProfile.GlobalRightHandModel;
            //            break;
            //    }
            //}

            //// If we've got a controller model, then place it in the scene and get/attach the visualizer.
            //if (controllerModel != null)
            //{
            //    //If the model was loaded from a system template
            //    if (useSystemDefaultModels && gltfObject != null)
            //    {
            //        controllerModel.name = $"{controllerType.Name}_Visualization";
            //        controllerModel.transform.SetParent(MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform);
            //        var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(controllerType, ControllerHandedness) ??
            //                                visualizationProfile.ControllerVisualizationType;
            //        controllerModel.AddComponent(visualizationType.Type);
            //        Visualizer = controllerModel.GetComponent<IMixedRealityControllerVisualizer>();
            //    }
            //    //If the model was a prefab
            //    else
            //    {
            //        var controllerObject = UnityEngine.Object.Instantiate(controllerModel, MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform);
            //        controllerObject.name = $"{controllerType.Name}_Visualization";
            //        Visualizer = controllerObject.GetComponent<IMixedRealityControllerVisualizer>();
            //    }

            //    //If a visualizer exists, set it up and bind it to the controller
            //    if (Visualizer != null)
            //    {
            //        Visualizer.Controller = this;
            //        SetupController(Visualizer);
            //    }
            //    else
            //    {
            //        Debug.LogWarning($"Failed to attach a valid {nameof(IMixedRealityControllerVisualizer)} to {controllerType.Name}");
            //    }
            //}

            //if (Visualizer == null)
            //{
            //    Debug.LogError("Failed to render controller model!");
            //}

            //void SetupController(IMixedRealityControllerVisualizer visualizer)
            //{
            //    if (!useAlternatePoseAction &&
            //        visualizationProfile.TryGetControllerPose(controllerType, ControllerHandedness, out MixedRealityInputAction poseAction))
            //    {
            //        visualizer.UseSourcePoseData = false;
            //        visualizer.PoseAction = poseAction;
            //    }
            //    else if (useAlternatePoseAction &&
            //             visualizationProfile.TryGetControllerPoseOverride(controllerType, ControllerHandedness, out MixedRealityInputAction altPoseAction))
            //    {
            //        visualizer.UseSourcePoseData = false;
            //        visualizer.PoseAction = altPoseAction;
            //    }
            //    else if (visualizationProfile.GlobalPointerPose != MixedRealityInputAction.None)
            //    {
            //        visualizer.UseSourcePoseData = false;
            //        visualizer.PoseAction = visualizationProfile.GlobalPointerPose;
            //    }
            //    else
            //    {
            //        Debug.LogError("Failed to get pose actions for controller visual.");
            //    }
            //}
        }
    }
}