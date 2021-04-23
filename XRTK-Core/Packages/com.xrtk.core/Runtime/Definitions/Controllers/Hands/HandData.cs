// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// An <see cref="Interfaces.Providers.Controllers.Hands.IMixedRealityHandController"/>'s data
    /// in a single frame.
    /// </summary>
    [Serializable]
    public struct HandData
    {
        /// <summary>
        /// Creates a new hand data from joint poses.
        /// </summary>
        /// <param name="rootPose">The hands root pose.</param>
        /// <param name="jointPoses">Joint pose values.</param>
        public HandData(MixedRealityPose rootPose, MixedRealityPose[] jointPoses)
        {
            if (jointPoses.Length != JointCount)
            {
                throw new ArgumentException($"{nameof(HandData)} expects {JointCount} joint poses.");
            }

            RootPose = rootPose;
            Joints = new MixedRealityPose[JointCount];
            Array.Copy(jointPoses, Joints, JointCount);

            UpdatedAt = long.MinValue;
            TrackingState = TrackingState.NotTracked;
            PinchStrength = 0;
            GripStrength = 0;
            PointerPose = MixedRealityPose.ZeroIdentity;
            IsPinching = false;
            IsPointing = false;
            IsGripping = false;
            TrackedPoseId = null;
            Mesh = new HandMeshData();
            FingerCurlStrengths = new float[] { 0, 0, 0, 0, 0 };
        }

        /// <summary>
        /// Gets the total count of joints the XRTK hand controller supports.
        /// </summary>
        public static readonly int JointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Timestamp of hand data, as FileTime, e.g. <see cref="DateTime.UtcNow"/>
        /// </summary>
        public long UpdatedAt { get; set; }

        /// <summary>
        /// The hand controller's tracking state.
        /// </summary>
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Is the hand currently in a pinch pose?
        /// </summary>
        public bool IsPinching { get; set; }

        /// <summary>
        /// What's the pinch strength for index and thumb?
        /// </summary>
        public float PinchStrength { get; set; }

        /// <summary>
        /// Is the hand currently in a pointing pose?
        /// </summary>
        public bool IsPointing { get; set; }

        /// <summary>
        /// Is the hand currently in a gripping pose?
        /// </summary>
        public bool IsGripping { get; set; }

        /// <summary>
        /// Finger curling values per hand finger.
        /// </summary>
        public float[] FingerCurlStrengths { get; set; }

        /// <summary>
        /// What's the grip strength of the hand?
        /// </summary>
        public float GripStrength { get; set; }

        /// <summary>
        /// The hand's pointer pose, relative to <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.PlayspaceTransform"/>.
        /// </summary>
        public MixedRealityPose PointerPose { get; set; }

        /// <summary>
        /// Recognized hand pose, if any.
        /// Recognizable hand poses are defined in <see cref="BaseHandControllerDataProviderProfile.TrackedPoses"/>
        /// or <see cref="InputSystem.MixedRealityInputSystemProfile.TrackedPoses"/>.
        /// </summary>
        public string TrackedPoseId { get; set; }

        /// <summary>
        /// The hands root pose. <see cref="Joints"/> poses are relative to the root pose.
        /// The root pose itself is relative to <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.PlayspaceTransform"/>.
        /// </summary>
        public MixedRealityPose RootPose { get; set; }

        /// <summary>
        /// Pose information for each hand joint, relative to <see cref="RootPose"/>.
        /// </summary>
        public MixedRealityPose[] Joints { get; set; }

        /// <summary>
        /// Mesh information of the hand.
        /// </summary>
        public HandMeshData Mesh { get; set; }
    }
}