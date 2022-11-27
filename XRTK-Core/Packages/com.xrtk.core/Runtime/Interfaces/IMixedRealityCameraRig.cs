// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace XRTK.Interfaces.CameraSystem
{
    /// <summary>
    /// This interface is to be implemented by a <see cref="MonoBehaviour"/> and attached to the rig root object.
    /// </summary>
    public interface IMixedRealityCameraRig
    {
        /// <summary>
        /// The <see cref="GameObject"/> reference for this camera rig.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// The root rig transform.<para/>
        /// This transform serves as a virtual representation of the physical space.<para/>
        /// All physical objects that have digital twins will use this frame of reference to synchronize their transform data.
        /// </summary>
        Transform RigTransform { get; }

        /// <summary>
        /// The player's head transform where the <see cref="Camera"/> is located.
        /// </summary>
        Transform CameraTransform { get; }

        /// <summary>
        /// The player's <see cref="Camera"/> reference, located at their tracked head position.
        /// </summary>
        Camera PlayerCamera { get; }

        /// <summary>
        /// The <see cref="UnityEngine.SpatialTracking.TrackedPoseDriver"/> attached to the <see cref="CameraTransform"/>.
        /// </summary>
        TrackedPoseDriver TrackedPoseDriver { get; }

        /// <summary>
        /// The player's body transform, located at the player's feet.
        /// </summary>
        /// <remarks>
        /// This <see cref="Transform"/> is synced to the player's head camera X &amp; Z values.
        /// Y value is set using current <see cref="IMixedRealityCameraDataProvider.HeadHeight"/>.
        /// </remarks>
        Transform BodyTransform { get; }
    }
}
