// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.TeleportSystem;
using XRTK.Interfaces.TeleportSystem.Handlers;

namespace XRTK.Definitions.TeleportSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Services.Teleportation.MixedRealityTeleportSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Teleport System Profile", fileName = "MixedRealityTeleportSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityTeleportSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityTeleportDataProvider>
    {
        [SerializeField]
        [Tooltip("The concrete teleport provider to use for teleportation.")]
        [Implements(typeof(IMixedRealityTeleportProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType teleportProvider;

        /// <summary>
        /// The concrete teleport provider to use for teleportation.
        /// </summary>
        public SystemType TeleportProvider
        {
            get => teleportProvider;
            internal set => teleportProvider = value;
        }
    }
}
