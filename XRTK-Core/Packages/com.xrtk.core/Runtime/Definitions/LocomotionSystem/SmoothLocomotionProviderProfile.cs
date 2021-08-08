// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.SmoothLocomotionProvider"/>.
    /// </summary>
    public class SmoothLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Input action to perform locomotion the player.")]
        [AxisConstraint(Utilities.AxisType.DualAxis)]
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
