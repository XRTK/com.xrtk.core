// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.PlatformSystem;

namespace XRTK.Definitions.PlatformSystem
{
    [Serializable]
    public struct PlatformConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="platformType">The concrete type for the system, feature or manager.</param>
        /// <param name="platformName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="configurationProfile">The configuration profile for the system, feature, or manager.</param>
        public PlatformConfiguration(SystemType platformType, string platformName, uint priority, BaseMixedRealityPlatformProfile configurationProfile)
        {
            this.platformType = platformType;
            this.platformName = platformName;
            this.priority = priority;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityPlatform), TypeGrouping.ByNamespaceFlat)]
        private SystemType platformType;

        /// <summary>
        /// The concrete type for the system, feature or manager.
        /// </summary>
        public SystemType PlatformType => platformType;

        [SerializeField]
        private string platformName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string PlatformName => platformName;

        [SerializeField]
        private uint priority;

        /// <summary>
        /// The priority this system, feature, or manager will be initialized in.
        /// </summary>
        public uint Priority => priority;

        [SerializeField]
        private BaseMixedRealityPlatformProfile configurationProfile;

        /// <summary>
        /// The configuration profile for the system, feature, or manager.
        /// </summary>
        public BaseMixedRealityPlatformProfile ConfigurationProfile => configurationProfile;
    }
}