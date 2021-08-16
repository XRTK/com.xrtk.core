// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Interfaces.Providers.Controllers.Hands
{
    public interface IMixedRealityHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Gets or sets the current rendering mode for hand controllers.
        /// </summary>
        HandRenderingMode RenderingMode { get; set; }

        /// <summary>
        /// Gets or sets whether hand physics are enabled.
        /// </summary>
        bool HandPhysicsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether hand colliders should be configured as triggers.
        /// </summary>
        bool UseTriggers { get; }

        /// <summary>
        /// Gets or sets the configured hand bounds mode to be used with hand physics.
        /// </summary>
        HandBoundsLOD BoundsMode { get; set; }

        /// <summary>
        /// Gets configured <see cref="HandControllerPoseProfile"/>s for pose recognition.
        /// </summary>
        IReadOnlyDictionary<string, HandControllerPoseProfile> TrackedPoses { get; }
    }
}
