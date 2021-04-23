// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Teleport;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for handling <see cref="IMixedRealityTeleportSystem"/> events in a
    /// <see cref="MonoBehaviour"/> component.
    /// </summary>
    public abstract class BaseTeleportProvider : MonoBehaviour, IMixedRealityTeleportProvider
    {
        private IMixedRealityLocomotionSystem locomotionSystem = null;

        protected IMixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>());

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
        public virtual void OnTeleportCanceled(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData) { }
    }
}
