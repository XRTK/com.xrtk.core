// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public struct MixedRealityServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The concrete type for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityService"/> will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.</param>
        /// <param name="configurationProfile">The configuration profile for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType componentType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityExtensionServiceProfile configurationProfile)
        {
            this.componentType = componentType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityExtensionService), TypeGrouping.ByNamespaceFlat)]
        private SystemType componentType;

        /// <inheritdoc />
        public SystemType InstancedType => componentType;

        [SerializeField]
        [FormerlySerializedAs("componentName")]
        private string name;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string Name => name;

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
        private BaseMixedRealityExtensionServiceProfile configurationProfile;

        /// <summary>
        /// The configuration profile for the system, feature, or manager.
        /// </summary>
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}