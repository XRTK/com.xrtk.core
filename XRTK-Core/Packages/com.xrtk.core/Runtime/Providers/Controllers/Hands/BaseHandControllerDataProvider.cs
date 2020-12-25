// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
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

            RenderingMode = profile.RenderingMode != globalSettingsProfile.RenderingMode
                ? profile.RenderingMode
                : globalSettingsProfile.RenderingMode;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != globalSettingsProfile.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : globalSettingsProfile.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != globalSettingsProfile.UseTriggers
                ? profile.UseTriggers
                : globalSettingsProfile.UseTriggers;

            BoundsMode = profile.BoundsMode != globalSettingsProfile.BoundsMode
                ? profile.BoundsMode
                : globalSettingsProfile.BoundsMode;

            if (profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != globalSettingsProfile.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : globalSettingsProfile.TrackedPoses;
            }
            else
            {
                TrackedPoses = globalSettingsProfile.TrackedPoses;
            }
        }

        /// <inheritdoc />
        public HandRenderingMode RenderingMode { get; set; }

        /// <inheritdoc />
        public bool HandPhysicsEnabled { get; set; }

        /// <inheritdoc />
        public bool UseTriggers { get; set; }

        /// <inheritdoc />
        public HandBoundsLOD BoundsMode { get; set; }

        /// <summary>
        /// Configured <see cref="HandControllerPoseProfile"/>s for pose recognition.
        /// </summary>
        protected IReadOnlyList<HandControllerPoseProfile> TrackedPoses { get; }
    }
}