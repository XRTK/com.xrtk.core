// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityControllerProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

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
        private MixedRealityControllerVisualizationProfile visualizationProfile;

        public MixedRealityControllerVisualizationProfile VisualizationProfile
        {
            get => visualizationProfile;
            internal set => visualizationProfile = value;
        }

        [SerializeField]
        private MixedRealityPointerProfile[] pointerProfiles;

        /// <summary>
        /// The pointer profiles for this interaction if the interaction is 3 or 6 Dof
        /// </summary>
        public MixedRealityPointerProfile[] PointerProfiles
        {
            get => pointerProfiles;
            internal set => pointerProfiles = value;
        }
    }
}
