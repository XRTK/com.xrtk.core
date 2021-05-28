// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for handling <see cref="IMixedRealityLocomotionSystem"/> movement events in a
    /// <see cref="MonoBehaviour"/> component.
    /// </summary>
    public abstract class BaseMovementProvider : BaseLocomotionProvider, IMixedRealityMovementProvider
    {
        /// <inheritdoc />
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (LocomotionSystem == null || !LocomotionSystem.LocomotionEnabled)
            {
                return;
            }

            if (eventData.MixedRealityInputAction == LocomotionSystem.TeleportAction)
            {
                eventData.Use();

                var angle = Mathf.Atan2(eventData.InputData.x, eventData.InputData.y) * Mathf.Rad2Deg;
                var direction = Quaternion.Euler(0f, angle, 0f);

                LocomotionTargetTransform.position += direction * LocomotionTargetTransform.forward * LocomotionSystem.MovementSpeed;
            }
        }
    }
}
