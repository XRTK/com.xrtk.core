// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
        /// Gets a list of currently enabled <see cref="IMixedRealityLocomotionProvider"/>s.
        /// </summary>
        IReadOnlyList<IMixedRealityLocomotionProvider> EnabledLocomotionProviders { get; }

        /// <summary>
        /// Gets the <see cref="MixedRealityInputAction"/> used to trigger a teleport request.
        /// </summary>
        MixedRealityInputAction TeleportAction { get; }

        void EnableLocomotionProvider<T>() where T : IMixedRealityLocomotionProvider;

        void EnableLocomotionProvider(Type locomotionProviderType);

        void DisableLocomotionProvider<T>() where T : IMixedRealityLocomotionProvider;

        void DisableLocomotionProvider(Type locomotionProviderType);

        /// <summary>
        /// A <see cref="IMixedRealityLocomotionProvider"/> was enabled.
        /// </summary>
        /// <param name="locomotionProvider">The enabled <see cref="IMixedRealityLocomotionProvider"/>.</param>
        void OnLocomotionProviderEnabled(IMixedRealityLocomotionProvider locomotionProvider);

        /// <summary>
        /// A <see cref="IMixedRealityLocomotionProvider"/> was disabled.
        /// </summary>
        /// <param name="locomotionProvider">The disabled <see cref="IMixedRealityLocomotionProvider"/>.</param>
        void OnLocomotionProviderDisabled(IMixedRealityLocomotionProvider locomotionProvider);

        /// <summary>
        /// Raise a teleportation request event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation started event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation canceled event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);
    }
}
