// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.MixedRealitySmoothLocomotionProvider"/>.
    /// </summary>
    public class MixedRealitySmoothLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Speed in meters per second for movement.")]
        [Range(1f, 100f)]
        private float speed = 5f;

        /// <summary>
        /// Speed in meters per second for movement.
        /// </summary>
        public float Speed
        {
            get => speed;
            internal set => speed = value;
        }
    }
}
