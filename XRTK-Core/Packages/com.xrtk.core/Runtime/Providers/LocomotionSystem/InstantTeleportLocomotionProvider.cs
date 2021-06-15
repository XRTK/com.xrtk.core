// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
using XRTK.Definitions.LocomotionSystem;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("790cdfd8-89c7-41c9-8dab-6b32e1e9d0a9")]
    public class InstantTeleportLocomotionProvider : BaseTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public InstantTeleportLocomotionProvider(string name, uint priority, InstantTeleportLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            InputAction = profile.InputAction;
        }

        private readonly Dictionary<uint, IMixedRealityInputSource> targetRequestsDict = new Dictionary<uint, IMixedRealityInputSource>();

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            if (!eventData.used && eventData.MixedRealityInputAction == InputAction &&
                !targetRequestsDict.ContainsKey(eventData.SourceId))
            {
                Debug.Log($"{nameof(InstantTeleportLocomotionProvider)} - Requested teleport target from input source {eventData.SourceId}.");

                targetRequestsDict.Add(eventData.SourceId, eventData.InputSource);
                LocomotionSystem.RaiseTeleportTargetRequest(this, eventData.InputSource);
                eventData.Use();
            }
        }

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);

            if (!eventData.used && eventData.MixedRealityInputAction == InputAction &&
                targetRequestsDict.ContainsKey(eventData.SourceId))
            {
                targetRequestsDict.Remove(eventData.SourceId);
            }
        }

        /// <inheritdoc />
        public override void OnTeleportStarted(LocomotionEventData eventData)
        {
            base.OnTeleportStarted(eventData);

            // Was our teleport request answered and we get to start performing teleport?
            if (!eventData.used && eventData.LocomotionProvider == this &&
                targetRequestsDict.ContainsKey(eventData.EventSource.SourceId))
            {
                Debug.Log($"{nameof(InstantTeleportLocomotionProvider)} - Started teleport using target provided by input source {eventData.EventSource.SourceId}.");
                IsTeleporting = true;

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
                targetPosition -= CameraTransform.position - LocomotionTargetTransform.position;
                targetPosition.y = height;
                LocomotionTargetTransform.position = targetPosition;
                LocomotionTargetTransform.RotateAround(CameraTransform.position, Vector3.up, targetRotation.y - CameraTransform.eulerAngles.y);

                LocomotionSystem.RaiseTeleportCompleted(this, eventData.Pointer, eventData.HotSpot);

                eventData.Use();
            }
        }

        /// <inheritdoc />
        public override void OnTeleportCompleted(LocomotionEventData eventData)
        {
            base.OnTeleportCompleted(eventData);

            // Did our teleport complete?
            if (!eventData.used && eventData.LocomotionProvider == this &&
                targetRequestsDict.ContainsKey(eventData.EventSource.SourceId))
            {
                Debug.Log($"{nameof(InstantTeleportLocomotionProvider)} - Completed teleport for input source {eventData.EventSource.SourceId}.");
                targetRequestsDict.Remove(eventData.Pointer.InputSourceParent.SourceId);
                eventData.Use();
                IsTeleporting = false;
            }
        }

        /// <inheritdoc />
        public override void OnTeleportCanceled(LocomotionEventData eventData)
        {
            base.OnTeleportCanceled(eventData);

            // Have we requested a teleportation target and got canceled?
            if (!eventData.used && eventData.LocomotionProvider == this &&
                targetRequestsDict.ContainsKey(eventData.EventSource.SourceId))
            {
                Debug.Log($"{nameof(InstantTeleportLocomotionProvider)} - Canceled teleport for input source {eventData.EventSource.SourceId}.");
                targetRequestsDict.Remove(eventData.Pointer.InputSourceParent.SourceId);
                eventData.Use();
                IsTeleporting = false;
            }
        }
    }
}
