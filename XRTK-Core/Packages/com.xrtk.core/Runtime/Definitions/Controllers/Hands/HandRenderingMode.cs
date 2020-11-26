// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Defines the different rendering modes for hand controllers in space.
    /// </summary>
    public enum HandRenderingMode
    {
        /// <summary>
        /// A hand is not visualized at all.
        /// It's just there in ghost mode.
        /// </summary>
        None = 0,
        /// <summary>
        /// A hand is visuailzed using its joints poses data.
        /// </summary>
        Joints,
        /// <summary>
        /// A hand is visuailzed using its hand mesh data.
        /// If a platform does not provide hand mesh data, visualization
        /// will fallback to joints.
        /// </summary>
        Mesh
    }
}