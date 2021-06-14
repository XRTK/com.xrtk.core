// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to define teleport locomotion providers for the <see cref="IMixedRealityLocomotionSystem"/>.
    /// Teleport locomotion is defined as picking a target position and being translated
    /// to that target position once confirmed. This type of locomotion can coexist with <see cref="IFreeLocomotionProvider"/>s.
    /// However there can always be only one active provider for teleport locomotion at a time.
    /// </summary>
    public interface ITeleportLocomotionProvider : ILocomotionProvider
    {
        /// <summary>
        /// Is the provider currently teleporting?
        /// </summary>
        bool IsTeleporting { get; }
    }
}

