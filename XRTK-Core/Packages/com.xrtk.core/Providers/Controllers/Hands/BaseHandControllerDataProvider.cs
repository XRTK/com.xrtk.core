// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityHandController"/>s.
    /// </summary>
    public abstract class BaseHandControllerDataProvider : BaseControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        /// <inheritdoc />
        protected BaseHandControllerDataProvider(string name, uint priority, BaseHandControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            var globalSettingsProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;

            HandMeshingEnabled = profile.HandMeshingEnabled != globalSettingsProfile.HandMeshingEnabled
                ? profile.HandMeshingEnabled
                : globalSettingsProfile.HandMeshingEnabled;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != globalSettingsProfile.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : globalSettingsProfile.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != globalSettingsProfile.UseTriggers
                ? profile.UseTriggers
                : globalSettingsProfile.UseTriggers;

            BoundsMode = profile.BoundsMode != globalSettingsProfile.BoundsMode
                ? profile.BoundsMode
                : globalSettingsProfile.BoundsMode;
        }

        /// <inheritdoc />
        public bool HandMeshingEnabled { get; }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; }

        /// <inheritdoc />
        public bool UseTriggers { get; }

        /// <inheritdoc />
        public HandBoundsMode BoundsMode { get; }
    }
}