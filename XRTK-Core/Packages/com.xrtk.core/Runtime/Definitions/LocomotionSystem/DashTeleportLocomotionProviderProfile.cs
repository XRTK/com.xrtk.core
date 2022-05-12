﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.DashTeleportLocomotionProvider"/>.
    /// </summary>
    public class DashTeleportLocomotionProviderProfile : BaseTeleportLocomotionProviderProfile
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
