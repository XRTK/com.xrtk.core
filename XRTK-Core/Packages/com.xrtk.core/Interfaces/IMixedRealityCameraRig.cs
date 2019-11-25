// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SpatialTracking;

namespace XRTK.Interfaces
{
    /// <summary>
    /// This interface is to be implemented by a <see cref="MonoBehaviour"/> and attached to the playspace root object.
    /// </summary>
    public interface IMixedRealityCameraRig
    {
        /// <summary>
        /// The root playspace transform that serves as the root of the camera rig.
        /// </summary>
        /// <remarks>
        /// This transform serves as a virtual representation of the physical space.
        /// All physical objects that have digital twins will use this frame of reference to synchronize their transform data.
        /// </remarks>
        Transform PlayspaceTransform { get; }

        /// <summary>
        /// The player's head transform where the <see cref="Camera"/> is located.
        /// </summary>
        Transform CameraTransform { get; }

        /// <summary>
        /// The player's <see cref="Camera"/> reference.
        /// </summary>
        Camera PlayerCamera { get; }

        /// <summary>
        /// The <see cref="TrackedPoseDriver"/> attached to the <see cref="CameraTransform"/>.
        /// </summary>
        TrackedPoseDriver CameraPoseDriver { get; }

        /// <summary>
        /// The player's body transform.
        /// </summary>
        /// <remarks>
        /// This <see cref="Transform"/> is synced to the player's head camera X &amp; Z values
        /// and the <see cref="PlayspaceTransform"/>'s Y value.
        /// </remarks>
        Transform BodyTransform { get; }
    }
}