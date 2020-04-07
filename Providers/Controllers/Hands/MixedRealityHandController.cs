// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Platform agnostic hand controller type.
    /// </summary>
    public class MixedRealityHandController : BaseHandController
    {
        /// <inheritdoc />
        public MixedRealityHandController(IMixedRealityControllerDataProvider dataProvider, TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(dataProvider, trackingState, controllerHandedness, inputSource, interactions)
        {
        }
    }
}