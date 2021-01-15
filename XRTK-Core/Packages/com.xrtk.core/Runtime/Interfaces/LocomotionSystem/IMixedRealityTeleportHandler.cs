// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Teleport;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to implement for handling teleport events by the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a pointer requests a teleport target, but no teleport has started.
        /// </summary>
        /// <param name="eventData"><see cref="TeleportEventData"/> provided.</param>
        void OnTeleportRequest(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has started.
        /// </summary>
        /// <param name="eventData"><see cref="TeleportEventData"/> provided.</param>
        void OnTeleportStarted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has successfully completed.
        /// </summary>
        /// <param name="eventData"><see cref="TeleportEventData"/> provided.</param>
        void OnTeleportCompleted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport request has been canceled.
        /// </summary>
        /// <param name="eventData"><see cref="TeleportEventData"/> provided.</param>
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
