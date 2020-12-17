// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.TeleportSystem;

namespace XRTK.Definitions.TeleportSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Services.Teleportation.MixedRealityTeleportSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Teleport System Profile", fileName = "MixedRealityTeleportSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityTeleportSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityTeleportDataProvider>
    {
        [SerializeField]
        [Tooltip("The duration of the teleport in seconds.")]
        private float teleportDuration = .25f;

        /// <summary>
        /// The duration of the teleport in seconds.
        /// </summary>
        public float TeleportDuration => teleportDuration;
    }
}