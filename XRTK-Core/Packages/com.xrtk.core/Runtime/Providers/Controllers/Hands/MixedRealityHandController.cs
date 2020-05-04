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
    [System.Runtime.InteropServices.Guid("B18A9A6C-E5FD-40AE-89E9-9822415EC62B")]
    public class MixedRealityHandController : BaseHandController
    {
        public MixedRealityHandController() { }

        /// <inheritdoc />
        public MixedRealityHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }
    }
}