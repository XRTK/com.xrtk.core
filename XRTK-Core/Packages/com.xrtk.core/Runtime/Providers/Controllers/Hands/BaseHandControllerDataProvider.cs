// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.InputSystem;
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
            if (!MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(MixedRealityInputSystemProfile)}!");
            }

            RenderingMode = profile.RenderingMode != inputSystemProfile.RenderingMode
                ? profile.RenderingMode
                : inputSystemProfile.RenderingMode;

            HandPhysicsEnabled = profile.HandPhysicsEnabled != inputSystemProfile.HandPhysicsEnabled
                ? profile.HandPhysicsEnabled
                : inputSystemProfile.HandPhysicsEnabled;

            UseTriggers = profile.UseTriggers != inputSystemProfile.UseTriggers
                ? profile.UseTriggers
                : inputSystemProfile.UseTriggers;

            BoundsMode = profile.BoundsMode != inputSystemProfile.BoundsMode
                ? profile.BoundsMode
                : inputSystemProfile.BoundsMode;

            if (profile.TrackedPoses.Count > 0)
            {
                TrackedPoses = profile.TrackedPoses.Count != inputSystemProfile.TrackedPoses.Count
                    ? profile.TrackedPoses
                    : inputSystemProfile.TrackedPoses;
            }
            else
            {
                TrackedPoses = inputSystemProfile.TrackedPoses;
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