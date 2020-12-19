// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.TeleportSystem;
using XRTK.Interfaces.TeleportSystem;

namespace XRTK.Services.Teleportation
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityTeleportValidationDataProvider"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("14199fd8-1636-4147-bb08-6475e76ed1cd")]
    public class MixedRealityTeleportValidationDataProvider : BaseDataProvider, IMixedRealityTeleportValidationDataProvider
    {
        /// <inheritdoc />
        public MixedRealityTeleportValidationDataProvider(string name, uint priority, MixedRealityTeleportValidationDataProviderProfile profile, IMixedRealityTeleportSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }
    }
}
