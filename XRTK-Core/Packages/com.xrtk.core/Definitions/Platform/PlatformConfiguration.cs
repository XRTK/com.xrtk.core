// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.Platform;

namespace XRTK.Definitions.Platform
{
    [Serializable]
    public struct PlatformConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataModelType">The concrete type for the system, feature or manager.</param>
        /// <param name="dataModelName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="configurationProfile">The configuration profile for the system, feature, or manager.</param>
        public PlatformConfiguration(SystemType dataModelType, string dataModelName, uint priority, SupportedPlatforms runtimePlatform, MixedRealityPlatformProfile configurationProfile)
        {
            this.dataModelType = dataModelType;
            this.dataModelName = dataModelName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityPlatform), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataModelType;

        /// <summary>
        /// The concrete type for the system, feature or manager.
        /// </summary>
        public SystemType DataModelType => dataModelType;

        [SerializeField]
        private string dataModelName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string DataModelName => dataModelName;

        [SerializeField]
        private uint priority;

        /// <summary>
        /// The priority this system, feature, or manager will be initialized in.
        /// </summary>
        public uint Priority => priority;

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <summary>
        /// The runtime platform(s) to run this system, feature, or manager on.
        /// </summary>
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        private MixedRealityPlatformProfile configurationProfile;

        /// <summary>
        /// The configuration profile for the system, feature, or manager.
        /// </summary>
        public MixedRealityPlatformProfile ConfigurationProfile => configurationProfile;
    }
}