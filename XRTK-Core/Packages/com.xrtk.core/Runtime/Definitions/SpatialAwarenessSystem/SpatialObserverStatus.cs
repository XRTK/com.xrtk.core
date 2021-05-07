// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Providers.SpatialObservers;

namespace XRTK.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumerates possible changes that may happen to a spatial observers
    /// by the <see cref="BaseMixedRealitySpatialObserverDataProvider"/>.
    /// </summary>
    public enum SpatialObserverStatus
    {
        /// <summary>
        /// The spatial object was removed. This may happen with objects move outside of the spatial observer bounds,
        /// when an environment changes in between spatial meshing updates, or when when the spatial observer was refined.
        /// </summary>
        Removed = 0,
        /// <summary>
        /// The spatial object was initially recognized and added to the list of tracked spatial objects.
        /// </summary>
        Added,
        /// <summary>
        /// The spatial object was already being observed but got updated.
        /// </summary>
        Updated
    }
}
