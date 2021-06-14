// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    public abstract class BaseTeleportLocomotionProvider : BaseLocomotionProvider, ITeleportLocomotionProvider
    {
        /// <inheritdoc />
        public BaseTeleportLocomotionProvider(string name, uint priority, BaseLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService) { }

        /// <inheritdoc />
        public bool IsTeleporting { get; protected set; }
    }
}
