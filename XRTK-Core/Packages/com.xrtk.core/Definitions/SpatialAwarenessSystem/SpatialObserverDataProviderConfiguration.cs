// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Providers.SpatialObservers;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.Providers.SpatialObservers;

namespace XRTK.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// The configuration setting for a <see cref="IMixedRealitySpatialObserverDataProvider"/>
    /// </summary>
    [Serializable]
    public struct SpatialObserverDataProviderConfiguration : IBaseMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spatialObserverType"></param>
        /// <param name="spatialObserverName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public SpatialObserverDataProviderConfiguration(SystemType spatialObserverType, string spatialObserverName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealitySpatialObserverProfile profile)
        {
            this.spatialObserverType = spatialObserverType;
            this.spatialObserverName = spatialObserverName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Tooltip("The concrete type of spatial observer to register.")]
        [Implements(typeof(IMixedRealitySpatialObserverDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType spatialObserverType;

        /// <inheritdoc />
        public SystemType InstancedType => spatialObserverType;

        [SerializeField]
        private string spatialObserverName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string SpatialObserverName => spatialObserverName;

        [SerializeField]
        private uint priority;

        /// <inheritdoc />
        public uint Priority => priority;

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <summary>
        /// The runtime platform(s) to run this system, feature, or manager on.
        /// </summary>
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        private BaseMixedRealitySpatialObserverProfile profile;

        /// <summary>
        /// The profile to use for this spatial observer.
        /// </summary>
        public BaseMixedRealitySpatialObserverProfile Profile => profile;
    }
}