// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.OpenVR
{
    /// <summary>
    /// Open VR Implementation of the Windows Mixed Reality Motion Controllers.
    /// </summary>
    [System.Runtime.InteropServices.Guid("193F55D4-44E0-4D3B-82FD-0CB417B3668B")]
    public class WindowsMixedRealityOpenVRMotionController : GenericOpenVRController
    {
        /// <inheritdoc />
        public WindowsMixedRealityOpenVRMotionController() { }

        /// <inheritdoc />
        public WindowsMixedRealityOpenVRMotionController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerProfile controllerProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerProfile)
        {
            PointerOffsetAngle = -30f;
        }

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions
        //{
        //    get
        //    {
        //        var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
        //        dualAxisProcessor.InvertY = true;
        //        return new[]
        //        {
        //            new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //            new MixedRealityInteractionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
        //            new MixedRealityInteractionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_11),
        //            new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
        //            new MixedRealityInteractionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_9),
        //            new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
        //            new MixedRealityInteractionMapping("Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_17, ControllerMappingLibrary.AXIS_18),
        //            new MixedRealityInteractionMapping("Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
        //            new MixedRealityInteractionMapping("Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
        //            new MixedRealityInteractionMapping("Menu Press", AxisType.Digital, DeviceInputType.Menu, KeyCode.JoystickButton0),
        //            new MixedRealityInteractionMapping("Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, new List<InputProcessor>{ dualAxisProcessor }),
        //            new MixedRealityInteractionMapping("Thumbstick Click", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton8),
        //        };
        //    }
        //}

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions
        //{
        //    get
        //    {
        //        var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
        //        dualAxisProcessor.InvertY = true;
        //        return new[]
        //        {
        //            new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //            new MixedRealityInteractionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
        //            new MixedRealityInteractionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_12),
        //            new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
        //            new MixedRealityInteractionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
        //            new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
        //            new MixedRealityInteractionMapping("Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_19, ControllerMappingLibrary.AXIS_20),
        //            new MixedRealityInteractionMapping("Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
        //            new MixedRealityInteractionMapping("Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
        //            new MixedRealityInteractionMapping("Menu Press", AxisType.Digital, DeviceInputType.Menu, KeyCode.JoystickButton2),
        //            new MixedRealityInteractionMapping("Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, new List<InputProcessor>{ dualAxisProcessor }),
        //            new MixedRealityInteractionMapping("Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton9),
        //        };
        //    }
        //}
    }
}
