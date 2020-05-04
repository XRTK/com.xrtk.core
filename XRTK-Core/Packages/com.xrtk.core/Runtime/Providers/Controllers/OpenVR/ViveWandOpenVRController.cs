// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.OpenVR
{
    [System.Runtime.InteropServices.Guid("506F192F-B93F-43B3-A34F-B15DFD855631")]
    public class ViveWandOpenVRController : GenericOpenVRController
    {
        /// <inheritdoc />
        public ViveWandOpenVRController() { }

        /// <inheritdoc />
        public ViveWandOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping("Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping("Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
            new MixedRealityInteractionMapping("Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch,  KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping("Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress,  KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping("Menu Button", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton2),
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping("Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping("Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
            new MixedRealityInteractionMapping("Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch,  KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping("Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress,  KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping("Menu Button", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton0),
        };
    }
}