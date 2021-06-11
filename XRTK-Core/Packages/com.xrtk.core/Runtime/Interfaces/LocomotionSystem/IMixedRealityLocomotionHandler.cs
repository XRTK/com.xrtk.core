// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to implement for handling locomotion events by the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public interface IMixedRealityLocomotionHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a pointer requests a locomotion target, but no locomotion has started.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportRequest(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a locomotion has started.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportStarted(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a locomotion has successfully completed.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportCompleted(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a locomotion request has been canceled.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportCanceled(LocomotionEventData eventData);
    }
}
