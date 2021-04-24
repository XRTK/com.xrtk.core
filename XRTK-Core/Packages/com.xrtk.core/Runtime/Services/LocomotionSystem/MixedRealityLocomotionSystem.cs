// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Teleport;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Utilities;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityLocomotionSystem"/>
    /// </summary>
    [System.Runtime.InteropServices.Guid("9453c088-285e-47aa-bfbb-dafd9109fdd5")]
    public class MixedRealityLocomotionSystem : BaseEventSystem, IMixedRealityLocomotionSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The active <see cref="MixedRealityLocomotionSystemProfile"/>.</param>
        public MixedRealityLocomotionSystem(MixedRealityLocomotionSystemProfile profile)
            : base(profile)
        {
            teleportProvider = profile.TeleportProvider?.Type == null ? null : teleportProvider = profile.TeleportProvider;
            TeleportAction = profile.TeleportAction;
            CancelTeleportAction = profile.CancelTeleportAction;
            StartupBehavior = profile.StartupBehavior;

            // Locomotion Methods:
            // - Cloudstep
            // - Smooth Artificial Locomotion
            // - On-Rails Locomotion
        }

        private readonly SystemType teleportProvider;
        private TeleportEventData teleportEventData;
        private bool isTeleporting = false;

        /// <summary>
        /// Gets the configured startup behaviour for the system.
        /// </summary>
        private AutoStartBehavior StartupBehavior { get; }

        /// <inheritdoc />
        public bool TeleportationEnabled { get; set; }

        /// <inheritdoc />
        public bool LocomotionEnabled { get; set; }

        /// <inheritdoc />
        public MixedRealityInputAction TeleportAction { get; private set; }

        /// <inheritdoc />
        public MixedRealityInputAction CancelTeleportAction { get; private set; }

        /// <summary>
        /// Gets the currently active locomotion target override, if any.
        /// </summary>
        public LocomotionTargetOverride LocomotionTargetOverride { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                teleportEventData = new TeleportEventData(EventSystem.current);
            }

            LocomotionEnabled = StartupBehavior == AutoStartBehavior.AutoStart;
            TeleportationEnabled = StartupBehavior == AutoStartBehavior.AutoStart;

            if (teleportProvider == null)
            {
                // No provider selected, we'll be using default teleport.
                // Make sure to remove any leftover provider attached to the camera.
                var component = CameraCache.Main.GetComponent(typeof(IMixedRealityTeleportProvider));

                if (!component.IsNull())
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(component);
                    }
                    else
                    {
                        Object.DestroyImmediate(component);
                    }
                }
            }
            else
            {
                // A provider is set, make sure it's attached to the camera.
                CameraCache.Main.gameObject.EnsureComponent(teleportProvider.Type);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying)
            {
                var component = CameraCache.Main.GetComponent(typeof(IMixedRealityTeleportProvider));

                if (!component.IsNull())
                {
                    Object.DestroyImmediate(component);
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (isTeleporting)
            {
                Debug.LogError("Teleportation already in progress");
                return;
            }

            isTeleporting = true;
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportStartedHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (!isTeleporting)
            {
                Debug.LogError("No Active Teleportation in progress.");
                return;
            }

            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCompletedHandler);
            isTeleporting = false;
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);
            isTeleporting = false;
        }

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var teleportData = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            base.HandleEvent(teleportData, eventHandler);
        }
    }
}
