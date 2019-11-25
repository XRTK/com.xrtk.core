// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Utilities;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Snapshot of hand data.
    /// </summary>
    public class HandData
    {
        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. DateTime.UtcNow.ToFileTime().
        /// </summary>
        public long TimeStamp { get; set; } = 0;

        /// <summary>
        /// Is the hand currently being tracked by the system?
        /// </summary>
        public bool IsTracked { get; set; } = false;

        /// <summary>
        /// Pose information for each hand joint.
        /// </summary>
        public MixedRealityPose[] Joints { get; } = new MixedRealityPose[DefaultHandController.JointCount];

        /// <summary>
        /// Mesh information of the hand.
        /// </summary>
        public HandMeshData Mesh { get; } = new HandMeshData();
    }
}