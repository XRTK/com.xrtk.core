// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;
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
    public struct SpatialObserverDataProviderConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public SpatialObserverDataProviderConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealitySpatialObserverProfile profile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [FormerlySerializedAs("spatialObserverType")]
        [Tooltip("The concrete type of spatial observer to register.")]
        [Implements(typeof(IMixedRealitySpatialObserverDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType => instancedType;

        [FormerlySerializedAs("spatialObserverName")]
        [SerializeField]
        private string name;

        /// <inheritdoc />
        public string Name => name;

        [SerializeField]
        private uint priority;

        /// <inheritdoc />
        public uint Priority => priority;

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <inheritdoc />
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        private BaseMixedRealitySpatialObserverProfile profile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => profile;
    }
}