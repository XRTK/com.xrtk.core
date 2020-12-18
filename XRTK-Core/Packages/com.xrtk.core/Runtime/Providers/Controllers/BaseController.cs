// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;
using XRTK.Utilities;
using XRTK.Utilities.Gltf.Schema;
using XRTK.Utilities.Gltf.Serialization;
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
        protected BaseController() { }

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

            Name = $"{handednessPrefix}{GetType().Name}";

            if (controllerMappingProfile.IsNull())
            {
                throw new Exception($"{nameof(controllerMappingProfile)} cannot be null for {Name}");
            }

            visualizationProfile = controllerMappingProfile.VisualizationProfile;
            var pointers = AssignControllerMappings(controllerMappingProfile.InteractionMappingProfiles);

            // If no controller mappings found, warn the user.  Does not stop the project from running.
            if (Interactions == null || Interactions.Length < 1)
            {
                throw new Exception($"No Controller interaction mappings found for {controllerMappingProfile.name}!");
            }

            InputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource(Name, pointers);

            for (int i = 0; i < InputSource?.Pointers?.Length; i++)
            {
                InputSource.Pointers[i].Controller = this;
            }

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Enabled = true;
        }

        private readonly MixedRealityControllerVisualizationProfile visualizationProfile;

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

        /// <summary>
        /// Local offset from the controller position defining where the grip pose is.
        /// The grip pose may be used to attach things to the controller when grabbing objects.
        /// </summary>
        protected virtual MixedRealityPose GripPoseOffset => MixedRealityPose.ZeroIdentity;

        #region IMixedRealityController Implementation

        /// <inheritdoc />
        public string Name { get; }

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
                    var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, MixedRealityToolkit.CameraSystem?.MainCameraRig.PlayspaceTransform);
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
        public async void TryRenderControllerModel(byte[] glbData = null, bool useAlternatePoseAction = false) => await TryRenderControllerModelAsync(glbData, useAlternatePoseAction);

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="glbData">The raw binary glb data of the controller model, typically loaded from the driver.</param>
        /// <param name="useAlternatePoseAction">Should the visualizer be assigned the alternate pose actions?</param>
        public async Task TryRenderControllerModelAsync(byte[] glbData = null, bool useAlternatePoseAction = false)
        {
            if (visualizationProfile.IsNull())
            {
                Debug.LogWarning($"Missing {nameof(visualizationProfile)} for {GetType().Name}");
                return;
            }

            GltfObject gltfObject = null;
            GameObject controllerModel = null;

            // if we have model data from the platform and the controller has been configured to use the default model, attempt to load the controller model from glbData.
            if (glbData != null)
            {
                gltfObject = GltfUtility.GetGltfObjectFromGlb(glbData);
                await gltfObject.ConstructAsync();
                controllerModel = gltfObject.GameObjectReference;
            }

            // If we didn't get an override model, and we didn't load the driver model,
            // then get the global controller model for each hand.
            if (controllerModel.IsNull())
            {
                switch (ControllerHandedness)
                {
                    case Handedness.Left when !visualizationProfile.LeftHandModel.IsNull():
                        controllerModel = visualizationProfile.LeftHandModel;
                        break;
                    case Handedness.Right when !visualizationProfile.LeftHandModel.IsNull():
                        controllerModel = visualizationProfile.LeftHandModel;
                        break;
                }
            }

            // If we've got a controller model, then place it in the scene and get/attach the visualizer.
            if (!controllerModel.IsNull())
            {
                var playspaceTransform = MixedRealityToolkit.CameraSystem != null
                    ? MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform
                    : CameraCache.Main.transform.parent;

                // If the model was loaded from a system template
                if (gltfObject != null)
                {
                    controllerModel.name = $"{GetType().Name}_Visualization";
                    controllerModel.transform.SetParent(playspaceTransform);
                    var visualizationType = visualizationProfile.ControllerVisualizationType;
                    controllerModel.AddComponent(visualizationType.Type);
                    Visualizer = controllerModel.GetComponent<IMixedRealityControllerVisualizer>();
                }
                // If the model was a prefab
                else
                {
                    var controllerObject = Object.Instantiate(controllerModel, playspaceTransform) as GameObject;
                    Debug.Assert(controllerObject != null);
                    controllerObject.name = $"{GetType().Name}_Visualization";
                    Visualizer = controllerObject.GetComponent<IMixedRealityControllerVisualizer>();
                }

                // If a visualizer exists, set it up and bind it to the controller
                if (Visualizer != null)
                {
                    Visualizer.Controller = this;
                    SetupController(Visualizer);
                }
                else
                {
                    Debug.LogWarning($"Failed to attach a valid {nameof(IMixedRealityControllerVisualizer)} to {GetType().Name}");
                }
            }

            if (Visualizer == null)
            {
                Debug.LogError("Failed to render controller model!");
            }

            void SetupController(IMixedRealityControllerVisualizer visualizer)
            {
                if (useAlternatePoseAction)
                {
                    visualizer.UseSourcePoseData = visualizationProfile.AlternatePointerPose == MixedRealityInputAction.None;
                    visualizer.PoseAction = visualizationProfile.AlternatePointerPose;
                }
                else
                {
                    visualizer.UseSourcePoseData = visualizationProfile.PointerPose == MixedRealityInputAction.None;
                    visualizer.PoseAction = visualizationProfile.PointerPose;
                }
            }
        }
    }
}