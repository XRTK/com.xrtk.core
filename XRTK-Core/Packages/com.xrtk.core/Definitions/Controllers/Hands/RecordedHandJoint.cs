// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// A single recorded hand joint's information that may be used to restore the joint pose for simulation.
    /// </summary>
    [Serializable]
    public struct RecordedHandJoint
    {
        /// <summary>
        /// Constructs a new joint record.
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="pose"></param>
        public RecordedHandJoint(TrackedHandJoint joint, MixedRealityPose pose)
        {
            this.joint = JointNames[(int)joint];
            this.pose = pose;
        }

        private static readonly string[] JointNames = Enum.GetNames(typeof(TrackedHandJoint));

        /// <summary>
        /// Name of the joint recorded. Rather use <see cref="JointIndex"/> below for convenience.
        /// </summary>
        public string joint;

        /// <summary>
        /// The recorded pose.
        /// </summary>
        public MixedRealityPose pose;

        /// <summary>
        /// Gets the <see cref="TrackedHandJoint"/> this record represents.
        /// </summary>
        public TrackedHandJoint JointIndex
        {
            get
            {
                int nameIndex = Array.FindIndex(JointNames, IsJointName);

                if (nameIndex < 0)
                {
                    Debug.LogError($"Joint name {joint} not in {nameof(TrackedHandJoint)} enum");
                    return TrackedHandJoint.None;
                }

                return (TrackedHandJoint)nameIndex;
            }
            set => joint = JointNames[(int)value];
        }

        private bool IsJointName(string s)
        {
            return string.Equals(s, joint);
        }
    }
}