// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Snapshot of a hand pose in a single frame.
    /// Poses are always stored as right-hand poses in camera space.
    /// </summary>
    public struct HandPoseFrame
    {
        /// <summary>
        /// Constructs a pose frame from local joint poses.
        /// </summary>
        /// <param name="id">Identifier for the frame.</param>
        /// <param name="localJointPoses">Hand joint poses in local space.</param>
        public HandPoseFrame(string id, MixedRealityPose[] localJointPoses)
        {
            Id = id;
            LocalJointPoses = localJointPoses;

            var wristPose = LocalJointPoses[(int)TrackedHandJoint.Wrist];
            var palmPose = LocalJointPoses[(int)TrackedHandJoint.Palm];
            scaleDenominator = Vector3.Distance(wristPose.Position, palmPose.Position);
        }

        /// <summary>
        /// Constructs a pose frame from local joint poses and
        /// assigns a random ID.
        /// </summary>
        /// <param name="localJointPoses">Hand joint poses in local space.</param>
        public HandPoseFrame(MixedRealityPose[] localJointPoses)
            : this(Guid.NewGuid().ToString(), localJointPoses)
        { }

        private readonly float scaleDenominator;

        /// <summary>
        /// Unique identifier for the pose frame.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// </summary>
        public MixedRealityPose[] LocalJointPoses { get; }

        /// <summary>
        /// Compares another hand pose frame and returns true, if they can be considered
        /// equal.
        /// </summary>
        /// <param name="otherFrame">The frame to compare to.</param>
        /// <param name="tolerance">Tolerance for considering joints equal.</param>
        /// <returns></returns>
        public bool Compare(HandPoseFrame otherFrame, float tolerance)
        {
            var otherWristPose = otherFrame.LocalJointPoses[(int)TrackedHandJoint.Wrist];
            var otherPalmPose = otherFrame.LocalJointPoses[(int)TrackedHandJoint.Palm];
            var otherScaleDenominator = Vector3.Distance(otherWristPose.Position, otherPalmPose.Position);
            var scale = otherScaleDenominator / scaleDenominator;

            int matches = 0;

            // Skip None, Wrist and Palm.
            var skippedJoints = 3;
            var requiredMatches = HandData.JointCount - skippedJoints;
            for (int i = skippedJoints; i < LocalJointPoses.Length; i++)
            {
                var thisJoint = LocalJointPoses[i].Position;
                var otherJoint = scale * otherFrame.LocalJointPoses[i].Position;

                var considerEqual =
                    Math.Abs(thisJoint.x - otherJoint.x) <= tolerance &&
                    Math.Abs(thisJoint.y - otherJoint.y) <= tolerance &&
                    Math.Abs(thisJoint.z - otherJoint.z) <= tolerance;

                if (considerEqual)
                {
                    matches++;
                }
            }

            Debug.Log(matches);
            return matches >= requiredMatches;
        }
    }
}