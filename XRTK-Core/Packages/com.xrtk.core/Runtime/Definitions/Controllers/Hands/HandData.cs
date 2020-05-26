// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// An <see cref="Interfaces.Providers.Controllers.Hands.IMixedRealityHandController"/>'s data
    /// in a single frame.
    /// </summary>
    public class HandData
    {
        /// <summary>
        /// Gets the total count of joints the XRTK hand controller supports.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. <see cref="DateTime.UtcNow"/>
        /// </summary>
        public long UpdatedAt { get; set; } = 0;

        /// <summary>
        /// Is the hand currently being tracked by the system?
        /// </summary>
        public bool IsTracked { get; set; } = false;

        /// <summary>
        /// The handedness of the hand the data belongs to.
        /// </summary>
        public Handedness Handedness { get; set; } = Handedness.None;

        /// <summary>
        /// Is the hand currently in a pinch pose?
        /// </summary>
        public bool IsPinching { get; set; } = false;

        /// <summary>
        /// What's the pinch strength for index and thumb?
        /// </summary>
        public float PinchStrength { get; set; } = 0;

        /// <summary>
        /// Is the hand currently in a pointing pose?
        /// </summary>
        public bool IsPointing { get; set; } = false;

        /// <summary>
        /// Is the hand currently in a gripping pose?
        /// </summary>
        public bool IsGripping { get; set; } = false;

        /// <summary>
        /// What's the grip strength of the hand?
        /// </summary>
        public float GripStrength { get; set; } = 0;

        /// <summary>
        /// The hand's pointer pose.
        /// </summary>
        public MixedRealityPose PointerPose { get; set; } = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Recognized hand pose, if any.
        /// Recognizable hand poses are defined in <see cref="BaseHandControllerDataProviderProfile.TrackedPoses"/>
        /// or <see cref="InputSystem.MixedRealityInputSystemProfile.TrackedPoses"/>.
        /// </summary>
        public HandControllerPoseDefinition TrackedPose { get; set; } = null;

        /// <summary>
        /// The hands root pose. <see cref="Joints"/> poses are relative to the root pose.
        /// The root pose itself is in <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.PlayspaceTransform"/> space.
        /// </summary>
        public MixedRealityPose RootPose { get; set; } = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Pose information for each hand joint, relative to <see cref="RootPose"/>.
        /// </summary>
        public MixedRealityPose[] Joints { get; } = new MixedRealityPose[JointCount];

        /// <summary>
        /// Mesh information of the hand.
        /// </summary>
        public HandMeshData Mesh { get; set; } = new HandMeshData();
    }
}