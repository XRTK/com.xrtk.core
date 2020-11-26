// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumeration defining levels of detail for the spatial awareness service.
    /// </summary>
    public enum SpatialAwarenessMeshLevelOfDetail
    {
        /// <summary>
        /// The low level of detail is well suited for identifying large
        /// environmental features, such as floors and walls.
        /// </summary>
        Low = 0,

        /// <summary>
        /// The medium level of detail is suited for fast environmental mesh occlusion.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// The high level of detail is well suited for mesh occlusion for object like hands.
        /// </summary>
        High = 2
    }
}
