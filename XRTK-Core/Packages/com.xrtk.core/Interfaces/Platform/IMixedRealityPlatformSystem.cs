// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Interfaces.Events;

namespace XRTK.Interfaces.PlatformSystem
{
    /// <summary>
    /// Defines the platform system.
    /// </summary>
    public interface IMixedRealityPlatformSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// The list of active platforms detected by the <see cref="IMixedRealityPlatformSystem"/>.
        /// </summary>
        IReadOnlyList<IMixedRealityPlatform> ActivePlatforms { get; }
    }
}