// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Locomotion;
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
            teleportProviderType = profile.TeleportProvider?.Type;
            movementProviderType = profile.MovementProvider?.Type;

            TeleportAction = profile.TeleportAction;
            MovementSpeed = profile.MovementSpeed;
            MovementCancelsTeleport = profile.MovementCancelsTeleport;
            TeleportationEnabled = profile.TeleportStartupBehaviour == AutoStartBehavior.AutoStart;
            LocomotionEnabled = profile.MovementStartupBehaviour == AutoStartBehavior.AutoStart;
        }

        private readonly Type teleportProviderType;
        private readonly Type movementProviderType;
        private LocomotionEventData teleportEventData;
        private bool isTeleporting = false;

        /// <inheritdoc />
        public bool TeleportationEnabled { get; set; }

        /// <inheritdoc />
        public bool LocomotionEnabled { get; set; }

        /// <inheritdoc />
        public MixedRealityInputAction TeleportAction { get; private set; }

        /// <summary>
        /// If set, movement will cancel any teleportation in progress.
        /// </summary>
        public bool MovementCancelsTeleport { get; private set; }

        /// <summary>
        /// Gets the currently active locomotion target override, if any.
        /// </summary>
        public LocomotionTargetOverride LocomotionTargetOverride { get; set; }

        /// <summary>
        /// Speed in meters per second for movement.
        /// </summary>
        public float MovementSpeed { get; set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                teleportEventData = new LocomotionEventData(EventSystem.current);
            }

            Debug.Assert(teleportProviderType != null, $"The {nameof(MixedRealityLocomotionSystem)} requires a teleportation provider to be set. Check the active {nameof(MixedRealityLocomotionSystemProfile)} to resolve the issue.");
            Debug.Assert(movementProviderType != null, $"The {nameof(MixedRealityLocomotionSystem)} requires a movement provider to be set. Check the active {nameof(MixedRealityLocomotionSystemProfile)} to resolve the issue.");

            // Make sure to clean up any leftovers that may still be attached to the camera.
            var camera = CameraCache.Main.gameObject;
            var existingTeleportProviders = camera.GetComponents(typeof(IMixedRealityTeleportProvider));
            if (existingTeleportProviders != null)
            {
                for (var i = 0; i < existingTeleportProviders.Length; i++)
                {
                    var existingTeleportProvider = existingTeleportProviders[i];
                    if (!existingTeleportProvider.IsNull() && existingTeleportProvider.GetType() != teleportProviderType)
                    {
                        existingTeleportProvider.Destroy();
                    }
                }
            }

            camera.EnsureComponent(teleportProviderType);

            var existingMovementProviders = camera.GetComponents(typeof(IMixedRealityMovementProvider));
            if (existingMovementProviders != null)
            {
                for (var i = 0; i < existingMovementProviders.Length; i++)
                {
                    var existingMovementProvider = existingMovementProviders[i];
                    if (!existingMovementProvider.IsNull() && existingMovementProvider.GetType() != movementProviderType)
                    {
                        existingMovementProvider.Destroy();
                    }
                }
            }

            camera.EnsureComponent(movementProviderType);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (!Application.isPlaying)
            {
                var component = CameraCache.Main.gameObject.GetComponent(typeof(IMixedRealityTeleportProvider));
                if (!component.IsNull())
                {
                    component.Destroy();
                }

                component = CameraCache.Main.gameObject.GetComponent(typeof(IMixedRealityMovementProvider));
                if (!component.IsNull())
                {
                    component.Destroy();
                }
            }

            base.Destroy();
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            teleportEventData.Initialize(pointer, hotSpot);
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionStarted(casted);
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

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionCompleted(casted);
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

        private static readonly ExecuteEvents.EventFunction<IMixedRealityLocomotionHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityLocomotionHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
                handler.OnLocomotionCanceled(casted);
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
            var teleportData = ExecuteEvents.ValidateEventData<LocomotionEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            base.HandleEvent(teleportData, eventHandler);
        }
    }
}
