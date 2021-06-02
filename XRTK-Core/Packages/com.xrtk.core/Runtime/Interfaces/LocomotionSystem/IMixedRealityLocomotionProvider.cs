// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.InputSystem.Handlers;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// The base interface to define locomotion providers for the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public interface IMixedRealityLocomotionProvider : IMixedRealityLocomotionDataProvider,
        IMixedRealityLocomotionHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>
    {
        /// <summary>
        /// Gets whether this locomotion provider is enabled and handling locomotion requests.
        /// </summary>
        bool IsEnabled { get; }
    }
}
