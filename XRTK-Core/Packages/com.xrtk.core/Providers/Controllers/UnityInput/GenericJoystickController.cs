// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// The <see cref="GenericJoystickController"/> attempts to be a catch-all for joysticks and controllers defined in Unity's legacy input system.
    /// </summary>
    public class GenericJoystickController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
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
                    interactionMapping.BoolData = Input.GetAxisRaw(interactionMapping.AxisCodeX).Equals(interactionMapping.InvertXAxis ? -1f : 1f);
                    break;
                case DeviceInputType.TriggerNearTouch:
                case DeviceInputType.ThumbNearTouch:
                case DeviceInputType.IndexFingerNearTouch:
                case DeviceInputType.MiddleFingerNearTouch:
                case DeviceInputType.RingFingerNearTouch:
                case DeviceInputType.PinkyFingerNearTouch:
                    Debug.Assert(!string.IsNullOrEmpty(interactionMapping.AxisCodeX), $"[{interactionMapping.Description}] Axis mapping does not have an Axis defined");
                    interactionMapping.BoolData = !Input.GetAxisRaw(interactionMapping.AxisCodeX).Equals(0f);
                    break;
                default:
                    interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                    break;
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

            interactionMapping.FloatData = Input.GetAxisRaw(interactionMapping.AxisCodeX);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Select:
                case DeviceInputType.Trigger:
                case DeviceInputType.TriggerPress:
                case DeviceInputType.TouchpadPress:
                    interactionMapping.BoolData = interactionMapping.FloatData.Equals(interactionMapping.InvertXAxis ? -1f : 1f);
                    break;
                case DeviceInputType.TriggerTouch:
                case DeviceInputType.TouchpadTouch:
                case DeviceInputType.TriggerNearTouch:
                    interactionMapping.BoolData = !interactionMapping.FloatData.Equals(0f);
                    break;
                default:
                    Debug.LogError($"Input [{interactionMapping.InputType}] is not handled for this controller [{GetType().Name}]");
                    return;
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