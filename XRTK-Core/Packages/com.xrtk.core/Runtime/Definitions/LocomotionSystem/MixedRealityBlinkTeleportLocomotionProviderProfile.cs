// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.BlinkTeleportLocomotionProvider"/>.
    /// </summary>
    public class MixedRealityBlinkTeleportLocomotionProviderProfile : BaseTeleportLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Input action to perform locomotion the player.")]
        [AxisConstraint(Utilities.AxisType.SingleAxis | Utilities.AxisType.Digital | Utilities.AxisType.DualAxis)]
        private MixedRealityInputAction inputAction = MixedRealityInputAction.None;

        /// <summary>
        /// Gets the input action to perform locomotion the player.
        /// </summary>
        public MixedRealityInputAction InputAction
        {
            get => inputAction;
            internal set => inputAction = value;
        }

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
