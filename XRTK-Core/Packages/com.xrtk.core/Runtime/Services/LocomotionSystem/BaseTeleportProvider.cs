// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.EventDatum.Teleport;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for teleport providers working with the <see cref="MixedRealityLocomotionSystem"/>.
    /// Teleport providers perform the actual teleportation when requested.
    /// </summary>
    public abstract class BaseTeleportProvider : BaseLocomotionProvider, IMixedRealityTeleportProvider
    {
        /// <summary>
        /// This method is called when the behaviour becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            LocomotionSystem?.Register(gameObject);
        }

        /// <summary>
        /// This method is called when the behaviour becomes disabled and inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            LocomotionSystem?.Unregister(gameObject);
        }

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(TeleportEventData eventData) { }
    }
}
