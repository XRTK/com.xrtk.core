// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.OpenVR
{
    [System.Runtime.InteropServices.Guid("F57DF6F5-167E-45E8-B3F2-195D2C57A3F4")]
    public class OculusGoOpenVRController : GenericOpenVRController
    {
        /// <inheritdoc />
        public OculusGoOpenVRController() { }

        /// <inheritdoc />
        public OculusGoOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerProfile controllerProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerProfile)
        {
        }

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        //{
        //    new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //    new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
        //    new MixedRealityInteractionMapping("Back", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
        //    new MixedRealityInteractionMapping("PrimaryTouchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
        //    new MixedRealityInteractionMapping("PrimaryTouchpad Click", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
        //    new MixedRealityInteractionMapping("PrimaryTouchpad Axis", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5)
        //};
    }
}
