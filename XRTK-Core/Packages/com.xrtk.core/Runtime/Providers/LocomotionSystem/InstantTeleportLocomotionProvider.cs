// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// A simple <see cref="ITeleportLocomotionProvider"/> implementation that teleports
    /// the player rig instantly to a target location within a single frame.
    /// </summary>
    [System.Runtime.InteropServices.Guid("790cdfd8-89c7-41c9-8dab-6b32e1e9d0a9")]
    public class InstantTeleportLocomotionProvider : BaseTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public InstantTeleportLocomotionProvider(string name, uint priority, BaseTeleportLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService) { }

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

                if (eventData.Anchor != null)
                {
                    targetPosition = eventData.Anchor.Position;
                    if (eventData.Anchor.OverrideTargetOrientation)
                    {
                        targetRotation.y = eventData.Anchor.TargetOrientation;
                    }
                }

                var height = targetPosition.y;
                targetPosition -= CameraTransform.position - LocomotionTargetTransform.position;
                targetPosition.y = height;
                LocomotionTargetTransform.position = targetPosition;
                LocomotionTargetTransform.RotateAround(CameraTransform.position, Vector3.up, targetRotation.y - CameraTransform.eulerAngles.y);

                var inputSource = OpenTargetRequests[eventData.EventSource.SourceId];
                LocomotionSystem.RaiseTeleportCompleted(this, inputSource, eventData.Pose.Value, eventData.Anchor);
            }

            base.OnTeleportStarted(eventData);
        }
    }
}
