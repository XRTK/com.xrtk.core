// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using XRTK.Definitions;
using XRTK.Definitions.Utilities;

namespace XRTK.Interfaces
{
    /// <summary>
    /// This interface is meant to be used with serialized structs that define valid <see cref="IMixedRealityService"/> configurations.
    /// </summary>
    public interface IMixedRealityServiceConfiguration : IBaseMixedRealityServiceConfiguration
    {
        /// <summary>
        /// The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.
        /// </summary>
        SupportedPlatforms RuntimePlatform { get; }

        /// <summary>
        /// The configuration profile for the <see cref="IMixedRealityService"/>.
        /// </summary>
        BaseMixedRealityProfile ConfigurationProfile { get; }

        /// <summary>
        /// The simple, human readable name for the <see cref="IMixedRealityService"/>.
        /// </summary>
        string Name { get; }
    }
}