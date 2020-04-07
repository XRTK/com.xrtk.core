// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private SystemType controllerType = null;

        /// <summary>
        /// The type of controller this mapping corresponds to.
        /// </summary>
        public SystemType ControllerType
        {
            get => controllerType;
            internal set => controllerType = value;
        }

        [SerializeField]
        private Handedness handedness = Handedness.None;

        public Handedness Handedness
        {
            get => handedness;
            internal set => handedness = value;
        }

        [SerializeField]
        private MixedRealityControllerVisualizationProfile visualizationProfile = null;

        public MixedRealityControllerVisualizationProfile VisualizationProfile
        {
            get => visualizationProfile;
            internal set => visualizationProfile = value;
        }

        [SerializeField]
        [FormerlySerializedAs("useCustomInteractionMappings")]
        [Tooltip("Override the default interaction mappings.")]
        private bool useCustomInteractions = false;

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        public bool UseCustomInteractions
        {
            get => useCustomInteractions;
            internal set => useCustomInteractions = value;
        }

        [SerializeField]
        [FormerlySerializedAs("interactions")]
        [Tooltip("Details the list of available buttons / interactions available from the device.")]
        private MixedRealityInteractionMappingProfile[] interactionMappings = new MixedRealityInteractionMappingProfile[0];

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        public MixedRealityInteractionMappingProfile[] InteractionMappings
        {
            get => interactionMappings;
            internal set => interactionMappings = value;
        }

        internal void SetDefaultInteractionMapping(bool overwrite = false)
        {
            if (Activator.CreateInstance(controllerType, TrackingState.NotTracked, handedness, null, null) is BaseController detectedController &&
                (interactionMappings == null || interactionMappings.Length == 0 || overwrite))
            {
                switch (handedness)
                {
                    case Handedness.Left:
                        interactionMappings = CreateDefaultMappingProfiles(detectedController.DefaultLeftHandedInteractions);
                        break;
                    case Handedness.Right:
                        interactionMappings = CreateDefaultMappingProfiles(detectedController.DefaultRightHandedInteractions);
                        break;
                    default:
                        interactionMappings = CreateDefaultMappingProfiles(detectedController.DefaultInteractions);
                        break;
                }
            }
        }

        private MixedRealityInteractionMappingProfile[] CreateDefaultMappingProfiles(MixedRealityInteractionMapping[] defaultMappings)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Synchronizes the Input Actions of the same physical controller of a different concrete type.
        /// </summary>
        /// <param name="otherControllerMapping"></param>
        internal void SynchronizeInputActions(MixedRealityInteractionMappingProfile[] otherControllerMapping)
        {
            if (otherControllerMapping.Length != interactionMappings.Length)
            {
                Debug.LogError("Controller Input Actions must be the same length!");
                return;
            }

            for (int i = 0; i < interactionMappings.Length; i++)
            {
                interactionMappings[i].InteractionMapping.MixedRealityInputAction = otherControllerMapping[i].InteractionMapping.MixedRealityInputAction;
            }
        }
    }
}