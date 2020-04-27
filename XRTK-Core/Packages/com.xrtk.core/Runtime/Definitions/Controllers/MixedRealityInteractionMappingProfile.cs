// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;

namespace XRTK.Definitions.Controllers
{
    public class MixedRealityInteractionMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityInteractionMapping interactionMapping = new MixedRealityInteractionMapping();

        public MixedRealityInteractionMapping InteractionMapping
        {
            get => interactionMapping;
            internal set => interactionMapping = value;
        }

        [SerializeField]
        private MixedRealityPointerProfile[] pointerProfiles = null;

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