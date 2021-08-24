// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Interface for teleportation anchors supported by the <see cref="ILocomotionSystem"/>.
    /// A teleport anchor is a predefined teleportatio location and thus always a valid
    /// teleport target when enabled.
    /// </summary>
    public interface ITeleportAnchor
    {
        /// <summary>
        /// The position the teleport will end at.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The normal of the teleport raycast.
        /// </summary>
        Vector3 Normal { get; }

        /// <summary>
        /// Gets whether this anchor is active.
        /// An inactive anchor cannot be teleported to.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Should the target orientation be overridden?
        /// </summary>
        bool OverrideTargetOrientation { get; }

        /// <summary>
        /// If <see cref="OverrideTargetOrientation"/> is set, this will specify the camera's target
        /// orientation on the Y-axis.
        /// </summary>
        float TargetOrientation { get; }
    }
}
