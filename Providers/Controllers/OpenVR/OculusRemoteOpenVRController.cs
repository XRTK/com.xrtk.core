// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.OpenVR
{
    public class OculusRemoteOpenVRController : GenericOpenVRController
    {
        public OculusRemoteOpenVRController() : base() { }

        /// <inheritdoc />
        public OculusRemoteOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping("D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_5, ControllerMappingLibrary.AXIS_6),
            new MixedRealityInteractionMapping("Button.One", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping("Button.Two", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
        };
    }
}
