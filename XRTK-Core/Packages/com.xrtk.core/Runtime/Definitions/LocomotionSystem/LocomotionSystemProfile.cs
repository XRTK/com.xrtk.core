// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile settings for <see cref="Services.LocomotionSystem.LocomotionSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Locomotion System Profile", fileName = "MixedRealityLocomotionSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class LocomotionSystemProfile : BaseMixedRealityServiceProfile<ILocomotionSystemDataProvider>
    {
        [SerializeField]
        [Tooltip("The teleportation cooldown defines the time that needs to pass after a successful teleportation for another one to be possible.")]
        [Range(0, 10f)]
        private float teleportCooldown = 1f;

        /// <summary>
        /// The teleportation cooldown defines the time that needs to pass after a successful teleportation for another one to be possible.
        /// </summary>
        public float TeleportCooldown
        {
            get => teleportCooldown;
            internal set => teleportCooldown = value;
        }
    }
}
