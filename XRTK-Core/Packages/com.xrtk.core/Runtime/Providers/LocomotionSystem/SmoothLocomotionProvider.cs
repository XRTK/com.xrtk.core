// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// A simple <see cref="IFreeLocomotionProvider"/> implementation that allows free movement
    /// of the player rig, similar to a classic first person view character controller. Movement is constrained
    /// to the XZ-plane.
    /// </summary>
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

            if (eventData.InputAction == InputAction)
            {
                var forwardDirection = CameraTransform.forward;
                forwardDirection.y = 0f;

                var rightDirection = CameraTransform.right;
                rightDirection.y = 0f;

                var combinedDirection = (forwardDirection * eventData.InputData.y + rightDirection * eventData.InputData.x).normalized;
                LocomotionTargetTransform.Translate(combinedDirection * speed * Time.deltaTime, Space.World);
            }
        }
    }
}
