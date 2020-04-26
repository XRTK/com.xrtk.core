// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Platform agnostic hand controller type.
    /// </summary>
    public class MixedRealityHandController : BaseHandController
    {
        public MixedRealityHandController() : base() { }

        /// <inheritdoc />
        public MixedRealityHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }
    }
}