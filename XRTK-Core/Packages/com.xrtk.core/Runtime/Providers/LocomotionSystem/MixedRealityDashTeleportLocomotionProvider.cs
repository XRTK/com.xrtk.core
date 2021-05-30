// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Services;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("b3156486-94f3-4a02-98a9-a1c26fbf92d8")]
    public class MixedRealityDashTeleportLocomotionProvider : BaseLocomotionProvider
    {
        /// <inheritdoc />
        public MixedRealityDashTeleportLocomotionProvider(string name, uint priority, MixedRealityDashTeleportLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            dashDuration = profile.DashDuration;
        }

        private readonly float dashDuration = .25f;
        private bool isDashing;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private LocomotionEventData locomotionEventData;
        private float dashTime;

        /// <inheritdoc />
        public override LocomotionType Type => LocomotionType.Teleport;

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (isDashing)
            {
                var t = dashTime / dashDuration;

                LocomotionTargetTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                LocomotionTargetTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                if (t >= 1f)
                {
                    LocomotionSystem.RaiseTeleportComplete(locomotionEventData.Pointer, locomotionEventData.HotSpot);
                    isDashing = false;
                }

                dashTime += Time.deltaTime;
            }
        }

        /// <inheritdoc />
        public override void OnLocomotionStarted(LocomotionEventData eventData)
        {
            if (eventData.used)
            {
                return;
            }

            eventData.Use();

            locomotionEventData = eventData;
            var targetRotation = Vector3.zero;
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

            this.targetRotation = Quaternion.Euler(targetRotation);

            startPosition = LocomotionTargetTransform.position;
            startRotation = LocomotionTargetTransform.rotation;
            dashTime = 0f;
            isDashing = true;
        }

        /// <inheritdoc />
        public override void OnLocomotionCompleted(LocomotionEventData eventData) => isDashing = false;

        /// <inheritdoc />
        public override void OnLocomotionCanceled(LocomotionEventData eventData) => isDashing = false;
    }
}
