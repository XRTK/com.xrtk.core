// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces
{
    /// <summary>
    /// Generic interface for all Mixed Reality Data providers
    /// </summary>
    public interface IMixedRealityDataProvider : IMixedRealityService
    {
        /// <summary>
        /// The <see cref="IMixedRealityService"/> this data provider is registered with.
        /// </summary>
        IMixedRealityService ParentService { get; }
    }
}