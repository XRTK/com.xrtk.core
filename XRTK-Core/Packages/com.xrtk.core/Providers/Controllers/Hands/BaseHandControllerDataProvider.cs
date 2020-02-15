// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation with shared / common elements between all supported hand controllers.
    /// </summary>
    public abstract class BaseHandControllerDataProvider<T> : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider where T : MixedRealityHandControllerDataProviderProfile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        public BaseHandControllerDataProvider(string name, uint priority, T profile)
            : base(name, priority, profile)
        {
            Profile = profile;
        }

        /// <summary>
        /// Gets the active hand controller data provider profile.
        /// </summary>
        protected T Profile { get; }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; private set; }

        /// <inheritdoc />
        public bool UseTriggers { get; private set; }

        /// <inheritdoc />
        public HandBoundsMode BoundsMode { get; private set; }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            HandPhysicsEnabled = Profile.HandPhysicsEnabled;
            UseTriggers = Profile.UseTriggers;
            BoundsMode = Profile.BoundsMode;
        }
    }
}