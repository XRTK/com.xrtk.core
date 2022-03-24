// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.BlinkTeleportLocomotionProvider"/>.
    /// </summary>
    public class BlinkTeleportLocomotionProviderProfile : BaseTeleportLocomotionProviderProfile
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

        [SerializeField]
        [Tooltip("The material used to simulate a blink.")]
        private Material fadeMaterial = null;

        /// <summary>
        /// The material used to simulate a blink.
        /// </summary>
        public Material FadeMaterial
        {
            get => fadeMaterial;
            internal set => fadeMaterial = value;
        }

        [SerializeField]
        [Tooltip("The color applied gradually when fading back in after a teleport.")]
        private Color fadeInColor = Color.clear;

        /// <summary>
        /// The color applied gradually when fading back in after a teleport.
        /// </summary>
        public Color FadeInColor
        {
            get => fadeInColor;
            internal set => fadeInColor = value;
        }

        [SerializeField]
        [Tooltip("The color applied gradually when fading out before a teleport.")]
        private Color fadeOutColor = Color.black;

        /// <summary>
        /// The color applied gradually when fading out before a teleport.
        /// </summary>
        public Color FadeOutColor
        {
            get => fadeOutColor;
            internal set => fadeOutColor = value;
        }
    }
}
