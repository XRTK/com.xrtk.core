// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for teleport providers working with the <see cref="MixedRealityLocomotionSystem"/>.
    /// Teleport providers perform the actual teleportation when requested.
    /// </summary>
    public abstract class BaseTeleportProvider : BaseLocomotionProvider,
        IMixedRealityTeleportProvider,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>
    {
        public void OnInputChanged(InputEventData<float> eventData)
        {

        }

        public void OnInputChanged(InputEventData<Vector2> eventData)
        {

        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {

        }
    }
}
