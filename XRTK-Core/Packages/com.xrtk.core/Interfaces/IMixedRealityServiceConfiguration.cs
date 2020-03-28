// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using XRTK.Definitions;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Interfaces
{
    /// <summary>
    /// The generic interface for <see cref="IMixedRealityServiceConfiguration"/>s.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once UnusedTypeParameter (Used in concrete Implementations)
    public interface IMixedRealityServiceConfiguration<out T> : IMixedRealityServiceConfiguration where T : IMixedRealityService
    {
    }

    /// <summary>
    /// This interface is meant to be used with serialized structs that define valid <see cref="IMixedRealityService"/> configurations.
    /// </summary>
    public interface IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// The concrete type for the <see cref="IMixedRealityService"/> that will be instantiated and ran by the service locator.
        /// </summary>
        SystemType InstancedType { get; }

        /// <summary>
        /// The priority order of execution for this <see cref="IMixedRealityService"/>.
        /// </summary>
        /// <remarks>
        /// Multiple <see cref="IMixedRealityService"/>s may be running at the same priority for services that are not specifically registered to the <see cref="MixedRealityToolkit.ActiveSystems"/>.
        /// </remarks>
        uint Priority { get; }

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
        /// <remarks>
        /// This name should be unique.
        /// </remarks>
        string Name { get; }
    }
}