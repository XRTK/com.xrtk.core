// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface to implement for teleport providers working with the <see cref="Services.LocomotionSystem.MixedRealityLocomotionSystem"/>.
    /// Teleport providers perform the actual teleportation when requested.
    /// </summary>
    public interface IMixedRealityTeleportProvider : IMixedRealityLocomotionHandler { }
}
