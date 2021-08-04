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

        /// <summary>
        /// Adds a <see cref="ITeleportTargetProvider"/> to query for a teleportatoin target.
        /// </summary>
        /// <param name="teleportTargetProvider">The <see cref="ITeleportTargetProvider"/> answering a <see cref="IMixedRealityLocomotionSystemHandler.OnTeleportTargetRequested(Services.LocomotionSystem.LocomotionEventData)"/> request.</param>
        void AddTargetProvider(ITeleportTargetProvider teleportTargetProvider);

        /// <summary>
        /// Removes a <see cref="ITeleportTargetProvider"/>.
        /// </summary>
        /// <param name="teleportTargetProvider">The <see cref="ITeleportTargetProvider"/> to remove.</param>
        void RemoveTargetProvider(ITeleportTargetProvider teleportTargetProvider);
    }
}

