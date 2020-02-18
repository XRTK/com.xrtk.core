// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEngine;
using UnityEngine.Serialization;
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
        /// <param name="instancedType">The concrete type for the <see cref="IMixedRealityDataProvider"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityDataProvider"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityDataProvider"/> will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityDataProvider"/> on.</param>
        /// <param name="configurationProfile">The configuration profile for the <see cref="IMixedRealityDataProvider"/>.</param>
        public DataModelConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityProfile configurationProfile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [FormerlySerializedAs("dataModelType")]
        [Implements(typeof(IMixedRealityDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType => instancedType;

        [SerializeField]
        [FormerlySerializedAs("dataModelName")]
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
        private BaseMixedRealityProfile configurationProfile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}