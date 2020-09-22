// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Simulation.Hands
{
    /// <summary>
    /// Simulated pose of an hand controller defined by recorded joint poses.
    /// </summary>
    public struct SimulatedHandControllerPose : IEqualityComparer
    {
        public SimulatedHandControllerPose(SimulatedHandControllerPose pose)
        {
            Id = pose.Id;
            LocalJointPoses = new MixedRealityPose[HandData.JointCount];
            SetZero();
            Array.Copy(pose.LocalJointPoses, LocalJointPoses, HandData.JointCount);
        }

        /// <summary>
        /// Creates a new hand pose with all joints in their
        /// local space origin.
        /// </summary>
        /// <param name="id">Unique pose identifier.</param>
        public SimulatedHandControllerPose(string id)
        {
            Id = id;
            LocalJointPoses = new MixedRealityPose[HandData.JointCount];
            SetZero();
        }

        private static readonly Dictionary<string, SimulatedHandControllerPose> HandPoses = new Dictionary<string, SimulatedHandControllerPose>();

        private static bool isInitialized = false;

        /// <summary>
        /// Gets the configured default pose for simulation hands.
        /// </summary>
        public static HandControllerPoseProfile DefaultHandPose { get; private set; }

        /// <summary>
        /// Gets the unique identifier for the simulated pose.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        public MixedRealityPose[] LocalJointPoses { get; private set; }

        public static bool operator ==(SimulatedHandControllerPose left, SimulatedHandControllerPose right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimulatedHandControllerPose left, SimulatedHandControllerPose right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToJson();
        }

        /// <summary>
        /// Initialize pose data for use in editor from files.
        /// </summary>
        /// <param name="poses">List of pose data assets with pose information.</param>
        public static void Initialize(IReadOnlyList<HandControllerPoseProfile> poses)
        {
            if (isInitialized)
            {
                return;
            }

            // To stabilize the simulated hand poses, we look
            // for the hand "open" pose, which we will use as a reference to offset
            // all other poses, so the "Palm" joint position stays the same for all simulated
            // poses. If no open pose is defined, we can't do anything and keep everything as it is.
            SimulatedHandControllerPose openPose = default;

            foreach (var poseData in poses)
            {
                if (poseData.Id.Equals("open"))
                {
                    openPose = new SimulatedHandControllerPose(poseData.Id);
                    openPose.FromJson(poseData.Data.text);
                    HandPoses.Add(openPose.Id, openPose);
                    //break;
                }
            }

            foreach (var poseData in poses)
            {
                if (poseData.Data != null)
                {
                    if (openPose != default)
                    {
                        // If we already found the open pose, we don't want it initialized again, since we took
                        // care of that above.
                        if (!poseData.Id.Equals("open"))
                        {
                            // We have open pose data, offset this pose using it's palm position.
                            var pose = new SimulatedHandControllerPose(poseData.Id);
                            pose.FromJson(poseData.Data.text);
                            OffsetJointsRelativeToOpenPosePalmPosition(openPose, pose);
                            HandPoses.Add(pose.Id, pose);
                        }
                    }
                    else
                    {
                        var pose = new SimulatedHandControllerPose(poseData.Id);
                        pose.FromJson(poseData.Data.text);
                        HandPoses.Add(pose.Id, pose);
                    }
                }

                if (poseData.IsDefault)
                {
                    DefaultHandPose = poseData;
                }
            }

            isInitialized = true;
        }

        private static void OffsetJointsRelativeToOpenPosePalmPosition(SimulatedHandControllerPose openPose, SimulatedHandControllerPose pose)
        {
            Vector3 openHandPalmPosition = openPose.LocalJointPoses[(int)TrackedHandJoint.Palm].Position;
            Vector3 posePalmPosition = pose.LocalJointPoses[(int)TrackedHandJoint.Palm].Position;
            Vector3 offset = posePalmPosition - openHandPalmPosition;

            for (int i = 0; i < pose.LocalJointPoses.Length; i++)
            {
                pose.LocalJointPoses[i].Position -= offset;
            }
        }

        /// <summary>
        /// Set all poses to zero.
        /// </summary>
        public void SetZero()
        {
            for (int i = 0; i < LocalJointPoses.Length; i++)
            {
                LocalJointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
        }

        /// <summary>
        /// Interpolates between the poses a and b by the interpolant t.
        /// </summary>
        /// <param name="interpolatedPose"></param>
        /// <param name="a">Pose at t = 0.</param>
        /// <param name="b">Pose at t = 1.</param>
        /// <param name="t">The parameter t is clamped to the range [0,1].</param>
        public static void Lerp(ref SimulatedHandControllerPose interpolatedPose, SimulatedHandControllerPose a, SimulatedHandControllerPose b, float t)
        {
            for (int i = 0; i < interpolatedPose.LocalJointPoses.Length; i++)
            {
                var jointPoseA = a.LocalJointPoses[i];
                var jointPoseB = b.LocalJointPoses[i];

                var position = Vector3.Lerp(jointPoseA.Position, jointPoseB.Position, t);
                var rotation = Quaternion.Slerp(jointPoseA.Rotation, jointPoseB.Rotation, t);

                interpolatedPose.LocalJointPoses[i] = new MixedRealityPose(position, rotation);
            }

            interpolatedPose.Id = t >= 1 ? b.Id : a.Id;
        }

        /// <summary>
        /// Gets the pose for a given pose name, if it's registered.
        /// </summary>
        /// <param name="name">The name of the pose.</param>
        /// <param name="result">Hand pose reference.</param>
        /// <returns>True, if found.</returns>
        public static bool TryGetPoseByName(string name, out SimulatedHandControllerPose result)
        {
            if (HandPoses.TryGetValue(name, out var pose))
            {
                result = pose;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets the pose for a given pose name. Will throw it pose not registered.
        /// </summary>
        /// <param name="name">The name of the pose to lookup.</param>
        /// <returns>Simulated hand pose.</returns>
        public static SimulatedHandControllerPose GetPoseByName(string name)
        {
            return HandPoses[name];
        }

        /// <summary>
        /// Serialize pose data to JSON format.
        /// </summary>
        public string ToJson()
        {
            var record = new RecordedHandJoints
            {
                Joints = new RecordedHandJoint[LocalJointPoses.Length]
            };

            for (int i = 0; i < LocalJointPoses.Length; i++)
            {
                record.Joints[i] = new RecordedHandJoint((TrackedHandJoint)i, LocalJointPoses[i]);
            }

            return JsonUtility.ToJson(record);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            var record = JsonUtility.FromJson<RecordedHandJoints>(json);

            for (int i = 0; i < record.Joints.Length; i++)
            {
                var jointRecord = record.Joints[i];
                LocalJointPoses[(int)jointRecord.Joint] = jointRecord.Pose;
            }
        }

        #region IEqualityComparer Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            if (left is null || right is null) { return false; }
            if (!(left is SimulatedHandControllerPose) || !(right is SimulatedHandControllerPose)) { return false; }
            return ((SimulatedHandControllerPose)left).Equals((SimulatedHandControllerPose)right);
        }

        /// <summary>Determines whether the specified object is equal to this instance.</summary>
        /// <param name="other">The specified object.</param>
        /// <returns>True, if the specified object is equal to this instance, otherwise false.</returns>
        public bool Equals(SimulatedHandControllerPose other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return !(obj is null) && (obj is SimulatedHandControllerPose pose && Equals(pose));
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is SimulatedHandControllerPose pose ? pose.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion IEqualityComparer Implementation
    }
}