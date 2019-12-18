// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.PlatformSystem
{
    /// <summary>
    /// Defines the device and platform to be registered and used in <see cref="IMixedRealityPlatformSystem"/>
    /// </summary>
    public interface IMixedRealityPlatform : IMixedRealityDataProvider
    {
        /// <summary>
        /// Is this platform currently active and available?
        /// </summary>
        bool IsActive { get; }
    }
}