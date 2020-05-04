// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// Xbox Controller using Unity Input System
    /// </summary>
    [System.Runtime.InteropServices.Guid("71E70C1B-9F77-4B69-BFC7-974905BB7702")]
    public class XboxController : GenericJoystickController
    {
        /// <inheritdoc />
        public XboxController() { }

        /// <inheritdoc />
        public XboxController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                dualAxisProcessor.InvertY = true;
                return new[]
                {
                    new MixedRealityInteractionMapping("Left Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, new List<InputProcessor>{ dualAxisProcessor }),
                    new MixedRealityInteractionMapping("Left Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton8),
                    new MixedRealityInteractionMapping("Right Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, new List<InputProcessor>{ dualAxisProcessor }),
                    new MixedRealityInteractionMapping("Right Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton9),
                    new MixedRealityInteractionMapping("D-Pad", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_6, ControllerMappingLibrary.AXIS_7, new List<InputProcessor>{ dualAxisProcessor }),
                    new MixedRealityInteractionMapping("Shared Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_3),
                    new MixedRealityInteractionMapping("Left Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
                    new MixedRealityInteractionMapping("Right Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
                    new MixedRealityInteractionMapping("View", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton6),
                    new MixedRealityInteractionMapping("Menu", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
                    new MixedRealityInteractionMapping("Left Bumper", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton4),
                    new MixedRealityInteractionMapping("Right Bumper", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton5),
                    new MixedRealityInteractionMapping("A", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
                    new MixedRealityInteractionMapping("B", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
                    new MixedRealityInteractionMapping("X", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
                    new MixedRealityInteractionMapping("Y", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
                };
            }
        }
    }
}