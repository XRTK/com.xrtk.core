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
        [Implements(typeof(IMixedRealityTeleportComponentHandler), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete teleport handler component to use for teleportation.")]
        private SystemType teleportHandlerComponent = null;

        /// <summary>
        /// The concrete teleport handler component to use for teleportation.
        /// </summary>
        public SystemType TeleportHandlerComponent => teleportHandlerComponent;
    }
}