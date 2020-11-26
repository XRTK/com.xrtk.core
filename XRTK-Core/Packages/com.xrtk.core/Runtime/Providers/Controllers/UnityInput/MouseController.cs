// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    [System.Runtime.InteropServices.Guid("A80E17F6-8221-49C3-BC4B-CAB495C91D6C")]
    public class MouseController : BaseController
    {
        /// <inheritdoc />
        public MouseController() { }

        /// <inheritdoc />
        public MouseController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                var singleAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                singleAxisProcessor.InvertX = true;
                var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                dualAxisProcessor.InvertX = true;
                dualAxisProcessor.InvertY = true;
                return new[]
                {
                    new MixedRealityInteractionMapping("Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
                    new MixedRealityInteractionMapping("Mouse Position", AxisType.DualAxis, DeviceInputType.PointerPosition, ControllerMappingLibrary.MouseY, ControllerMappingLibrary.MouseX, new List<InputProcessor>{ singleAxisProcessor }),
                    new MixedRealityInteractionMapping("Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll, ControllerMappingLibrary.MouseScroll, ControllerMappingLibrary.MouseScroll, new List<InputProcessor>{ dualAxisProcessor }),
                    new MixedRealityInteractionMapping("Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse0),
                    new MixedRealityInteractionMapping("Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse1),
                    new MixedRealityInteractionMapping("Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse2),
                    new MixedRealityInteractionMapping("Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse3),
                    new MixedRealityInteractionMapping("Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse4),
                    new MixedRealityInteractionMapping("Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse5),
                    new MixedRealityInteractionMapping("Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse6),
                };
            }
        }

        public static bool IsInGameWindow => Input.mousePresent &&
                                             (Input.mousePosition.x > 0 ||
                                              Input.mousePosition.y > 0 ||
                                              Input.mousePosition.x < Screen.width ||
                                              Input.mousePosition.y < Screen.height);

        private MixedRealityPose controllerPose = MixedRealityPose.ZeroIdentity;
        private Vector2 mousePosition;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            base.UpdateController();

            // Bail early if our mouse isn't in our game window.
            if (!IsInGameWindow)
            {
                return;
            }

            if (InputSource.Pointers[0].BaseCursor != null)
            {
                controllerPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                controllerPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
            }

            mousePosition.x = Input.GetAxis(Interactions[1].AxisCodeX);
            mousePosition.y = Input.GetAxis(Interactions[1].AxisCodeY);

            MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, mousePosition);
            MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, controllerPose);

            for (int i = 0; i < Interactions.Length; i++)
            {
                if (Interactions[i].InputType == DeviceInputType.SpatialPointer)
                {
                    Interactions[i].PoseData = controllerPose;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, Interactions[i].MixedRealityInputAction, Interactions[i].PoseData);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.PointerPosition)
                {
                    Interactions[i].Vector2Data = mousePosition;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.Scroll)
                {
                    Interactions[i].Vector2Data = Input.mouseScrollDelta;

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
                    }
                }

                if (Interactions[i].AxisType == AxisType.Digital)
                {
                    var keyButton = Input.GetKey(Interactions[i].KeyCode);

                    // Update the interaction data source
                    Interactions[i].BoolData = keyButton;

                    // If our value changed raise it.
                    if (Interactions[i].ControlActivated)
                    {
                        // Raise input system Event if it enabled
                        if (Interactions[i].BoolData)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                        }
                    }

                    // If our value was updated, raise it.
                    if (Interactions[i].Updated)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                    }
                }
            }
        }
    }
}
