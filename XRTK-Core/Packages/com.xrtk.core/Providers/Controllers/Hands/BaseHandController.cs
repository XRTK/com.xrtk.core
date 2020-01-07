// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
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

        private const float currentVelocityWeight = .8f;
        private const float newVelocityWeight = .2f;
        private float deltaTimeStart;
        private Vector3 lastPosition;
        private Vector3 lastPalmNormal;
        private readonly int velocityUpdateInterval = 9;
        private int frameOn = 0;
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly Dictionary<TrackedHandBounds, Bounds> bounds = new Dictionary<TrackedHandBounds, Bounds>();

        /// <summary>
        /// Gets the total joint count supported by this hand controller.
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

        /// <summary>
        /// The Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override void UpdateController()
        {
            base.UpdateController();
        }

        /// <summary>
        /// Updates the state of the hand controller using provided hand data.
        /// </summary>
        /// <param name="handData">Updated hand data for this controller.</param>
        protected virtual void UpdateBase(HandData handData)
        {
            UpdateJoints(handData);
            UpdateBounds(handData);
            UpdateVelocity();

            MixedRealityToolkit.InputSystem.RaiseHandDataInputChanged(InputSource, ControllerHandedness, handData);
        }

        /// <summary>
        /// Updates the controller's joint poses using provided hand data.
        /// </summary>
        /// <param name="handData">The updated hand data for this controller.</param>
        protected virtual void UpdateJoints(HandData handData)
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
        /// <param name="handData">The updated hand data for this controller.</param>
        protected virtual void UpdateBounds(HandData handData)
        {
            IReadOnlyDictionary<TrackedHandJoint, MixedRealityPose> jointPoses = HandUtilities.ToJointPoseDictionary(handData.Joints);

            // TrackedHandBounds.Hand
            if (jointPoses.TryGetValue(TrackedHandJoint.Palm, out MixedRealityPose palmPose))
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
            if (JointPoses.TryGetValue(TrackedHandJoint.IndexKnuckle, out MixedRealityPose indexKnucklePose)
                && JointPoses.TryGetValue(TrackedHandJoint.IndexMiddleJoint, out MixedRealityPose indexMiddlePose))
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
        protected void UpdateVelocity()
        {
            if (frameOn == 0)
            {
                deltaTimeStart = Time.unscaledTime;
                lastPosition = GetJointPosition(TrackedHandJoint.Palm);
                lastPalmNormal = PalmNormal;
            }
            else if (frameOn == velocityUpdateInterval)
            {
                //update linear velocity
                float deltaTime = Time.unscaledTime - deltaTimeStart;
                Vector3 newVelocity = (GetJointPosition(TrackedHandJoint.Palm) - lastPosition) / deltaTime;
                Velocity = (Velocity * currentVelocityWeight) + (newVelocity * newVelocityWeight);

                //update angular velocity
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
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

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