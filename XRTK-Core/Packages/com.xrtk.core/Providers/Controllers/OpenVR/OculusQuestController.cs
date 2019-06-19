// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.Controllers.OpenVR
{
    public class OculusQuestController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusQuestController(TrackingState trackingState, Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_13),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(6, "Axis1D.PrimaryHandTrigger", AxisType.Digital, DeviceInputType.Trigger, KeyCode.JoystickButton4),
            new MixedRealityInteractionMapping(7, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(8, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(10, "Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton12),
            new MixedRealityInteractionMapping(11, "Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton13),
            new MixedRealityInteractionMapping(12, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(13, "Thumb Touch", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_15),
            new MixedRealityInteractionMapping(14, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton6)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_14),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(6, "Axis1D.PrimaryHandTrigger", AxisType.Digital, DeviceInputType.Trigger, KeyCode.JoystickButton5),
            new MixedRealityInteractionMapping(7, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(8, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(10, "Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton10),
            new MixedRealityInteractionMapping(11, "Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton11),
            new MixedRealityInteractionMapping(12, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(13, "Thumb Touch", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_17)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }
    }
}