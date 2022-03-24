﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
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
        private bool useCustomInteractions = true;

        internal bool UseCustomInteractions
        {
            get => useCustomInteractions;
            set => useCustomInteractions = value;
        }

        [SerializeField]
        private MixedRealityInteractionMappingProfile[] interactionMappingProfiles = new MixedRealityInteractionMappingProfile[0];

        /// <summary>
        /// Details the list of available interaction profiles available for the controller.
        /// </summary>
        public MixedRealityInteractionMappingProfile[] InteractionMappingProfiles
        {
            get => interactionMappingProfiles;
            internal set => interactionMappingProfiles = value;
        }
    }
}