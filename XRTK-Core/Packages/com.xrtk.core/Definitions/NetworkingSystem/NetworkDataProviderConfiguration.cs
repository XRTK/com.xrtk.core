// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
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
        /// <param name="dataProviderType">The concrete type for the system, feature or manager.</param>
        /// <param name="dataProviderName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="profile">The configuration profile for the system, feature, or manager.</param>
        public NetworkDataProviderConfiguration(SystemType dataProviderType, string dataProviderName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityNetworkProviderProfile profile)
        {
            this.dataProviderType = dataProviderType;
            this.dataProviderName = dataProviderName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityNetworkDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataProviderType;

        /// <inheritdoc />
        public SystemType InstancedType => dataProviderType;

        [SerializeField]
        private string dataProviderName;

        /// <inheritdoc />
        public string Name => dataProviderName;

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