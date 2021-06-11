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
        /// Raised when a <see cref="IMixedRealityTeleportLocomotionProvider"/> requests a
        /// target location for teleport, but no teleportation has started.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportTargetRequested(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a <see cref="IMixedRealityTeleportLocomotionProvider"/> has started
        /// teleportation to a target location.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportStarted(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a <see cref="IMixedRealityTeleportLocomotionProvider"/> has successfully
        /// completed teleportation.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportCompleted(LocomotionEventData eventData);

        /// <summary>
        /// Raised when a <see cref="IMixedRealityTeleportLocomotionProvider"/> has canceled
        /// teleportation.
        /// </summary>
        /// <param name="eventData"><see cref="LocomotionEventData"/> provided.</param>
        void OnTeleportCanceled(LocomotionEventData eventData);
    }
}
