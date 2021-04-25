// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for teleport providers working with the <see cref="MixedRealityLocomotionSystem"/>.
    /// Teleport providers perform the actual teleportation when requested.
    /// </summary>
    public abstract class BaseTeleportProvider : BaseLocomotionProvider, IMixedRealityTeleportProvider { }
}
