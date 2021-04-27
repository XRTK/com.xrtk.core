// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for handling <see cref="IMixedRealityLocomotionSystem"/> movement events in a
    /// <see cref="MonoBehaviour"/> component.
    /// </summary>
    public abstract class BaseMovementProvider : BaseLocomotionProvider, IMixedRealityMovementProvider
    {

    }
}
