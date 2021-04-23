// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// The supported tracked hand joints for hand tracking.
    /// </summary>
    /// <remarks>
    /// It is absolutely important to not change the order of these
    /// joints in the enum! The hand tracking system relies on the current order
    /// and changing it will cause issues with joint mappings.
    /// </remarks>
    public enum TrackedHandJoint
    {
        /// <summary>
        /// The hand wrist.
        /// </summary>
        Wrist = 0,
        /// <summary>
        /// The hand palm.
        /// </summary>
        Palm,
        /// <summary>
        /// The lowest joint in the thumb (down in your palm).
        /// </summary>
        ThumbMetacarpal,
        /// <summary>
        /// The thumb's second (middle-ish) joint.
        /// </summary>
        ThumbProximal,
        /// <summary>
        /// The thumb's first (furthest) joint.
        /// </summary>
        ThumbDistal,
        /// <summary>
        /// The tip of the thumb.
        /// </summary>
        ThumbTip,
        /// <summary>
        /// The lowest joint of the index finger.
        /// </summary>
        IndexMetacarpal,
        /// <summary>
        /// The knuckle joint of the index finger.
        /// </summary>
        IndexProximal,
        /// <summary>
        /// The middle joint of the index finger.
        /// </summary>
        IndexIntermediate,
        /// <summary>
        /// The joint nearest the tip of the index finger.
        /// </summary>
        IndexDistal,
        /// <summary>
        /// The tip of the index finger.
        /// </summary>
        IndexTip,
        /// <summary>
        /// The lowest joint of the middle finger.
        /// </summary>
        MiddleMetacarpal,
        /// <summary>
        /// The knuckle joint of the middle finger.
        /// </summary>
        MiddleProximal,
        /// <summary>
        /// The middle joint of the middle finger.
        /// </summary>
        MiddleIntermediate,
        /// <summary>
        /// The joint nearest the tip of the finger.
        /// </summary>
        MiddleDistal,
        /// <summary>
        /// The tip of the middle finger.
        /// </summary>
        MiddleTip,
        /// <summary>
        /// The lowest joint of the ring finger.
        /// </summary>
        RingMetacarpal,
        /// <summary>
        /// The knuckle of the ring finger.
        /// </summary>
        RingProximal,
        /// <summary>
        /// The middle joint of the ring finger.
        /// </summary>
        RingIntermediate,
        /// <summary>
        /// The joint nearest the tip of the ring finger.
        /// </summary>
        RingDistal,
        /// <summary>
        /// The tip of the ring finger.
        /// </summary>
        RingTip,
        /// <summary>
        /// The lowest joint of the pinky finger.
        /// </summary>
        LittleMetacarpal,
        /// <summary>
        /// The knuckle joint of the pinky finger.
        /// </summary>
        LittleProximal,
        /// <summary>
        /// The middle joint of the pinky finger.
        /// </summary>
        LittleIntermediate,
        /// <summary>
        /// The joint nearest the tip of the pink finger.
        /// </summary>
        LittleDistal,
        /// <summary>
        /// The tip of the pinky.
        /// </summary>
        LittleTip
    }
}