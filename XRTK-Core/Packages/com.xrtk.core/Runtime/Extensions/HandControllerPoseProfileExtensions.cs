// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HandControllerPoseProfile"/>
    /// </summary>
    public static class HandControllerPoseProfileExtensions
    {
        /// <summary>
        /// Converts a pose definition into <see cref="HandData"/> representing
        /// the pose.
        /// </summary>
        /// <param name="pose">The pose to convert.</param>
        /// <returns><see cref="HandData"/> object for the pose.</returns>
        public static HandData ToHandData(this HandControllerPoseProfile pose)
        {
            var rootPose = new MixedRealityPose(Vector3.zero, Quaternion.identity);
            var recordedHandJoints = JsonUtility.FromJson<RecordedHandJoints>(pose.Data.text);
            var jointPoses = new MixedRealityPose[HandData.JointCount];

            for (int j = 0; j < recordedHandJoints.Joints.Length; j++)
            {
                var jointRecord = recordedHandJoints.Joints[j];
                jointPoses[(int)jointRecord.Joint] = jointRecord.Pose;
            }

            var handData = new HandData(rootPose, jointPoses);
            handData.IsGripping = pose.IsGripping;
            handData.GripStrength = pose.GripStrength;
            handData.FingerCurlStrengths = new float[pose.FingerCurlStrengths.Length];
            Array.Copy(pose.FingerCurlStrengths, handData.FingerCurlStrengths, pose.FingerCurlStrengths.Length);

            return handData;
        }
    }
}