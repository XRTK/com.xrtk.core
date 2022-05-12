﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.InputSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to define teleportation validation data providers. A <see cref="ITeleportValidationProvider"/>
    /// is responsible for validating whether a given teleportation target position is valid or not.
    /// </summary>
    public interface ITeleportValidationProvider : ILocomotionSystemDataProvider
    {
        /// <summary>
        /// Validates a <see cref="IPointerResult"/> and returns whether the <see cref="RaycastHit"/>
        /// qualifies for teleporation.
        /// </summary>
        /// <param name="pointerResult">The <see cref="IPointerResult"/> to validate.</param>
        /// <param name="anchor"><see cref="ITeleportAnchor"/> found at the target position, if any.</param>
        /// <returns>The <see cref="TeleportValidationResult"/> for <paramref name="pointerResult"/>.</returns>
        TeleportValidationResult IsValid(IPointerResult pointerResult, ITeleportAnchor anchor = null);
    }
}
