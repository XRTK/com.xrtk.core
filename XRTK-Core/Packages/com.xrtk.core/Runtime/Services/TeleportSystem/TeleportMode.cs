// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.Teleportation
{
    /// <summary>
    /// Supported teleport modes in the <see cref="MixedRealityTeleportSystem"/> implementation.
    /// </summary>
    public enum TeleportMode
    {
        /// <summary>
        /// <see cref="Default"/> will instantly teleport the player
        /// to the selected location and does not require any additional setup.
        /// </summary>
        Default = 0,
        /// <summary>
        /// <see cref="Provider"/> mode lets <see cref="Interfaces.TeleportSystem.Handlers.IMixedRealityTeleportProvider"/>'s
        /// handle teleportation. This mode requires <see cref="Definitions.TeleportSystem.MixedRealityTeleportSystemProfile.TeleportProvider"/>
        /// to be configured with a concrete <see cref="Interfaces.TeleportSystem.Handlers.IMixedRealityTeleportProvider"/> implementation.
        /// </summary>
        Provider
    }
}
