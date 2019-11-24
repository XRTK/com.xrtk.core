// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands.UnityEditor
{
    /// <summary>
    /// Simulatd pose of an hand defined by joint poses. Used by <see cref="UnityEditorHandControllerDataProvider"/> to simulate hand tracking.
    /// </summary>
    public class SimulatedHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
        private static readonly Dictionary<string, SimulatedHandPose> handPoses = new Dictionary<string, SimulatedHandPose>();

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        private MixedRealityPose[] localJointPoses;

        /// <summary>
        /// Creates a new hand pose with all joints in their
        /// local space origin.
        /// </summary>
        public SimulatedHandPose()
        {
            localJointPoses = new MixedRealityPose[jointCount];
            SetZero();
        }

        /// <summary>
        /// Creates a new hand pose using local pose data for the hand's joints.
        /// </summary>
        /// <param name="localJointPoses">Joint poses in local space.</param>
        public SimulatedHandPose(MixedRealityPose[] localJointPoses)
        {
            this.localJointPoses = new MixedRealityPose[jointCount];
            Array.Copy(localJointPoses, this.localJointPoses, jointCount);
        }

        /// <summary>
        /// Computes world space poses from camera-space joint data.
        /// </summary>
        public void ComputeJointPoses(Handedness handedness, Quaternion rotation, Vector3 position, MixedRealityPose[] jointsOut)
        {
            Quaternion cameraRotation = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation;

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
            var invCameraRotation = Quaternion.Inverse(MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.rotation);

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
        /// Copy data from another simulated hand pose.
        /// </summary>
        public void Copy(SimulatedHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, jointCount);
        }

        /// <summary>
        /// Interpolates between the poses a and b by the interpolant t.
        /// </summary>
        /// <param name="a">Pose at t = 0.</param>
        /// <param name="b">Pose at t = 1.</param>
        /// <param name="t">The parameter t is clamped to the range [0,1].</param>
        public static SimulatedHandPose Lerp(SimulatedHandPose a, SimulatedHandPose b, float t)
        {
            MixedRealityPose[] updatedJointPoses = new MixedRealityPose[jointCount];

            for (int i = 0; i < jointCount; i++)
            {
                MixedRealityPose jointPoseA = a.localJointPoses[i];
                MixedRealityPose jointPoseB = b.localJointPoses[i];

                Vector3 position = Vector3.Lerp(jointPoseA.Position, jointPoseB.Position, t);
                Quaternion rotation = Quaternion.Slerp(jointPoseA.Rotation, jointPoseB.Rotation, t);

                updatedJointPoses[i] = new MixedRealityPose(position, rotation);
            }

            return new SimulatedHandPose(updatedJointPoses);
        }

        /// <summary>
        /// Gets the pose data for a given pose name, if it's registered.
        /// </summary>
        /// <param name="name">The name of the pose.</param>
        /// <param name="pose">Hand pose reference.</param>
        /// <returns>True, if found.</returns>
        public static bool TryGetPoseByName(string name, out SimulatedHandPose pose)
        {
            if (handPoses.TryGetValue(name, out SimulatedHandPose p))
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
        public static void Initialize(IEnumerable<UnityEditorHandPoseData> poses)
        {
            foreach (var poseData in poses)
            {
                if (poseData.Data != null)
                {
                    SimulatedHandPose pose = new SimulatedHandPose();
                    pose.FromJson(poseData.Data.text);
                    handPoses.Add(poseData.GestureName, pose);
                }
            }
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