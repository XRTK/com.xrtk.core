// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.Events;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// System interface for a locomotion system in the Mixed Reality Toolkit.
    /// All replacement systems for providing locomotion functionality should derive from this interface.
    /// </summary>
    public interface IMixedRealityLocomotionSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Gets or sets whether the <see cref="IMixedRealityLocomotionSystem"/> should allow any teleportation
        /// requested using <see cref="RaiseTeleportRequest(IMixedRealityPointer, IMixedRealityTeleportHotSpot)"/>.
        /// Use this property to temporarily disable teleportation. Note that teleportation requires locomotion to be enabled.
        /// </summary>
        /// <example>
        /// TeleportationEnabled = false; // Disables teleportation.
        /// TeleportationEnabled = true; // Enables teleportation.
        /// </example>
        bool TeleportationEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="IMixedRealityLocomotionSystem"/> should allow any locomotion.
        /// Use this property to temporarily disable locomotion. Note that disabling locomotion will also
        /// disable teleportation.
        /// </summary>
        /// <example>
        /// LocomotionEnabled = false; // Disables locomotion.
        /// LocomotionEnabled = true; // Enables locomotion.
        /// </example>
        bool LocomotionEnabled { get; set; }

        /// <summary>
        /// Gets the <see cref="MixedRealityInputAction"/> used to trigger a teleport request.
        /// </summary>
        MixedRealityInputAction TeleportAction { get; }

        /// <summary>
        /// Gets the <see cref="MixedRealityInputAction"/> used to cancel a teleport request.
        /// </summary>
        MixedRealityInputAction CancelTeleportAction { get; }

        /// <summary>
        /// Raise a teleportation request event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation started event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation canceled event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);
    }
}
