// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.OpenVR
{
    [System.Runtime.InteropServices.Guid("F5779130-4990-44F7-A61A-196B34FA3AEF")]
    public class OculusTouchOpenVRController : GenericOpenVRController
    {
        /// <inheritdoc />
        public OculusTouchOpenVRController() { }

        /// <inheritdoc />
        public OculusTouchOpenVRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerProfile controllerProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerProfile) { }

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        //{
        //    new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_13),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_9),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryHandTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
        //    new MixedRealityInteractionMapping("Axis1D.PrimaryHandTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_11),
        //    new MixedRealityInteractionMapping("Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
        //    new MixedRealityInteractionMapping("Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton16),
        //    new MixedRealityInteractionMapping("Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_15),
        //    new MixedRealityInteractionMapping("Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton8),
        //    new MixedRealityInteractionMapping("Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
        //    new MixedRealityInteractionMapping("Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
        //    new MixedRealityInteractionMapping("Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
        //    new MixedRealityInteractionMapping("Button.Three Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton12),
        //    new MixedRealityInteractionMapping("Button.Four Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton13),
        //    new MixedRealityInteractionMapping("Touch.PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton18),
        //    new MixedRealityInteractionMapping("Touch.PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_17),
        //    new MixedRealityInteractionMapping("Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip)
        //};

        ///// <inheritdoc />
        //public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        //{
        //    new MixedRealityInteractionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_14),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_10),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryHandTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
        //    new MixedRealityInteractionMapping("Axis1D.SecondaryHandTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_12),
        //    new MixedRealityInteractionMapping("Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
        //    new MixedRealityInteractionMapping("Button.SecondaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton17),
        //    new MixedRealityInteractionMapping("Button.SecondaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_16),
        //    new MixedRealityInteractionMapping("Button.SecondaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton9),
        //    new MixedRealityInteractionMapping("Button.One Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
        //    new MixedRealityInteractionMapping("Button.Two Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
        //    new MixedRealityInteractionMapping("Button.One Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton10),
        //    new MixedRealityInteractionMapping("Button.Two Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton11),
        //    new MixedRealityInteractionMapping("Touch.SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton19),
        //    new MixedRealityInteractionMapping("Touch.SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_18),
        //    new MixedRealityInteractionMapping("Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip)
        //};

        ///// <inheritdoc />
        //protected override MixedRealityPose GripPoseOffset => new MixedRealityPose(Vector3.zero, Quaternion.Euler(0f, 0f, -90f));

        //public override void UpdateController()
        //{
        //    base.UpdateController();

        //    if (TrackingState == TrackingState.Tracked)
        //    {
        //        for (int i = 0; i < Interactions?.Length; i++)
        //        {
        //            var interactionMapping = Interactions[i];
        //            switch (interactionMapping.InputType)
        //            {
        //                case DeviceInputType.SpatialGrip:
        //                    UpdateSpatialGripData(interactionMapping);
        //                    interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private void UpdateSpatialGripData(MixedRealityInteractionMapping interactionMapping)
        //{
        //    Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);
        //    interactionMapping.PoseData = new MixedRealityPose(
        //        CurrentControllerPose.Position + CurrentControllerPose.Rotation * GripPoseOffset.Position,
        //        CurrentControllerPose.Rotation * GripPoseOffset.Rotation);
        //}
    }
}
