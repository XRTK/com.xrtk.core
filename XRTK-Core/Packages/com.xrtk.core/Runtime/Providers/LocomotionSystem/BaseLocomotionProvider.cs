// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;
using XRTK.Definitions;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    public abstract class BaseLocomotionProvider : BaseDataProvider, IMixedRealityLocomotionProvider
    {
        /// <inheritdoc />
        public BaseLocomotionProvider(string name, uint priority, BaseMixedRealityProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService) { }

        /// <inheritdoc />
        public bool IsEnabled { get; protected set; }

        /// <inheritdoc />
        public abstract LocomotionType Type { get; }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            IsEnabled = true;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();
            IsEnabled = false;
        }
    }
}
