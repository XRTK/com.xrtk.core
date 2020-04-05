// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.InputSystem.Controllers.Hands;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityHandController"/>s.
    /// </summary>
    public abstract class BaseHandControllerDataProvider : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        /// <inheritdoc />
        public bool HandMeshingEnabled { get; }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; }

        /// <inheritdoc />
        public bool UseTriggers { get; }

        /// <inheritdoc />
        public HandBoundsMode BoundsMode { get; }

        protected BaseHandControllerDataProvider(string name, uint priority, BaseHandControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
            HandMeshingEnabled = profile.HandMeshingEnabled;
            HandPhysicsEnabled = profile.HandPhysicsEnabled;
            UseTriggers = profile.UseTriggers;
            BoundsMode = profile.BoundsMode;
        }
    }
}