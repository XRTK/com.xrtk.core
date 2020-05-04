// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.UnityInput
{
    [System.Runtime.InteropServices.Guid("98F97EDA-4418-4B4B-88E9-E4F1F0734E4E")]
    public class UnityTouchController : BaseController
    {
        /// <inheritdoc />
        public UnityTouchController() { }

        /// <inheritdoc />
        public UnityTouchController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        private const float K_CONTACT_EPSILON = 30.0f;

        /// <summary>
        /// Time in seconds to determine if the contact registers as a tap or a hold
        /// </summary>
        public float MaxTapContactTime { get; set; } = 0.5f;

        /// <summary>
        /// The threshold a finger must move before starting a manipulation gesture.
        /// </summary>
        public float ManipulationThreshold { get; set; } = 5f;

        /// <summary>
        /// Current Touch Data for the Controller.
        /// </summary>
        public Touch TouchData { get; internal set; }

        /// <summary>
        /// Current Screen point ray for the Touch.
        /// </summary>
        public Ray ScreenPointRay { get; internal set; }

        /// <summary>
        /// The current lifetime of the Touch.
        /// </summary>
        public float Lifetime { get; private set; } = 0.0f;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping("Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInteractionMapping("Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping("Touch Press", AxisType.Digital, DeviceInputType.PointerClick),
            new MixedRealityInteractionMapping("Touch Hold", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping("Touch Drag", AxisType.DualAxis, DeviceInputType.Touchpad)
        };

        private bool isTouched;
        private bool isHolding;
        private bool isManipulating;
        private MixedRealityPose lastPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Start the touch.
        /// </summary>
        public void StartTouch()
        {
            MixedRealityToolkit.InputSystem?.RaisePointerDown(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);
            isTouched = true;
            MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, Interactions[4].MixedRealityInputAction);
            isHolding = true;
        }

        /// <summary>
        /// Update the touch data.
        /// </summary>
        public void Update()
        {
            UpdateController();

            if (!isTouched) { return; }

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                Interactions[0].Vector2Data = TouchData.deltaPosition;

                // If our value was updated, raise it.
                if (Interactions[0].Updated)
                {
                    MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, Interactions[0].MixedRealityInputAction, TouchData.deltaPosition);
                }

                if (InputSource.Pointers[0].BaseCursor != null)
                {
                    lastPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                    lastPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
                }

                MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, lastPose);

                Interactions[1].PoseData = lastPose;

                // If our value was updated, raise it.
                if (Interactions[1].Updated)
                {
                    MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, Interactions[1].MixedRealityInputAction, lastPose);
                }

                if (!isManipulating)
                {
                    if (Mathf.Abs(TouchData.deltaPosition.x) > ManipulationThreshold ||
                        Mathf.Abs(TouchData.deltaPosition.y) > ManipulationThreshold)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, Interactions[4].MixedRealityInputAction);
                        isHolding = false;

                        MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, Interactions[5].MixedRealityInputAction);
                        isManipulating = true;
                    }
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureUpdated(this, Interactions[5].MixedRealityInputAction, TouchData.deltaPosition);
                }
            }
        }

        /// <summary>
        /// End the touch.
        /// </summary>
        public void EndTouch()
        {
            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < K_CONTACT_EPSILON)
                {
                    if (isHolding)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, Interactions[4].MixedRealityInputAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, Interactions[5].MixedRealityInputAction);
                        isManipulating = false;
                    }
                }
                else if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, Interactions[4].MixedRealityInputAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, Interactions[5].MixedRealityInputAction);
                        isManipulating = false;
                    }

                    MixedRealityToolkit.InputSystem?.RaisePointerClicked(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);
                }

                if (isHolding)
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, Interactions[4].MixedRealityInputAction);
                    isHolding = false;
                }

                if (isManipulating)
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, Interactions[5].MixedRealityInputAction, TouchData.deltaPosition);
                    isManipulating = false;
                }
            }

            if (isHolding)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, Interactions[4].MixedRealityInputAction);
                isHolding = false;
            }

            Debug.Assert(!isHolding);

            if (isManipulating)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, Interactions[5].MixedRealityInputAction, TouchData.deltaPosition);
                isManipulating = false;
            }

            Debug.Assert(!isManipulating);

            MixedRealityToolkit.InputSystem?.RaisePointerUp(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);

            Lifetime = 0.0f;
            isTouched = false;
            Interactions[0].Vector2Data = Vector2.zero;
            Interactions[1].PoseData = MixedRealityPose.ZeroIdentity;
        }
    }
}