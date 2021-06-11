// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("790cdfd8-89c7-41c9-8dab-6b32e1e9d0a9")]
    public class InstantTeleportLocomotionProvider : BaseLocomotionProvider, IMixedRealityTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public InstantTeleportLocomotionProvider(string name, uint priority, BaseLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService) { }

        /// <inheritdoc />
        public override void OnLocomotionStarted(LocomotionEventData eventData)
        {
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

            LocomotionSystem.RaiseTeleportComplete(eventData.Pointer, eventData.HotSpot);
        }
    }
}
