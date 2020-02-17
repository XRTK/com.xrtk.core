// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    [Serializable]
    public struct DataModelConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataModelType">The concrete type for the system, feature or manager.</param>
        /// <param name="dataModelName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="configurationProfile">The configuration profile for the system, feature, or manager.</param>
        public DataModelConfiguration(SystemType dataModelType, string dataModelName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityProfile configurationProfile)
        {
            this.dataModelType = dataModelType;
            this.dataModelName = dataModelName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataModelType;

        /// <inheritdoc />
        public SystemType InstancedType => dataModelType;

        [SerializeField]
        private string dataModelName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string DataModelName => dataModelName;

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
        private BaseMixedRealityProfile configurationProfile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}