// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand data.
    /// </summary>
    public class HandData
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. DateTime.UtcNow.ToFileTime().
        /// </summary>
        public long Timestamp { get; set; } = 0;

        /// <summary>
        /// Is the hand currently being tracked by the system?
        /// </summary>
        public bool IsTracked { get; set; } = false;

        /// <summary>
        /// Pose information for each hand joint.
        /// </summary>
        public MixedRealityPose[] Joints { get; } = new MixedRealityPose[jointCount];

        public void Copy(HandData other)
        {
            Timestamp = other.Timestamp;
            IsTracked = other.IsTracked;

            for (int i = 0; i < jointCount; ++i)
            {
                Joints[i] = other.Joints[i];
            }
        }
    }
}