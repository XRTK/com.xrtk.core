// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation with shared / common elements between all supported hand controllers.
    /// </summary>
    public abstract class BaseHandController : BaseController, IMixedRealityHandController
    {
        protected BaseHandController() : base() { }

        /// <inheritdoc />
        protected BaseHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        { }

        private const float NEW_VELOCITY_WEIGHT = .2f;
        private const float CURRENT_VELOCITY_WEIGHT = .8f;
        private const float INPUT_DOWN_POSE_EPSILON = .02f;
        private const int INPUT_DOWN_FRAME_BUFFER_SIZE = 5;

        private readonly int velocityUpdateFrameInterval = 9;
        private readonly Dictionary<TrackedHandBounds, Bounds[]> bounds = new Dictionary<TrackedHandBounds, Bounds[]>();
        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly Queue<bool> inputDownBuffer = new Queue<bool>(INPUT_DOWN_FRAME_BUFFER_SIZE);

        private int velocityUpdateFrame = 0;
        private float deltaTimeStart;

        private Vector3 lastPalmNormal;
        private Vector3 lastPalmPosition;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping("Spatial Pointer Pose", AxisType.SixDof, DeviceInputType.SpatialPointer)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <summary>
        /// Gets the current palm normal of the hand controller.
        /// </summary>
        protected Vector3 PalmNormal => TryGetJointPose(TrackedHandJoint.Palm, out var pose) ? -pose.Up : Vector3.zero;

        /// <inheritdoc />
        public bool IsInInputDownPose { get; private set; }

        public void UpdateController(HandData handData)
        {
            if (!Enabled) { return; }

            var lastState = TrackingState;
            var lastIsInInputDownPose = IsInInputDownPose;

            UpdateJoints(handData);
            UpdateBounds();
            UpdateVelocity();
            UpdateIsInputDownPose();

            Debug.Log(handData.PoseId);

            if (TryGetJointPose(TrackedHandJoint.Wrist, out var wristPose))
            {
                IsPositionAvailable = true;
                IsPositionApproximate = false;
                IsRotationAvailable = true;
                TrackingState = handData.IsTracked ? TrackingState.Tracked : TrackingState.NotTracked;
            }

            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, wristPose);
            }

            if (lastIsInInputDownPose != IsInInputDownPose)
            {
                if (IsInInputDownPose && !lastIsInInputDownPose)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, MixedRealityInputAction.None);
                }
                else if (!IsInInputDownPose && lastIsInInputDownPose)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, MixedRealityInputAction.None);
                }
            }

            if (IsInInputDownPose)
            {
                MixedRealityToolkit.InputSystem?.RaiseOnInputPressed(InputSource, MixedRealityInputAction.None);
            }

            MixedRealityToolkit.InputSystem?.RaiseHandDataInputChanged(InputSource, ControllerHandedness, handData);
        }

        /// <summary>
        /// Updates the controller's joint poses using provided hand data.
        /// </summary>
        /// <param name="handData">The updated hand data for this controller.</param>
        private void UpdateJoints(HandData handData)
        {
            for (int i = 0; i < HandData.JointCount; i++)
            {
                var handJoint = (TrackedHandJoint)i;

                if (TryGetJointPose(handJoint, out _))
                {
                    jointPoses[handJoint] = handData.Joints[i];
                }
                else
                {
                    jointPoses.Add(handJoint, handData.Joints[i]);
                }
            }
        }

        #region Hand Bounds Implementation

        private void UpdateBounds()
        {
            var handControllerDataProvider = (IMixedRealityHandControllerDataProvider)ControllerDataProvider;

            if (handControllerDataProvider.HandPhysicsEnabled && handControllerDataProvider.BoundsMode == HandBoundsMode.Hand)
            {
                UpdateHandBounds();
            }
            else if (handControllerDataProvider.HandPhysicsEnabled && handControllerDataProvider.BoundsMode == HandBoundsMode.Fingers)
            {
                UpdatePalmBounds();
                UpdateThumbBounds();
                UpdateIndexFingerBounds();
                UpdateMiddleFingerBounds();
                UpdateRingFingerBounds();
                UpdatePinkyFingerBounds();
            }
        }

        private void UpdatePalmBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.PinkyMetacarpal, out var pinkyMetacarpalPose) &&
                TryGetJointPose(TrackedHandJoint.PinkyKnuckle, out var pinkyKnucklePose) &&
                TryGetJointPose(TrackedHandJoint.RingMetacarpal, out var ringMetacarpalPose) &&
                TryGetJointPose(TrackedHandJoint.RingKnuckle, out var ringKnucklePose) &&
                TryGetJointPose(TrackedHandJoint.MiddleMetacarpal, out var middleMetacarpalPose) &&
                TryGetJointPose(TrackedHandJoint.MiddleKnuckle, out var middleKnucklePose) &&
                TryGetJointPose(TrackedHandJoint.IndexMetacarpal, out var indexMetacarpalPose) &&
                TryGetJointPose(TrackedHandJoint.IndexKnuckle, out var indexKnucklePose))
            {
                // Palm bounds are a composite of each finger's metacarpal -> knuckle joint bounds.
                // Excluding the thumb here.
                var palmBounds = new Bounds[4];

                // Index
                var indexPalmBounds = new Bounds(indexMetacarpalPose.Position, Vector3.zero);
                indexPalmBounds.Encapsulate(indexKnucklePose.Position);
                palmBounds[0] = indexPalmBounds;

                // Middle
                var middlePalmBounds = new Bounds(middleMetacarpalPose.Position, Vector3.zero);
                middlePalmBounds.Encapsulate(middleKnucklePose.Position);
                palmBounds[1] = middlePalmBounds;

                // Ring
                var ringPalmBounds = new Bounds(ringMetacarpalPose.Position, Vector3.zero);
                ringPalmBounds.Encapsulate(ringKnucklePose.Position);
                palmBounds[2] = ringPalmBounds;

                // Pinky
                var pinkyPalmBounds = new Bounds(pinkyMetacarpalPose.Position, Vector3.zero);
                pinkyPalmBounds.Encapsulate(pinkyKnucklePose.Position);
                palmBounds[3] = pinkyPalmBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Palm))
                {
                    bounds[TrackedHandBounds.Palm] = palmBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Palm, palmBounds);
                }
            }
        }

        private void UpdateHandBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.Palm, out var palmPose))
            {
                var newHandBounds = new Bounds(palmPose.Position, Vector3.zero);

                foreach (var kvp in jointPoses)
                {
                    if (kvp.Key == TrackedHandJoint.None ||
                        kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newHandBounds.Encapsulate(kvp.Value.Position);
                }

                if (bounds.ContainsKey(TrackedHandBounds.Hand))
                {
                    bounds[TrackedHandBounds.Hand] = new[] { newHandBounds };
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Hand, new[] { newHandBounds });
                }
            }
        }

        private void UpdateThumbBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.ThumbMetacarpalJoint, out var knucklePose) &&
                TryGetJointPose(TrackedHandJoint.ThumbProximalJoint, out var middlePose) &&
                TryGetJointPose(TrackedHandJoint.ThumbTip, out var tipPose))
            {
                // Thumb bounds include metacarpal -> proximal and proximal -> tip bounds.
                var thumbBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                thumbBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                thumbBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Thumb))
                {
                    bounds[TrackedHandBounds.Thumb] = thumbBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Thumb, thumbBounds);
                }
            }
        }

        private void UpdateIndexFingerBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.IndexKnuckle, out var knucklePose) &&
                TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, out var middlePose) &&
                TryGetJointPose(TrackedHandJoint.IndexTip, out var tipPose))
            {
                // Index finger bounds include knuckle -> middle and middle -> tip bounds.
                var indexFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                indexFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                indexFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.IndexFinger))
                {
                    bounds[TrackedHandBounds.IndexFinger] = indexFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.IndexFinger, indexFingerBounds);
                }
            }
        }

        private void UpdateMiddleFingerBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.MiddleKnuckle, out var knucklePose) &&
                TryGetJointPose(TrackedHandJoint.MiddleMiddleJoint, out var middlePose) &&
                TryGetJointPose(TrackedHandJoint.MiddleTip, out var tipPose))
            {
                // Middle finger bounds include knuckle -> middle and middle -> tip bounds.
                var middleFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                middleFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                middleFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.MiddleFinger))
                {
                    bounds[TrackedHandBounds.MiddleFinger] = middleFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.MiddleFinger, middleFingerBounds);
                }
            }
        }

        private void UpdateRingFingerBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.RingKnuckle, out var knucklePose) &&
                TryGetJointPose(TrackedHandJoint.RingMiddleJoint, out var middlePose) &&
                TryGetJointPose(TrackedHandJoint.RingTip, out var tipPose))
            {
                // Ring finger bounds include knuckle -> middle and middle -> tip bounds.
                var ringFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                ringFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                ringFingerBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.RingFinger))
                {
                    bounds[TrackedHandBounds.RingFinger] = ringFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.RingFinger, ringFingerBounds);
                }
            }
        }

        private void UpdatePinkyFingerBounds()
        {
            if (TryGetJointPose(TrackedHandJoint.PinkyKnuckle, out var knucklePose) &&
                TryGetJointPose(TrackedHandJoint.PinkyMiddleJoint, out var middlePose) &&
                TryGetJointPose(TrackedHandJoint.PinkyTip, out var tipPose))
            {
                // Pinky finger bounds include knuckle -> middle and middle -> tip bounds.
                var pinkyBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                var knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                pinkyBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                var middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
                middleToTipBounds.Encapsulate(tipPose.Position);
                pinkyBounds[1] = middleToTipBounds;

                // Update cached bounds entry.
                if (bounds.ContainsKey(TrackedHandBounds.Pinky))
                {
                    bounds[TrackedHandBounds.Pinky] = pinkyBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Pinky, pinkyBounds);
                }
            }
        }

        #endregion

        /// <summary>
        /// Updates the controller's velocity / angular velocity.
        /// </summary>
        private void UpdateVelocity()
        {
            if (velocityUpdateFrame == 0)
            {
                deltaTimeStart = Time.unscaledTime;
                lastPalmPosition = GetJointPosition(TrackedHandJoint.Palm);
                lastPalmNormal = PalmNormal;
            }
            else if (velocityUpdateFrame == velocityUpdateFrameInterval)
            {
                // Update linear velocity.
                var deltaTime = Time.unscaledTime - deltaTimeStart;
                var newVelocity = (GetJointPosition(TrackedHandJoint.Palm) - lastPalmPosition) / deltaTime;
                Velocity = (Velocity * CURRENT_VELOCITY_WEIGHT) + (newVelocity * NEW_VELOCITY_WEIGHT);

                // Update angular velocity.
                var currentPalmNormal = PalmNormal;
                var rotation = Quaternion.FromToRotation(lastPalmNormal, currentPalmNormal);
                var rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                AngularVelocity = rotationRate / deltaTime;
            }

            velocityUpdateFrame++;
            velocityUpdateFrame = velocityUpdateFrame > velocityUpdateFrameInterval ? 0 : velocityUpdateFrame;
        }

        private void UpdateIsInputDownPose()
        {
            if (TrackingState == TrackingState.Tracked)
            {
                var isInInputDownPoseThisFrame = false;
                if (TryGetJointPose(TrackedHandJoint.ThumbTip, out var thumbTip) &&
                    TryGetJointPose(TrackedHandJoint.IndexTip, out var indexTip))
                {
                    if (Vector3.Distance(thumbTip.Position, indexTip.Position) < INPUT_DOWN_POSE_EPSILON)
                    {
                        isInInputDownPoseThisFrame = true;
                    }
                }

                if (inputDownBuffer.Count < INPUT_DOWN_FRAME_BUFFER_SIZE)
                {
                    inputDownBuffer.Enqueue(isInInputDownPoseThisFrame);
                    IsInInputDownPose = false;
                }
                else
                {
                    inputDownBuffer.Dequeue();
                    inputDownBuffer.Enqueue(isInInputDownPoseThisFrame);

                    isInInputDownPoseThisFrame = true;
                    for (int i = 0; i < inputDownBuffer.Count; i++)
                    {
                        var value = inputDownBuffer.Dequeue();

                        if (!value)
                        {
                            isInInputDownPoseThisFrame = false;
                        }

                        inputDownBuffer.Enqueue(value);
                    }

                    IsInInputDownPose = isInInputDownPoseThisFrame;
                }
            }
            else
            {
                inputDownBuffer.Clear();
                IsInInputDownPose = false;
            }
        }

        /// <inheritdoc />
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] newBounds)
        {
            if (bounds.ContainsKey(handBounds))
            {
                newBounds = bounds[handBounds];
                return true;
            }

            newBounds = null;
            return false;
        }

        /// <inheritdoc />
        public virtual bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose) => jointPoses.TryGetValue(joint, out pose);

        private Vector3 GetJointPosition(TrackedHandJoint joint) => TryGetJointPose(joint, out var pose) ? pose.Position : Vector3.zero;
    }
}