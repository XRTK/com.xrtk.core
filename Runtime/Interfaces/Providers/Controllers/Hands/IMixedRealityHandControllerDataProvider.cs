// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Interfaces.Providers.Controllers.Hands
{
    public interface IMixedRealityHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Gets the current rendering mode for hand controllers.
        /// </summary>
        HandRenderingMode RenderingMode { get; set; }

        /// <summary>
        /// Are hand physics enabled?
        /// </summary>
        bool HandPhysicsEnabled { get; set; }

        /// <summary>
        /// Shall hand colliders be triggers?
        /// </summary>
        bool UseTriggers { get; set; }

        /// <summary>
        /// Gets the configured hand bounds mode to be used with hand physics.
        /// </summary>
        HandBoundsLOD BoundsMode { get; set; }
    }
}