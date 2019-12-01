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

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base implementation with shared / common elements between all supported hand controllers.
    /// </summary>
    public abstract class BaseHandController : BaseController, IMixedRealityHandController
    {
        private const float currentVelocityWeight = .8f;
        private const float newVelocityWeight = .2f;

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        // Velocity internal states
        private float deltaTimeStart;
        private Vector3 lastPosition;
        private Vector3 lastPalmNormal;
        private readonly int velocityUpdateInterval = 9;
        private int frameOn = 0;

        // Hand ray
        protected HandRay HandRay { get; } = new HandRay();

        /// <inheritdoc />
        public bool IsTracked => TrackingState == TrackingState.Tracked;

        /// <inheritdoc />
        public bool IsInPointingPose => HandRay.ShouldShowRay;

        /// <summary>
        /// Gets the total joint count supported by this hand controller.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        protected Vector3 PalmNormal
        {
            get
            {
                if (TryGetJointPose(TrackedHandJoint.Palm, out MixedRealityPose pose))
                {
                    return -pose.Up;
                }
                return Vector3.zero;
            }
        }

        /// <inheritdoc />
        public Bounds Bounds { get; private set; }

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public BaseHandController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        public virtual void UpdateState(HandData handData)
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

            UpdateBounds(handData);
            UpdateVelocity();
        }

        private void UpdateBounds(HandData handData)
        {
            MixedRealityPose palmPose;
            IReadOnlyDictionary<TrackedHandJoint, MixedRealityPose> jointPoses = HandUtils.ToJointPoseDictionary(handData.Joints);

            if (jointPoses.TryGetValue(TrackedHandJoint.Palm, out palmPose))
            {
                Bounds newBounds = new Bounds(palmPose.Position, Vector3.zero);
                foreach (var kvp in jointPoses)
                {
                    if (kvp.Key == TrackedHandJoint.None ||
                        kvp.Key == TrackedHandJoint.Palm)
                    {
                        continue;
                    }

                    newBounds.Encapsulate(kvp.Value.Position);
                }

                Bounds = newBounds;
            }
        }

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        #region Protected InputSource Helpers

        #region Gesture Definitions

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
                //currentPalmNormal = new Vector3(0, 1, 0);
                Quaternion rotation = Quaternion.FromToRotation(lastPalmNormal, currentPalmNormal);
                Vector3 rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                AngularVelocity = rotationRate / deltaTime;
            }

            frameOn++;
            frameOn = frameOn > velocityUpdateInterval ? 0 : frameOn;
        }

        #endregion Gesture Definitions

        public virtual bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        private Vector3 GetJointPosition(TrackedHandJoint joint)
        {
            if (TryGetJointPose(joint, out MixedRealityPose pose))
            {
                return pose.Position;
            }

            return Vector3.zero;
        }

        private float DistanceSqrPointToLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            if (lineStart == lineEnd)
            {
                return (point - lineStart).magnitude;
            }

            float lineSegmentMagnitude = (lineEnd - lineStart).magnitude;
            Vector3 ray = (lineEnd - lineStart);
            ray *= (1.0f / lineSegmentMagnitude);
            float dot = Vector3.Dot(point - lineStart, ray);
            if (dot <= 0)
            {
                return (point - lineStart).sqrMagnitude;
            }
            if (dot >= lineSegmentMagnitude)
            {
                return (point - lineEnd).sqrMagnitude;
            }
            return ((lineStart + (ray * dot)) - point).sqrMagnitude;
        }

        #endregion Private InputSource Helpers
    }
}