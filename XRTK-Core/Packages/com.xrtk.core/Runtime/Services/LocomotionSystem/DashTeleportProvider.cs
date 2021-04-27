// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Locomotion;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// This <see cref="Interfaces.LocomotionSystem.IMixedRealityTeleportProvider"/> implementation will
    /// dash to the target location.
    /// </summary>
    [System.Runtime.InteropServices.Guid("52f6c2d4-97f9-44c3-b88e-99ba8ecdc07f")]
    public class DashTeleportProvider : BaseTeleportProvider
    {
        [SerializeField]
        [Tooltip("Duration of the dash in seconds.")]
        private float dashDuration = .25f;

        private bool isDashing;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private LocomotionEventData locomotionEventData;
        private float dashTime;

        private void Update()
        {
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
