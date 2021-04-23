// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Interfaces.BoundarySystem
{
    /// <summary>
    /// Mixed Reality Toolkit boundary data provider definition, used to instantiate and manage low level api access specific devices, SDKs, and libraries.
    /// </summary>
    public interface IMixedRealityBoundaryDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Is the platform's boundary visible?
        /// </summary>
        bool IsPlatformBoundaryVisible { get; set; }

        /// <summary>
        /// The the platform's boundary configured?
        /// </summary>
        bool IsPlatformConfigured { get; }

        /// <summary>
        /// Try to get the boundary geometry from the library api.
        /// </summary>
        /// <param name="geometry">The list of points associated with the boundary geometry.</param>
        /// <returns>True, if valid geometry was successfully returned, otherwise false.</returns>
        bool TryGetBoundaryGeometry(ref List<Vector3> geometry);
    }
}