// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Controllers.Hands;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation with shared / common elements between all supported hand controllers.
    /// </summary>
    public abstract class BaseHandController : BaseController, IMixedRealityHandController
    {
        /// <summary>
        /// Controller constructor.
        /// </summary>
        /// <param name="trackingState">The controller's tracking state.</param>
        /// <param name="controllerHandedness">The controller's handedness.</param>
        /// <param name="inputSource">Optional input source of the controller.</param>
        /// <param name="interactions">Optional controller interactions mappings.</param>
        public BaseHandController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            handControllerDataProvider = MixedRealityToolkit.GetService<IMixedRealityHandControllerDataProvider>();
        }

        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly Dictionary<TrackedHandBounds, Bounds[]> bounds = new Dictionary<TrackedHandBounds, Bounds[]>();
        private readonly IMixedRealityHandControllerDataProvider handControllerDataProvider;

        private const float currentVelocityWeight = .8f;
        private const float newVelocityWeight = .2f;
        private float deltaTimeStart;
        private Vector3 lastPalmPosition;
        private Vector3 lastPalmNormal;
        private readonly int velocityUpdateInterval = 9;
        private int frameOn = 0;

        /// <summary>
        /// Gets the total count of joints a hand controller supports.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Gets the current palm normal of the hand controller.
        /// </summary>
        protected Vector3 PalmNormal => TryGetJointPose(TrackedHandJoint.Palm, out MixedRealityPose pose) ? -pose.Up : Vector3.zero;

        /// <inheritdoc />
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            base.UpdateController();
            UpdateInteractions();
        }

        /// <inheritdoc />
        public void UpdateController(HandData handData)
        {
            if (!Enabled) { return; }

            TrackingState lastState = TrackingState;

            UpdateJoints(handData);
            UpdateBounds();
            UpdateVelocity();

            MixedRealityPose wristPose;
            if (TryGetJointPose(TrackedHandJoint.Wrist, out wristPose))
            {
                IsPositionAvailable = true;
                IsPositionApproximate = false;
                IsRotationAvailable = true;
                TrackingState = IsPositionAvailable && IsRotationAvailable ? TrackingState.Tracked : TrackingState.NotTracked;
                MixedRealityToolkit.InputSystem.RaiseSourcePoseChanged(InputSource, this, wristPose);
            }

            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, wristPose);
            }

            MixedRealityToolkit.InputSystem.RaiseHandDataInputChanged(InputSource, ControllerHandedness, handData);

            UpdateInteractions();
        }

        /// <summary>
        /// Updates interaction mappings on the controller.
        /// </summary>
        protected virtual void UpdateInteractions()
        {
            for (int i = 0; i < Interactions?.Length; i++)
            {
                MixedRealityInteractionMapping interactionMapping = Interactions[i];
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.PointerPosition:
                        interactionMapping.PoseData = new MixedRealityPose(Input.mousePosition);
                        interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the controller's joint poses using provided hand data.
        /// </summary>
        /// <param name="handData">The updated hand data for this controller.</param>
        private void UpdateJoints(HandData handData)
        {
            for (int i = 0; i < JointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;
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
            if (TryGetJointPose(TrackedHandJoint.PinkyMetacarpal, out MixedRealityPose pinkyMetacarpalPose)
                && TryGetJointPose(TrackedHandJoint.PinkyKnuckle, out MixedRealityPose pinkyKnucklePose)
                && TryGetJointPose(TrackedHandJoint.RingMetacarpal, out MixedRealityPose ringMetacarpalPose)
                && TryGetJointPose(TrackedHandJoint.RingKnuckle, out MixedRealityPose ringKnucklePose)
                && TryGetJointPose(TrackedHandJoint.MiddleMetacarpal, out MixedRealityPose middleMetacarpalPose)
                && TryGetJointPose(TrackedHandJoint.MiddleKnuckle, out MixedRealityPose middleKnucklePose)
                && TryGetJointPose(TrackedHandJoint.IndexMetacarpal, out MixedRealityPose indexMetacarpalPose)
                && TryGetJointPose(TrackedHandJoint.IndexKnuckle, out MixedRealityPose indexKnucklePose))
            {
                // Palm bounds are a composite of each finger's metacarpal -> knuckle joint bounds.
                // Excluding the thumb here.
                Bounds[] palmBounds = new Bounds[4];

                // Index
                Bounds indexPalmBounds = new Bounds(indexMetacarpalPose.Position, Vector3.zero);
                indexPalmBounds.Encapsulate(indexKnucklePose.Position);
                palmBounds[0] = indexPalmBounds;

                // Middle
                Bounds middlePalmBounds = new Bounds(middleMetacarpalPose.Position, Vector3.zero);
                middlePalmBounds.Encapsulate(middleKnucklePose.Position);
                palmBounds[1] = middlePalmBounds;

                // Ring
                Bounds ringPalmBounds = new Bounds(ringMetacarpalPose.Position, Vector3.zero);
                ringPalmBounds.Encapsulate(ringKnucklePose.Position);
                palmBounds[2] = ringPalmBounds;

                // Pinky
                Bounds pinkyPalmBounds = new Bounds(pinkyMetacarpalPose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.Palm, out MixedRealityPose palmPose))
            {
                Bounds newHandBounds = new Bounds(palmPose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.ThumbMetacarpalJoint, out MixedRealityPose knucklePose)
                && TryGetJointPose(TrackedHandJoint.ThumbProximalJoint, out MixedRealityPose middlePose)
                && TryGetJointPose(TrackedHandJoint.ThumbTip, out MixedRealityPose tipPose))
            {
                // Thumb bounds include metacarpal -> proximal and proximal -> tip bounds.
                Bounds[] thumbBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                Bounds knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                thumbBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                Bounds middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.IndexKnuckle, out MixedRealityPose knucklePose)
                && TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, out MixedRealityPose middlePose)
                && TryGetJointPose(TrackedHandJoint.IndexTip, out MixedRealityPose tipPose))
            {
                // Index finger bounds include knuckle -> middle and middle -> tip bounds.
                Bounds[] indexFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                Bounds knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                indexFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                Bounds middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.MiddleKnuckle, out MixedRealityPose knucklePose)
                && TryGetJointPose(TrackedHandJoint.MiddleMiddleJoint, out MixedRealityPose middlePose)
                && TryGetJointPose(TrackedHandJoint.MiddleTip, out MixedRealityPose tipPose))
            {
                // Middle finger bounds include knuckle -> middle and middle -> tip bounds.
                Bounds[] middleFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                Bounds knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                middleFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                Bounds middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.RingKnuckle, out MixedRealityPose knucklePose)
                && TryGetJointPose(TrackedHandJoint.RingMiddleJoint, out MixedRealityPose middlePose)
                && TryGetJointPose(TrackedHandJoint.RingTip, out MixedRealityPose tipPose))
            {
                // Ring finger bounds include knuckle -> middle and middle -> tip bounds.
                Bounds[] ringFingerBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                Bounds knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                ringFingerBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                Bounds middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
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
            if (TryGetJointPose(TrackedHandJoint.PinkyKnuckle, out MixedRealityPose knucklePose)
                && TryGetJointPose(TrackedHandJoint.PinkyMiddleJoint, out MixedRealityPose middlePose)
                && TryGetJointPose(TrackedHandJoint.PinkyTip, out MixedRealityPose tipPose))
            {
                // Pinky finger bounds include knuckle -> middle and middle -> tip bounds.
                Bounds[] pinkyBounds = new Bounds[2];

                // Knuckle to middle joint bounds.
                Bounds knuckleToMiddleBounds = new Bounds(knucklePose.Position, Vector3.zero);
                knuckleToMiddleBounds.Encapsulate(middlePose.Position);
                pinkyBounds[0] = knuckleToMiddleBounds;

                // Middle to tip joint bounds.
                Bounds middleToTipBounds = new Bounds(middlePose.Position, Vector3.zero);
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
            if (frameOn == 0)
            {
                deltaTimeStart = Time.unscaledTime;
                lastPalmPosition = GetJointPosition(TrackedHandJoint.Palm);
                lastPalmNormal = PalmNormal;
            }
            else if (frameOn == velocityUpdateInterval)
            {
                // Update linear velocity.
                float deltaTime = Time.unscaledTime - deltaTimeStart;
                Vector3 newVelocity = (GetJointPosition(TrackedHandJoint.Palm) - lastPalmPosition) / deltaTime;
                Velocity = (Velocity * currentVelocityWeight) + (newVelocity * newVelocityWeight);

                // Update angular velocity.
                Vector3 currentPalmNormal = PalmNormal;
                Quaternion rotation = Quaternion.FromToRotation(lastPalmNormal, currentPalmNormal);
                Vector3 rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                AngularVelocity = rotationRate / deltaTime;
            }

            frameOn++;
            frameOn = frameOn > velocityUpdateInterval ? 0 : frameOn;
        }

        /// <inheritdoc />
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] bounds)
        {
            if (this.bounds.ContainsKey(handBounds))
            {
                bounds = this.bounds[handBounds];
                return true;
            }

            bounds = null;
            return false;
        }

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness) { }

        /// <inheritdoc />
        public virtual bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose) => jointPoses.TryGetValue(joint, out pose);

        private Vector3 GetJointPosition(TrackedHandJoint joint) => TryGetJointPose(joint, out MixedRealityPose pose) ? pose.Position : Vector3.zero;
    }
}