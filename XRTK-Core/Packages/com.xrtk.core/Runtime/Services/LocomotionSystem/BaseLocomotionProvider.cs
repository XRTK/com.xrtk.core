// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Teleport;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Utilities;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for locomotion providers working with the <see cref="MixedRealityLocomotionSystem"/>.
    /// </summary>
    public abstract class BaseLocomotionProvider : MonoBehaviour, IMixedRealityTeleportHandler
    {
        private MixedRealityLocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected MixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>() as MixedRealityLocomotionSystem);

        /// <summary>
        /// Gets the player camera <see cref="Transform"/>.
        /// </summary>
        protected virtual Transform CameraTransform
        {
            get
            {
                return MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem)
                    ? cameraSystem.MainCameraRig.CameraTransform
                    : CameraCache.Main.transform;
            }
        }

        /// <summary>
        /// Gets the target <see cref="Transform"/> for locomotion.
        /// </summary>
        protected virtual Transform LocomotionTargetTransform
        {
            get
            {
                if (LocomotionSystem.LocomotionTargetOverride.IsNull() ||
                    !LocomotionSystem.LocomotionTargetOverride.enabled)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.Assert(!CameraTransform.parent.IsNull(), $"The {nameof(MixedRealityLocomotionSystem)} expects the camera to be parented under another transform!");
                    }

                    return CameraTransform.parent;
                }

                return LocomotionSystem.LocomotionTargetOverride.transform;
            }
        }

        /// <summary>
        /// This method is called just before any of the update methods is called the first time.
        /// </summary>
        protected virtual async void Start()
        {
            try
            {
                locomotionSystem = (await MixedRealityToolkit.GetSystemAsync<IMixedRealityLocomotionSystem>()) as MixedRealityLocomotionSystem;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return;
            }

            // We've been destroyed during the await.
            if (this == null) { return; }

            LocomotionSystem?.Register(gameObject);
        }

        /// <summary>
        /// This method is called when the behaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy()
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
