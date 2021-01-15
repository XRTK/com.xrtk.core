// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// Base interface to implement for teleportation hotspots to work with the <see cref="IMixedRealityLocomotionSystem"/>.
    /// </summary>
    public interface IMixedRealityTeleportHotSpot
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
        /// Gets whether this hotspot is active. An inactive hotspot cannot be
        /// teleported to.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Should the target orientation be overridden?
        /// </summary>
        bool OverrideTargetOrientation { get; }

        /// <summary>
        /// Should the destination orientation be overridden?
        /// Useful when you want to orient the user in a specific direction when they teleport to this position.
        /// </summary>
        /// <remarks>
        /// Override orientation is the transform forward of the GameObject this component is attached to.
        /// </remarks>
        float TargetOrientation { get; }
    }
}