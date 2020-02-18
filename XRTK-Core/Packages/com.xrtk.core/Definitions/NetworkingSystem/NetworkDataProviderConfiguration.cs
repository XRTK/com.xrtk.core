// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Providers.Networking.Profiles;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.NetworkingSystem;

namespace XRTK.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration settings for a registered <see cref="IMixedRealityNetworkDataProvider"/>.
    /// </summary>
    [Serializable]
    public struct NetworkDataProviderConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType">The concrete type for the system, feature or manager.</param>
        /// <param name="name">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="profile">The configuration profile for the system, feature, or manager.</param>
        public NetworkDataProviderConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityNetworkProviderProfile profile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [FormerlySerializedAs("dataProviderType")]
        [Implements(typeof(IMixedRealityNetworkDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType => instancedType;

        [SerializeField]
        [FormerlySerializedAs("dataProviderName")]
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
        private BaseMixedRealityNetworkProviderProfile profile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => profile;
    }
}