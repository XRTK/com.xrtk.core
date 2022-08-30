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
    [System.Runtime.InteropServices.Guid("9550BBBB-799E-48AB-B421-3E64CCB7A2E7")]
    public class ViveKnucklesOpenVRController : GenericOpenVRController
    {
        /// <inheritdoc />
        public ViveKnucklesOpenVRController() { }

        /// <inheritdoc />
        public ViveKnucklesOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerProfile controllerProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerProfile)
        {
        }

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        //{
        //    new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //    new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
        //    new MixedRealityInteractionMapping("Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_9),
        //    new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
        //    new MixedRealityInteractionMapping("Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
        //    new MixedRealityInteractionMapping("Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
        //    new MixedRealityInteractionMapping("Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
        //    new MixedRealityInteractionMapping("Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
        //    new MixedRealityInteractionMapping("Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
        //    new MixedRealityInteractionMapping("Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
        //    new MixedRealityInteractionMapping("Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.AXIS_20),
        //    new MixedRealityInteractionMapping("Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.AXIS_22),
        //    new MixedRealityInteractionMapping("Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.AXIS_24),
        //    new MixedRealityInteractionMapping("Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.AXIS_26),
        //};


        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        //{
        //    new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //    new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
        //    new MixedRealityInteractionMapping("Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
        //    new MixedRealityInteractionMapping("Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
        //    new MixedRealityInteractionMapping("Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
        //    new MixedRealityInteractionMapping("Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
        //    new MixedRealityInteractionMapping("Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
        //    new MixedRealityInteractionMapping("Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
        //    new MixedRealityInteractionMapping("Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.AXIS_21),
        //    new MixedRealityInteractionMapping("Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.AXIS_23),
        //    new MixedRealityInteractionMapping("Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.AXIS_25),
        //    new MixedRealityInteractionMapping("Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.AXIS_27),
        //};
    }
}
