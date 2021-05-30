// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Defines possible locomotion types supported by the <see cref="MixedRealityLocomotionSystem"/>
    /// and its providers.
    /// </summary>
    public enum LocomotionType
    {
        /// <summary>
        /// Teleport locomotion is defined as picking a target position and being translated
        /// to that target position once confirmed. This type of locomotion can coexist with <see cref="Free"/> locomotion.
        /// However there can always be only one active provider for teleport locomotion at a time.
        /// </summary>
        Teleport = 0,
        /// <summary>
        /// Free locomotion is defined as movement in a specific direction in 3D space, which the user can freely choose.
        /// This type of locomotion can coexist with <see cref="Teleport"/> locomotion. However there can always be only one
        /// active provider for free locomotion at a time.
        /// </summary>
        Free,
        /// <summary>
        /// With on rails locomotion the user has very limited to none control over their movement. Instead the player rig
        /// moves on predefined pathways (rails). Either automatically or by input control. On rails locomotion is exclusive,
        /// that means when any on rails locomotion provider is active and enabled, all other types of locomotion are not available.
        /// There can always be only one active provider for on rails locomotion at a time.
        /// </summary>
        OnRails,
        /// <summary>
        /// An undefined or very specific type of locomotion. This value is reserved for edge cases.
        /// This type of locomotion is exclusive, when any locomotion provider implementing it is active,
        /// all other types of locomotion are not available.
        /// </summary>
        Other
    }
}
