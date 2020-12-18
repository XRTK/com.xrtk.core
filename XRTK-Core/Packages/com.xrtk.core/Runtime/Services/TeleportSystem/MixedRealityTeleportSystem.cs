// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.TeleportSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Teleport;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.TeleportSystem;
using XRTK.Interfaces.TeleportSystem.Handlers;
using XRTK.Utilities;

namespace XRTK.Services.Teleportation
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityTeleportSystem"/>
    /// </summary>
    [System.Runtime.InteropServices.Guid("2C18630A-9837-46FA-ADDF-6C8AF34D4143")]
    public class MixedRealityTeleportSystem : BaseEventSystem, IMixedRealityTeleportSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The active <see cref="MixedRealityTeleportSystemProfile"/>.</param>
        public MixedRealityTeleportSystem(MixedRealityTeleportSystemProfile profile)
            : base(profile)
        {
            teleportHandlerComponent = profile.TeleportHandlerComponent;
        }

        private TeleportEventData teleportEventData;
        private bool isTeleporting = false;
        private readonly SystemType teleportHandlerComponent;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            var checkedHandlerComponent = false;
            if (!Application.isPlaying)
            {
                VerifyHandlerComponent();
                checkedHandlerComponent = true;
            }
            else
            {
                teleportEventData = new TeleportEventData(EventSystem.current);
            }

            if (!checkedHandlerComponent)
            {
                VerifyHandlerComponent();
            }
        }

        private void VerifyHandlerComponent()
        {
            var activeHandler = (Object)CameraCache.Main.GetComponent<IMixedRealityTeleportComponentHandler>();
            if (activeHandler.IsNull())
            {
                // No teleport handler attached to the camera yet, we can safely
                // add the configured handler.
                CameraCache.Main.gameObject.EnsureComponent(teleportHandlerComponent.Type);
            }
            else if (activeHandler.GetType() != teleportHandlerComponent.Type)
            {
                // There is handler attached to the camera but it's not the one configured
                // in the profile.
                Debug.LogWarning($"There is a {activeHandler.GetType().Name} attached to the camera but the active teleport system configuration requests a {teleportHandlerComponent.Type.Name}. " +
                    $"Likely you want to check your teleport system configuration.");
            }
        }

        #endregion IMixedRealityService Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var teleportData = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            // Process all the event listeners
            base.HandleEvent(teleportData, eventHandler);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to Teleport events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to Teleport events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion IEventSystemManager Implementation

        #region IMixedRealityTeleportSystem Implementation

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
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

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
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

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
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
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);

            isTeleporting = false;
        }

        #endregion IMixedRealityTeleportSystem Implementation
    }
}
