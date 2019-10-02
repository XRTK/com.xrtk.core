// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using XRTK.Definitions.Utilities;
using XRTK.Utilities;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    /// <summary>
    /// Pose of an hand defined by joint poses for use when simulating hands in editor.
    /// Used by <see cref="UnityEditorHandControllerDataProvider"/> to fake hand tracking.
    /// </summary>
    public class UnityEditorHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
        private static readonly Dictionary<string, UnityEditorHandPose> handPoses = new Dictionary<string, UnityEditorHandPose>();

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        private MixedRealityPose[] localJointPoses;

        /// <summary>
        /// Creates a new hand pose with all joints in their
        /// local space origin.
        /// </summary>
        public UnityEditorHandPose()
        {
            localJointPoses = new MixedRealityPose[jointCount];
            SetZero();
        }

        /// <summary>
        /// Creates a new hand pose using local pose data for the hand's joints.
        /// </summary>
        /// <param name="localJointPoses">Jont poses in local space.</param>
        public UnityEditorHandPose(MixedRealityPose[] localJointPoses)
        {
            this.localJointPoses = new MixedRealityPose[jointCount];
            Array.Copy(localJointPoses, this.localJointPoses, jointCount);
        }

        /// <summary>
        /// Computes world space poses from camera-space joint data.
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
        /// Takes world space joint poses from any hand and convert into right-hand, camera-space poses.
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
        /// Copy data from another unity editor hand pose.
        /// </summary>
        public void Copy(UnityEditorHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, jointCount);
        }

        /// <summary>
        /// Blends between two hand poses.
        /// </summary>
        public void Lerp(UnityEditorHandPose poseA, UnityEditorHandPose poseB, float value)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var p = Vector3.Lerp(poseA.localJointPoses[i].Position, poseB.localJointPoses[i].Position, value);
                var r = Quaternion.Slerp(poseA.localJointPoses[i].Rotation, poseB.localJointPoses[i].Rotation, value);
                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Gets the pose data for a given pose name, if it's registered.
        /// </summary>
        /// <param name="name">The name of the pose.</param>
        /// <param name="pose">Hand pose reference.</param>
        /// <returns>True, if found.</returns>
        public static bool TryGetPoseByName(string name, out UnityEditorHandPose pose)
        {
            if (handPoses.TryGetValue(name, out UnityEditorHandPose p))
            {
                pose = p;
                return true;
            }

            pose = null;
            return false;
        }

        /// <summary>
        /// Initialize pose data for use in editor from files.
        /// </summary>
        /// <param name="poses">List of pose data assets with pose information.</param>
        public static void Initialize(List<UnityEditorHandPoseData> poses)
        {
            for (int i = 0; i < poses.Count; i++)
            {
                InitializePose(poses[i]);
            }
        }

        private static UnityEditorHandPose InitializePose(UnityEditorHandPoseData poseData)
        {
            if (poseData.Data != null)
            {
                UnityEditorHandPose pose = new UnityEditorHandPose();
                pose.FromJson(poseData.Data.text);
                handPoses.Add(poseData.GestureName, pose);

                return pose;
            }

            return null;
        }

        /// <summary>
        /// Serialize pose data to JSON format.
        /// </summary>
        public string ToJson()
        {
            var dict = new HandJointPoseDictionary();
            dict.FromJointPoses(localJointPoses);
            return JsonUtility.ToJson(dict);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            HandJointPoseDictionary dict = JsonUtility.FromJson<HandJointPoseDictionary>(json);
            dict.ToJointPoses(localJointPoses);
        }
    }
}