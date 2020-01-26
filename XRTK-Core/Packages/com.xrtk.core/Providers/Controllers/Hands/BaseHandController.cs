// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
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
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly Dictionary<TrackedHandBounds, Bounds> bounds = new Dictionary<TrackedHandBounds, Bounds>();

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
        public IReadOnlyDictionary<TrackedHandJoint, MixedRealityPose> JointPoses => jointPoses;

        /// <inheritdoc />
        public IReadOnlyDictionary<TrackedHandBounds, Bounds> Bounds => bounds;

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

            UpdateInteractions();
            UpdateJoints(handData);
            UpdateBounds();
            UpdateVelocity();

            MixedRealityToolkit.InputSystem.RaiseHandDataInputChanged(InputSource, ControllerHandedness, handData);
        }

        private void UpdateInteractions()
        {
            for (int i = 0; i < Interactions?.Length; i++)
            {
                UpdateInteraction(Interactions[i]);
            }
        }

        /// <summary>
        /// If needed, update the input from the device for an interaction configured.
        /// </summary>
        /// <param name="interactionMapping">Interaction mapping to update.</param>
        protected virtual void UpdateInteraction(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ButtonPress:
                    interactionMapping.BoolData = Input.GetKey(interactionMapping.KeyCode);
                    break;
                case DeviceInputType.PointerPosition:
                    interactionMapping.PositionData = Input.mousePosition;
                    break;
            }

            interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
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

        /// <summary>
        /// Updates the controller's axis aligned bounds using provided hand data.
        /// </summary>
        private void UpdateBounds()
        {
            // TrackedHandBounds.Hand
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
                    bounds[TrackedHandBounds.Hand] = newHandBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.Hand, newHandBounds);
                }
            }

            // TrackedHandBounds.IndexFinger
            if (TryGetJointPose(TrackedHandJoint.IndexKnuckle, out MixedRealityPose indexKnucklePose)
                && TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, out MixedRealityPose indexMiddlePose))
            {
                Bounds newIndexFingerBounds = new Bounds(indexKnucklePose.Position, Vector3.zero);
                newIndexFingerBounds.Encapsulate(indexMiddlePose.Position);

                if (bounds.ContainsKey(TrackedHandBounds.IndexFinger))
                {
                    bounds[TrackedHandBounds.IndexFinger] = newIndexFingerBounds;
                }
                else
                {
                    bounds.Add(TrackedHandBounds.IndexFinger, newIndexFingerBounds);
                }
            }
        }

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
        public bool TryGetBounds(TrackedHandBounds handBounds, out Bounds? bounds)
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
        public virtual bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        private Vector3 GetJointPosition(TrackedHandJoint joint)
        {
            return TryGetJointPose(joint, out MixedRealityPose pose) ? pose.Position : Vector3.zero;
        }
    }
}