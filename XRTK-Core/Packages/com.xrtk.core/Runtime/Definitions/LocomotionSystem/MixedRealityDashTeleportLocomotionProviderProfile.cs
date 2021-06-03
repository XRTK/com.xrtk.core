// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.MixedRealityDashTeleportLocomotionProvider"/>.
    /// </summary>
    public class MixedRealityDashTeleportLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Duration of the dash in seconds.")]
        private float dashDuration = .25f;

        /// <summary>
        /// Duration of the dash in seconds.
        /// </summary>
        public float DashDuration
        {
            get => dashDuration;
            internal set => dashDuration = value;
        }
    }
}
