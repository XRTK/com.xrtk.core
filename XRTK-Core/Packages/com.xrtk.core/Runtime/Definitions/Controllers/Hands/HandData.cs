// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand data.
    /// </summary>
    [Serializable]
    public class HandData
    {
        /// <summary>
        /// Gets the total count of joints the hand data supports.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. <see cref="DateTime.UtcNow"/>
        /// </summary>
        public long TimeStamp { get; set; } = 0;

        /// <summary>
        /// Is the hand currently being tracked by the system?
        /// </summary>
        public bool IsTracked { get; set; } = false;

        /// <summary>
        /// Is the hand currently in a pinch pose?
        /// </summary>
        public bool IsPinching { get; set; }

        /// <summary>
        /// Is the hand currently in a pointing pose?
        /// </summary>
        public bool IsPointing { get; set; }

        /// <summary>
        /// Is the hand currently in a grabbing pose?
        /// </summary>
        public bool IsGrabbing { get; set; }

        /// <summary>
        /// The hand's pointer pose.
        /// </summary>
        public MixedRealityPose PointerPose { get; set; }

        /// <summary>
        /// Recognized hand pose, if any.
        /// Recognizable hand poses are defined in <see cref="BaseHandControllerDataProviderProfile.TrackedPoses"/>.
        /// </summary>
        public HandControllerPoseDefinition TrackedPose { get; set; } = null;

        /// <summary>
        /// Pose information for each hand joint.
        /// </summary>
        public MixedRealityPose[] Joints { get; } = new MixedRealityPose[JointCount];

        /// <summary>
        /// Mesh information of the hand.
        /// </summary>
        public HandMeshData Mesh { get; set; } = new HandMeshData();
    }
}