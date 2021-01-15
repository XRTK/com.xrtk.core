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
            AllowHotSpotsOnly = profile.HotSpotsOnly;
            TeleportAction = profile.TeleportAction;
            CancelTeleportAction = profile.CancelTeleportAction;

            // Teleportation Methods:
            // - Instant Teleportation
            // - Teleport Blink
            // - Teleport Dash

            // Locomotion Methods:
            // - Cloudstep
            // - Smooth Artificial Locomotion
            // - On-Rails Locomotion
        }

        private readonly SystemType teleportProvider;
        private TeleportEventData teleportEventData;
        private bool isTeleporting = false;

        /// <inheritdoc />
        public bool CanTeleport { get; set; } = true;

        /// <inheritdoc />
        public bool AllowHotSpotsOnly { get; set; } = false;

        /// <inheritdoc />
        public MixedRealityInputAction TeleportAction { get; private set; }

        /// <inheritdoc />
        public MixedRealityInputAction CancelTeleportAction { get; private set; }

        /// <inheritdoc />
        public bool CanMove { get; set; } = true;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (Application.isPlaying)
            {
                teleportEventData = new TeleportEventData(EventSystem.current);
            }

            if (teleportProvider == null)
            {
                // No provier selected, we'll be using default teleport.
                // Make sure to remove any leftover provider attached to the camera.
                var component = CameraCache.Main.GetComponent<IMixedRealityTeleportProvider>() as Component;
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
                var component = CameraCache.Main.GetComponent<IMixedRealityTeleportProvider>() as Component;
                if (!component.IsNull())
                {
                    Object.DestroyImmediate(component);
                }
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

            // In default teleportation mode we do not expect any provider
            // to handle teleportation, instead we simply perform an instant teleport.
            if (teleportProvider == null)
            {
                PerformDefaultTeleport(teleportEventData);
            }
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

        private void PerformDefaultTeleport(TeleportEventData eventData)
        {
            var cameraTransform = MixedRealityToolkit.CameraSystem != null ?
                MixedRealityToolkit.CameraSystem.MainCameraRig.CameraTransform :
                CameraCache.Main.transform;
            var teleportTransform = cameraTransform.parent;
            Debug.Assert(teleportTransform != null,
                $"{nameof(MixedRealityLocomotionSystem)} without a provider set requires that the camera be parented under another object! Assign a teleport provider in the system profile or fix the camera setup.");

            var targetRotation = Vector3.zero;
            var targetPosition = eventData.Pointer.Result.EndPoint;
            targetRotation.y = eventData.Pointer.PointerOrientation;

            if (eventData.HotSpot != null)
            {
                targetPosition = eventData.HotSpot.Position;
                if (eventData.HotSpot.OverrideTargetOrientation)
                {
                    targetRotation.y = eventData.HotSpot.TargetOrientation;
                }
            }

            var height = targetPosition.y;
            targetPosition -= cameraTransform.position - teleportTransform.position;
            targetPosition.y = height;
            teleportTransform.position = targetPosition;
            teleportTransform.RotateAround(cameraTransform.position, Vector3.up, targetRotation.y - cameraTransform.eulerAngles.y);

            RaiseTeleportComplete(eventData.Pointer, eventData.HotSpot);
        }
    }
}