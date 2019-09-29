// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;

namespace XRTK.Providers.Controllers.UnityInput
{
    public class GenericJoystickController : BaseController
    {
        public GenericJoystickController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The pointer's offset angle.
        /// </summary>
        public float PointerOffsetAngle { get; protected set; } = 0f;

        private Vector2 dualAxisPosition = Vector2.zero;
        protected Vector3 CurrentControllerPosition = Vector3.zero;
        protected Quaternion CurrentControllerRotation = Quaternion.identity;
        private MixedRealityPose pointerOffsetPose = MixedRealityPose.ZeroIdentity;
        protected MixedRealityPose LastControllerPose = MixedRealityPose.ZeroIdentity;
        protected MixedRealityPose CurrentControllerPose = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            // Generic unity controller's will not have default interactions
        }

        /// <summary>
        /// Update the controller data from Unity's Input Manager
        /// </summary>
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            base.UpdateController();

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for {GetType().Name}");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                var interactionMapping = Interactions[i];

                switch (interactionMapping.AxisType)
                {
                    case AxisType.None:
                        break;
                    case AxisType.Digital:
                        UpdateButtonData(interactionMapping);
                        break;
                    case AxisType.SingleAxis:
                        UpdateSingleAxisData(interactionMapping);
                        break;
                    case AxisType.DualAxis:
                        UpdateDualAxisData(interactionMapping);
                        break;
                    case AxisType.ThreeDofRotation:
                    case AxisType.ThreeDofPosition:
                    case AxisType.SixDof:
                        UpdatePoseData(interactionMapping);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [{GetType().Name}]");
                        break;
                }

                interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
            }
        }

        /// <summary>
        /// Update an Interaction Bool data type from a Bool input 
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// Also raises a "Pressed" event while pressed
        /// </remarks>
        protected void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                    Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeX), $"[{interactionMapping.Description}] Axis mapping does not have an Axis defined");
                    interactionMapping.BoolData = Input.GetAxisRaw(interactionMapping.AxisCodeX).Equals(1);
                    break;
                case DeviceInputType.TriggerNearTouch:
                case DeviceInputType.ThumbNearTouch:
                case DeviceInputType.IndexFingerNearTouch:
                case DeviceInputType.MiddleFingerNearTouch:
                case DeviceInputType.RingFingerNearTouch:
                case DeviceInputType.PinkyFingerNearTouch:
                    Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeX), $"[{interactionMapping.Description}] Axis mapping does not have an Axis defined");
                    interactionMapping.BoolData = !Input.GetAxisRaw(interactionMapping.AxisCodeX).Equals(0);
                    break;
                default:
                    interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                    break;
            }

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it is enabled
                if (interactionMapping.BoolData)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }

            // If pressed always raise pressed update.
            if (interactionMapping.Updated)
            {
                MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
            }
        }

        /// <summary>
        /// Update an Interaction Float data type from a SingleAxis (float) input 
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Pressed" event when the float data changes
        /// </remarks>
        /// <param name="interactionMapping"></param>
        protected void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
            Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeX), $"[{interactionMapping.Description}] Single Axis mapping does not have an Axis defined");

            var singleAxisValue = Input.GetAxisRaw(interactionMapping.AxisCodeX);

            if (interactionMapping.InputType == DeviceInputType.TriggerPress)
            {
                // Update the interaction data source
                interactionMapping.BoolData = singleAxisValue.Equals(1);
            }
            else
            {
                // Update the interaction data source
                interactionMapping.FloatData = singleAxisValue;
            }
        }

        /// <summary>
        /// Update the Touchpad / Thumbstick input from the device (in OpenVR, touchpad and thumbstick are the same input control)
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);
            Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeX), $"[{interactionMapping.Description}] Dual Axis mapping does not have an Axis defined for X Axis");
            Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeY), $"[{interactionMapping.Description}] Dual Axis mapping does not have an Axis defined for Y Axis");

            dualAxisPosition.x = Input.GetAxis(interactionMapping.AxisCodeX);
            dualAxisPosition.y = Input.GetAxis(interactionMapping.AxisCodeY);

            // Update the interaction data source
            interactionMapping.Vector2Data = dualAxisPosition;
        }

        /// <summary>
        /// Update Spatial Pointer Data.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdatePoseData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.SpatialPointer:
                    pointerOffsetPose.Position = CurrentControllerPose.Position;
                    pointerOffsetPose.Rotation = CurrentControllerPose.Rotation * Quaternion.AngleAxis(PointerOffsetAngle, Vector3.left);

                    // Update the interaction data source
                    interactionMapping.PoseData = pointerOffsetPose;
                    break;
                case DeviceInputType.SpatialGrip:
                    // Update the interaction data source
                    interactionMapping.PoseData = CurrentControllerPose;
                    break;
                default:
                    Debug.LogWarning($"Unhandled Interaction {interactionMapping.Description}");
                    break;
            }
        }
    }
}