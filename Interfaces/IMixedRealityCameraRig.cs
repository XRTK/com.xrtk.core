// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

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
        /// The player's body transform. This <see cref="Transform"/> is synced to the player's head X &amp; Z values
        /// and the <see cref="CameraTransform"/>'s Y value.
        /// </summary>
        Transform BodyTransform { get; }
    }
}
