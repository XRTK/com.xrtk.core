// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.TeleportSystem;
using XRTK.EventDatum.Teleport;
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
            : base(profile) { }

        private TeleportEventData teleportEventData;
        private TeleportEventData currentTeleportEventData;
        private bool isTeleporting = false;
        private bool isProcessingTeleportRequest = false;
        private Vector3 targetPosition;
        private Vector3 targetRotation;
        private float teleportationDelay;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying) { return; }

            teleportEventData = new TeleportEventData(EventSystem.current);
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (isProcessingTeleportRequest && teleportationDelay > 0f)
            {
                teleportationDelay -= Time.deltaTime;
                if (teleportationDelay <= 0f)
                {
                    CompleteTeleportation();
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

            ProcessTeleportationRequest(teleportEventData);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCompleted(casted);
            };

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        private void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
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
        }

        #endregion IMixedRealityTeleportSystem Implementation

        private void ProcessTeleportationRequest(TeleportEventData eventData)
        {
            if (eventData.used)
            {
                return;
            }

            eventData.Use();
            currentTeleportEventData = eventData;

            isProcessingTeleportRequest = true;

            targetRotation = Vector3.zero;
            targetPosition = eventData.Pointer.Result.EndPoint;
            targetRotation.y = eventData.Pointer.PointerOrientation;

            if (eventData.HotSpot != null)
            {
                targetPosition = eventData.HotSpot.Position;
                if (eventData.HotSpot.OverrideTargetOrientation)
                {
                    targetRotation.y = eventData.HotSpot.TargetOrientation;
                }
            }

            var cameraTransform = MixedRealityToolkit.CameraSystem == null
                ? CameraCache.Main.transform
                : MixedRealityToolkit.CameraSystem.MainCameraRig.CameraTransform;
            var cameraParent = cameraTransform.parent;
            Debug.Assert(cameraParent != null, "The Teleport System requires that the camera be parented under another object.");

            var height = targetPosition.y;
            targetPosition -= cameraTransform.position - cameraParent.position;
            targetPosition.y = height;

            // If we don't have a teleporation delay, we can complete right away.
            //if (!(TeleportDuration > 0f))
            //{
            //    CompleteTeleportation();
            //}
            //else
            //{
            //    teleportationDelay = teleportDuration;
            //}
        }

        private void CompleteTeleportation()
        {
            if (isProcessingTeleportRequest)
            {
                var cameraTransform = MixedRealityToolkit.CameraSystem == null
                ? CameraCache.Main.transform
                : MixedRealityToolkit.CameraSystem.MainCameraRig.CameraTransform;
                var cameraParent = cameraTransform.parent;
                Debug.Assert(cameraParent != null, "The Teleport System requires that the camera be parented under another object.");

                cameraParent.position = targetPosition;
                cameraParent.RotateAround(cameraTransform.position, Vector3.up, targetRotation.y - cameraTransform.eulerAngles.y);

                isProcessingTeleportRequest = false;

                // Raise complete event using the pointer and hot spot provided.
                RaiseTeleportComplete(currentTeleportEventData.Pointer, currentTeleportEventData.HotSpot);
            }
        }
    }
}
