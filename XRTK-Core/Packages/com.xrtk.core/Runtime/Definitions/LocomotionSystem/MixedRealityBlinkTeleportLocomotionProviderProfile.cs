// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.BlinkTeleportLocomotionProvider"/>.
    /// </summary>
    public class MixedRealityBlinkTeleportLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Duration of the fade in / fade out in seconds.")]
        private float fadeDuration = .25f;

        /// <summary>
        /// Duration of the fade in / fade out in seconds.
        /// </summary>
        public float FadeDuration
        {
            get => fadeDuration;
            internal set => fadeDuration = value;
        }
    }
}
