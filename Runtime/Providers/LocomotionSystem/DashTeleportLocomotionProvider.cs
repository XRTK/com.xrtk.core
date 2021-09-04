﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// A <see cref="ITeleportLocomotionProvider"/> implementation that dashes to a target location
    /// and doing so teleports the player rig to said location. The player cannot intefere or freely control
    /// the movement. The fluid translations helps with fatigue for some players.
    /// </summary>
    [System.Runtime.InteropServices.Guid("b3156486-94f3-4a02-98a9-a1c26fbf92d8")]
    public class DashTeleportLocomotionProvider : BaseTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public DashTeleportLocomotionProvider(string name, uint priority, DashTeleportLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            dashDuration = profile.DashDuration;
        }

        private readonly float dashDuration = .25f;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private LocomotionEventData locomotionEventData;
        private float dashTime;

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (IsTeleporting)
            {
                var t = dashTime / dashDuration;

                LocomotionTargetTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                LocomotionTargetTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                if (t >= 1f)
                {
                    LocomotionSystem.RaiseTeleportCompleted(this, (IMixedRealityInputSource)locomotionEventData.EventSource, locomotionEventData.Pose.Value, locomotionEventData.Anchor);
                    return;
                }

                dashTime += Time.deltaTime;
            }
        }

        /// <inheritdoc />
        public override void OnTeleportStarted(LocomotionEventData eventData)
        {
            // Was this teleport provider's teleport started and did this provider
            // actually expect a teleport to start?
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                locomotionEventData = eventData;
                var targetRotation = Vector3.zero;
                targetPosition = eventData.Pose.Value.Position;
                targetRotation.y = eventData.Pose.Value.Rotation.eulerAngles.y;

                if (eventData.Anchor != null)
                {
                    targetPosition = eventData.Anchor.Position;
                    if (eventData.Anchor.OverrideTargetOrientation)
                    {
                        targetRotation.y = eventData.Anchor.TargetOrientation;
                    }
                }

                this.targetRotation = Quaternion.Euler(targetRotation);

                startPosition = LocomotionTargetTransform.position;
                startRotation = LocomotionTargetTransform.rotation;
                dashTime = 0f;
            }

            base.OnTeleportStarted(eventData);
        }
    }
}
