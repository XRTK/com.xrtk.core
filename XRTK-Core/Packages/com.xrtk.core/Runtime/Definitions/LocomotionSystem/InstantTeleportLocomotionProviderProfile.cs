// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.LocomotionSystem.InstantTeleportLocomotionProvider"/>.
    /// </summary>
    public class InstantTeleportLocomotionProviderProfile : BaseLocomotionProviderProfile
    {
        [SerializeField]
        [Tooltip("Input action to perform locomotion the player.")]
        [AxisConstraint(Utilities.AxisType.Digital)]
        private MixedRealityInputAction inputAction = MixedRealityInputAction.None;

        /// <summary>
        /// Gets the input action to perform locomotion the player.
        /// </summary>
        public MixedRealityInputAction InputAction
        {
            get => inputAction;
            internal set => inputAction = value;
        }
    }
}
