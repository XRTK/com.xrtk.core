// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Services;
using XRTK.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.EventDatum.Input;
using XRTK.Services.LocomotionSystem;
using XRTK.Interfaces.CameraSystem;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Definitions.Utilities;
using XRTK.Definitions.InputSystem;

namespace XRTK.Providers.LocomotionSystem
{
    public abstract class BaseLocomotionProvider : BaseDataProvider, ILocomotionProvider
    {
        /// <inheritdoc />
        public BaseLocomotionProvider(string name, uint priority, BaseLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            startupBehaviour = profile.StartupBehaviour;
        }

        private readonly AutoStartBehavior startupBehaviour;
        private bool isInitialized;

        /// <inheritdoc />
        public MixedRealityInputAction InputAction { get; protected set; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets the active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected virtual MixedRealityLocomotionSystem LocomotionSystem => (MixedRealityLocomotionSystem)ParentService;

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

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (IsEnabled)
            {
                return;
            }

            if (startupBehaviour == AutoStartBehavior.AutoStart || isInitialized)
            {
                IsEnabled = true;
                LocomotionSystem.OnLocomotionProviderEnabled(this);

                Debug.Log($"Enabled {GetType().Name}");
            }
            else
            {
                Disable();
            }

            isInitialized = true;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!IsEnabled)
            {
                return;
            }

            IsEnabled = false;
            LocomotionSystem.OnLocomotionProviderDisabled(this);

            Debug.Log($"Disabled {GetType().Name}");
        }

        /// <inheritdoc />
        public virtual void OnTeleportTargetRequested(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(LocomotionEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputChanged(InputEventData<Vector2> eventData) { }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData) { }
    }
}
