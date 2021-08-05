// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
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

        /// <inheritdoc />
        public override void OnTeleportStarted(LocomotionEventData eventData)
        {
            // Was this teleport provider's teleport started and did this provider
            // actually expect a teleport to start?
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                var targetRotation = Vector3.zero;
                var targetPosition = eventData.Pose.Value.Position;
                targetRotation.y = eventData.Pose.Value.Rotation.eulerAngles.y;

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

                var inputSource = OpenTargetRequests[eventData.EventSource.SourceId];
                LocomotionSystem.RaiseTeleportCompleted(this, inputSource, eventData.Pose.Value, eventData.HotSpot);
            }

            base.OnTeleportStarted(eventData);
        }
    }
}
