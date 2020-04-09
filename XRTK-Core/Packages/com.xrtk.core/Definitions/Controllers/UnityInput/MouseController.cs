// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Definitions.Controllers.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    public class MouseController : BaseController
    {
        /// <inheritdoc />
        public MouseController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(controllerDataProvider, trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                var dualAxisProcessor = ScriptableObject.CreateInstance<InvertDualAxisProcessor>();
                dualAxisProcessor.InvertX = true;
                dualAxisProcessor.InvertY = true;
                return new[]
                {
                    new MixedRealityInteractionMapping("Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
                    new MixedRealityInteractionMapping("Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition, ControllerMappingLibrary.MouseX, ControllerMappingLibrary.MouseY),
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

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private MixedRealityPose controllerPose = MixedRealityPose.ZeroIdentity;
        private Vector2 mouseDelta;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            if (!Input.mousePresent) { return; }

            base.UpdateController();

            // Bail early if our mouse isn't in our game window.
            if (Input.mousePosition.x < 0 ||
                Input.mousePosition.y < 0 ||
                Input.mousePosition.x > Screen.width ||
                Input.mousePosition.y > Screen.height)
            {
                return;
            }

            if (InputSource.Pointers[0].BaseCursor != null)
            {
                controllerPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                controllerPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
            }

            // Don't ask me why it's mapped weird. Bc Unity...
            mouseDelta.x = Input.GetAxis(Interactions[1].AxisCodeX);
            mouseDelta.y = Input.GetAxis(Interactions[1].AxisCodeY);

            MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, mouseDelta);
            MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, controllerPose);

            for (int i = 0; i < Interactions?.Length; i++)
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
                    Interactions[i].Vector2Data = mouseDelta;

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
