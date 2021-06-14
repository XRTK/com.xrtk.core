// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Possible validation outcomes by the <see cref="Interfaces.LocomotionSystem.ITeleportValidationProvider"/>.
    /// </summary>
    [Serializable]
    public enum TeleportValidationResult
    {
        /// <summary>
        /// The teleport validation result is unknown. That is the case
        /// when teleport has no target.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The teleportation target is invalid.
        /// </summary>
        Invalid,
        /// <summary>
        /// The teleportation target is valid.
        /// </summary>
        Valid
    }
}
