// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Services;

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
        public static SimulatedHandControllerPoseData DefaultHandPose { get; private set; }

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
            return $"{ToJson()}";
        }

        /// <summary>
        /// Initialize pose data for use in editor from files.
        /// </summary>
        /// <param name="poses">List of pose data assets with pose information.</param>
        public static void Initialize(IReadOnlyList<SimulatedHandControllerPoseData> poses)
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
        /// Computes world space poses from camera-space joint data.
        /// </summary>
        public void ComputeJointPoses(Handedness handedness, Quaternion rotation, Vector3 position, MixedRealityPose[] jointsOut)
        {
            Quaternion cameraRotation = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation;

            for (int i = 0; i < HandData.JointCount; i++)
            {
                // Initialize from local offsets
                var localPosition = LocalJointPoses[i].Position;
                var localRotation = LocalJointPoses[i].Rotation;

                // Pose offset are for right hand, mirror on X axis if left hand is needed
                if (handedness == Handedness.Left)
                {
                    localPosition.x = -localPosition.x;
                    localRotation.y = -localRotation.y;
                    localRotation.z = -localRotation.z;
                }

                // Apply camera transform
                localPosition = cameraRotation * localPosition;
                localRotation = cameraRotation * localRotation;

                // Apply external transform
                localPosition = position + rotation * localPosition;
                localRotation = rotation * localRotation;

                jointsOut[i] = new MixedRealityPose(localPosition, localRotation);
            }
        }

        /// <summary>
        /// Takes world space joint poses from any hand and convert into right-hand, camera-space poses.
        /// </summary>
        public void ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation);

            for (int i = 0; i < HandData.JointCount; i++)
            {
                Vector3 p = joints[i].Position;
                Quaternion r = joints[i].Rotation;

                // Apply inverse external transform
                p = invRotation * (p - position);
                r = invRotation * r;

                // To camera space
                p = invCameraRotation * p;
                r = invCameraRotation * r;

                // Pose offset are for right hand, mirror on X axis if left hand is given
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                LocalJointPoses[i] = new MixedRealityPose(p, r);
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
            var recordings = new List<RecordedHandJoint>();

            for (int i = 0; i < LocalJointPoses.Length; i++)
            {
                recordings.Add(new RecordedHandJoint((TrackedHandJoint)i, LocalJointPoses[i]));
            }

            return JsonUtility.ToJson(recordings);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            var record = JsonUtility.FromJson<RecordedHandJoints>(json);

            for (int i = 0; i < record.items.Length; i++)
            {
                var jointRecord = record.items[i];
                LocalJointPoses[(int)jointRecord.JointIndex] = jointRecord.pose;
            }
        }

        #region IEqualityComparer Implementation

        bool IEqualityComparer.Equals(object left, object right)
        {
            if (left is null || right is null) { return false; }
            if (!(left is SimulatedHandControllerPose) || !(right is SimulatedHandControllerPose)) { return false; }
            return ((SimulatedHandControllerPose)left).Equals((SimulatedHandControllerPose)right);
        }

        public bool Equals(SimulatedHandControllerPose other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return !(obj is null) && (obj is SimulatedHandControllerPose pose && Equals(pose));
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is SimulatedHandControllerPose pose ? pose.GetHashCode() : 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion IEqualityComparer Implementation
    }
}