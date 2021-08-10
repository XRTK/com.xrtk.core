// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.EventDatum.Input;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("1be53dfa-b8ae-4eb8-8459-17a5df87ade5")]
    public class SmoothLocomotionProvider : BaseLocomotionProvider, IFreeLocomotionProvider
    {
        /// <inheritdoc />
        public SmoothLocomotionProvider(string name, uint priority, SmoothLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            speed = profile.Speed;
        }

        private readonly float speed;

        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            base.OnInputChanged(eventData);

            if (eventData.MixedRealityInputAction == InputAction)
            {
                eventData.Use();

                var angle = Mathf.Atan2(eventData.InputData.x, eventData.InputData.y) * Mathf.Rad2Deg;
                var direction = Quaternion.Euler(0f, angle, 0f);

                LocomotionTargetTransform.position += direction * LocomotionTargetTransform.forward * speed;
            }
        }
    }
}
