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
    /// <summary>
    /// Defines a system, feature, or manager to be registered with as a <see cref="IMixedRealityExtensionService"/> on startup.
    /// </summary>
    [Serializable]
    public struct MixedRealityExtensionServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType">The concrete type for the <see cref="IMixedRealityExtensionService"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityExtensionService"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityExtensionService"/> will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityExtensionService"/> to run on.</param>
        /// <param name="configurationProfile">The configuration profile for <see cref="IMixedRealityExtensionService"/>.</param>
        public MixedRealityExtensionServiceConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityExtensionServiceProfile configurationProfile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [FormerlySerializedAs("componentType")]
        [Implements(typeof(IMixedRealityExtensionService), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType => instancedType;

        [SerializeField]
        [FormerlySerializedAs("componentName")]
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
        private BaseMixedRealityExtensionServiceProfile configurationProfile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}