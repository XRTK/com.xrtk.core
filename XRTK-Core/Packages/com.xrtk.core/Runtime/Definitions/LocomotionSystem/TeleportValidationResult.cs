// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Possible validation outcomes by the <see cref="Interfaces.LocomotionSystem.IMixedRealityTeleportValidationDataProvider"/>.
    /// </summary>
    [Serializable]
    public enum TeleportValidationResult
    {
        None = 0,
        Valid,
        Invalid,
        HotSpot,
    }
}
