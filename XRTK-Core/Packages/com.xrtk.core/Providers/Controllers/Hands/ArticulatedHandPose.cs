// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Services.InputSystem.Simulation
{
    /// <summary>
    /// Shape of an articulated hand defined by joint poses.
    /// </summary>
    public class ArticulatedHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        private MixedRealityPose[] localJointPoses;

        public ArticulatedHandPose()
        {
            localJointPoses = new MixedRealityPose[jointCount];
            SetZero();
        }

        public ArticulatedHandPose(MixedRealityPose[] _localJointPoses)
        {
            localJointPoses = new MixedRealityPose[jointCount];
            Array.Copy(_localJointPoses, localJointPoses, jointCount);
        }

        /// <summary>
        /// Compute world space poses from camera-space joint data.
        /// </summary>
        public void ComputeJointPoses(Handedness handedness, Quaternion rotation, Vector3 position, MixedRealityPose[] jointsOut)
        {
            var cameraRotation = CameraCache.Main.transform.rotation;

            for (int i = 0; i < jointCount; i++)
            {
                // Initialize from local offsets
                Vector3 p = localJointPoses[i].Position;
                Quaternion r = localJointPoses[i].Rotation;

                // Pose offset are for right hand, mirror on X axis if left hand is needed
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                // Apply camera transform
                p = cameraRotation * p;
                r = cameraRotation * r;

                // Apply external transform
                p = position + rotation * p;
                r = rotation * r;

                jointsOut[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Take world space joint poses from any hand and convert into right-hand, camera-space poses.
        /// </summary>
        public void ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(CameraCache.Main.transform.rotation);

            for (int i = 0; i < jointCount; i++)
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

                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Set all poses to zero.
        /// </summary>
        public void SetZero()
        {
            for (int i = 0; i < jointCount; i++)
            {
                localJointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
        }

        /// <summary>
        /// Copy data from another articulated hand pose.
        /// </summary>
        public void Copy(ArticulatedHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, jointCount);
        }

        /// <summary>
        /// Blend between two hand poses.
        /// </summary>
        public void InterpolateOffsets(ArticulatedHandPose poseA, ArticulatedHandPose poseB, float value)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var p = Vector3.Lerp(poseA.localJointPoses[i].Position, poseB.localJointPoses[i].Position, value);
                var r = Quaternion.Slerp(poseA.localJointPoses[i].Rotation, poseB.localJointPoses[i].Rotation, value);
                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        private static readonly Dictionary<string, ArticulatedHandPose> handPoses = new Dictionary<string, ArticulatedHandPose>();

        /// <summary>
        /// Get pose data for a supported gesture.
        /// </summary>
        public static ArticulatedHandPose GetGesturePose(string name)
        {
            if (handPoses.TryGetValue(name, out ArticulatedHandPose pose))
            {
                return pose;
            }
            return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Load pose data from files.
        /// </summary>
        public static void LoadGesturePoses(List<HandPose> definitions)
        {
            for (int i = 0; i < definitions.Count; i++)
            {
                LoadGesturePose(definitions[i]);
            }
        }

        private static ArticulatedHandPose LoadGesturePose(HandPose definition)
        {
            if (definition.Data != null)
            {
                ArticulatedHandPose pose = new ArticulatedHandPose();
                pose.FromJson(definition.Data.text);
                handPoses.Add(definition.GestureName, pose);

                return pose;
            }

            return null;
        }

#endif

        /// Utility class to serialize hand pose as a dictionary with full joint names
        [Serializable]
        internal struct ArticulatedHandPoseItem
        {
            private static readonly string[] jointNames = Enum.GetNames(typeof(TrackedHandJoint));

            public string joint;
            public MixedRealityPose pose;

            public TrackedHandJoint JointIndex
            {
                get
                {
                    int nameIndex = Array.FindIndex(jointNames, IsJointName);
                    if (nameIndex < 0)
                    {
                        Debug.LogError($"Joint name {joint} not in TrackedHandJoint enum");
                        return TrackedHandJoint.None;
                    }
                    return (TrackedHandJoint)nameIndex;
                }
                set { joint = jointNames[(int)value]; }
            }

            private bool IsJointName(string s)
            {
                return s == joint;
            }

            public ArticulatedHandPoseItem(TrackedHandJoint joint, MixedRealityPose pose)
            {
                this.joint = jointNames[(int)joint];
                this.pose = pose;
            }
        }

        /// Utility class to serialize hand pose as a dictionary with full joint names
        [Serializable]
        internal class ArticulatedHandPoseDictionary
        {
            private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

            public ArticulatedHandPoseItem[] items = null;

            public void FromJointPoses(MixedRealityPose[] jointPoses)
            {
                items = new ArticulatedHandPoseItem[jointCount];
                for (int i = 0; i < jointCount; ++i)
                {
                    items[i].JointIndex = (TrackedHandJoint)i;
                    items[i].pose = jointPoses[i];
                }
            }

            public void ToJointPoses(MixedRealityPose[] jointPoses)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    jointPoses[i] = MixedRealityPose.ZeroIdentity;
                }
                foreach (var item in items)
                {
                    jointPoses[(int)item.JointIndex] = item.pose;
                }
            }
        }

        /// <summary>
        /// Serialize pose data to JSON format.
        /// </summary>
        public string ToJson()
        {
            var dict = new ArticulatedHandPoseDictionary();
            dict.FromJointPoses(localJointPoses);
            return JsonUtility.ToJson(dict);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            var dict = JsonUtility.FromJson<ArticulatedHandPoseDictionary>(json);
            dict.ToJointPoses(localJointPoses);
        }
    }
}