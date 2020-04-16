// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
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
        /// Unique identifier for the pose frame.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// </summary>
        public MixedRealityPose[] LocalJointPoses { get; }

        /// <summary>
        /// Constructs a pose frame from local joint poses.
        /// </summary>
        /// <param name="id">Identifier for the frame.</param>
        /// <param name="localJointPoses">Hand joint poses in local space.</param>
        public HandPoseFrame(string id, MixedRealityPose[] localJointPoses)
        {
            Id = id;
            LocalJointPoses = localJointPoses;
        }

        /// <summary>
        /// Constructs a pose frame from local joint poses and
        /// assigns a random ID.
        /// </summary>
        /// <param name="localJointPoses">Hand joint poses in local space.</param>
        public HandPoseFrame(MixedRealityPose[] localJointPoses)
            : this(Guid.NewGuid().ToString(), localJointPoses)
        { }

        public bool Compare(HandPoseFrame otherFrame, float tolerance)
        {
            for (int i = 0; i < LocalJointPoses.Length - 1; i++)
            {
                var a = LocalJointPoses[i];
                var b = LocalJointPoses[i + 1];
                var diff = Vector3.Distance(a.Position, b.Position);

                var aOther = otherFrame.LocalJointPoses[i];
                var bOther = otherFrame.LocalJointPoses[i + 1];
                var diffOther = Vector3.Distance(aOther.Position, bOther.Position);

                if (Math.Abs(diff - diffOther) >= tolerance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}