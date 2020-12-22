// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.TeleportSystem;
using XRTK.Interfaces.TeleportSystem.Handlers;
using XRTK.Services.Teleportation;

namespace XRTK.Definitions.TeleportSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="MixedRealityTeleportSystem"/>.
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

        [SerializeField]
        [Tooltip("Input action to trigger a teleport request.")]
        private MixedRealityInputAction teleportAction = MixedRealityInputAction.None;

        /// <summary>
        /// Input action to trigger a teleport request.
        /// </summary>
        public MixedRealityInputAction TeleportAction
        {
            get => teleportAction;
            internal set => teleportAction = value;
        }
    }
}
