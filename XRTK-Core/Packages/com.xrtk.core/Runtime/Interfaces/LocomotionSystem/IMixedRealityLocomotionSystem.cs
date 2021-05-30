// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
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
        /// Gets or sets whether the <see cref="IMixedRealityLocomotionSystem"/> should allow any locomotion
        /// of type <see cref="Services.LocomotionSystem.LocomotionType.Teleport"/>.
        /// Use this property e.g. to temporarily disable teleportation. Note that teleportation requires <see cref="LocomotionIsEnabled"/> to be set.
        /// </summary>
        /// <example>
        /// <see cref="TeleportationIsEnabled"/> = false;
        /// <see cref="TeleportationIsEnabled"/> = true;
        /// </example>
        bool TeleportationIsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="IMixedRealityLocomotionSystem"/> should allow locomotion of
        /// any <see cref="Services.LocomotionSystem.LocomotionType"/> to happen.
        /// Use this property e.g. to temporarily disable locomotion.
        /// </summary>
        /// <example>
        /// <see cref="LocomotionIsEnabled"/> = false;
        /// <see cref="LocomotionIsEnabled"/> = true;
        /// </example>
        bool LocomotionIsEnabled { get; set; }

        /// <summary>
        /// Gets a list of currently enabled <see cref="IMixedRealityLocomotionProvider"/>s.
        /// </summary>
        IReadOnlyCollection<IMixedRealityLocomotionProvider> EnabledLocomotionProviders { get; }

        /// <summary>
        /// Gets the <see cref="MixedRealityInputAction"/> used to trigger a teleport request.
        /// </summary>
        MixedRealityInputAction TeleportAction { get; }

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
