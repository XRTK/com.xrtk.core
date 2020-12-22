// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Teleport;

namespace XRTK.Interfaces.TeleportSystem.Handlers
{
    /// <summary>
    /// Interface to implement for handling teleport events by the <see cref="IMixedRealityTeleportSystem"/>.
    /// </summary>
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a pointer requests a teleport target, but no teleport has begun.
        /// </summary>
        /// <param name="eventData">Teleport event data.</param>
        void OnTeleportRequest(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has started.
        /// </summary>
        /// <param name="eventData">Teleport event data.</param>
        void OnTeleportStarted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has successfully completed.
        /// </summary>
        /// <param name="eventData">Teleport event data.</param>
        void OnTeleportCompleted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport request has been canceled.
        /// </summary>
        /// <param name="eventData">Teleport event data.</param>
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
