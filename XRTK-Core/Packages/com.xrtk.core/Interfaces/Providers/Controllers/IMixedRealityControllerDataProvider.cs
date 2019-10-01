// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityControllerDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required)
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMixedRealityController> ActiveControllers { get; }
    }
}