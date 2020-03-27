// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Definitions
{
    /// <inheritdoc cref="MixedRealityServiceConfiguration" />
    public class MixedRealityServiceConfiguration<T> : MixedRealityServiceConfiguration, IMixedRealityServiceConfiguration<T>
        where T : IMixedRealityService
    {
        /// <inheritdoc />
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityProfile configurationProfile)
            : base(instancedType, name, priority, runtimePlatform, configurationProfile)
        {
        }
    }

    /// <summary>
    /// Defines a <see cref="IMixedRealityService"/> to be registered with the <see cref="MixedRealityToolkit"/>.
    /// </summary>
    [Serializable]
    public class MixedRealityServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType">The concrete type for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityService"/> will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.</param>
        /// <param name="configurationProfile">The configuration profile for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityProfile configurationProfile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [FormerlySerializedAs("dataModelType")]
        [FormerlySerializedAs("componentType")]
        [FormerlySerializedAs("dataProviderType")]
        [FormerlySerializedAs("spatialObserverType")]
        [Implements(typeof(IMixedRealityService), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType
        {
            get => instancedType;
            internal set => instancedType = value;
        }

        [SerializeField]
        [FormerlySerializedAs("dataModelName")]
        [FormerlySerializedAs("componentName")]
        [FormerlySerializedAs("dataProviderName")]
        [FormerlySerializedAs("spatialObserverName")]
        private string name;

        /// <inheritdoc />
        public string Name
        {
            get => name;
            internal set => name = value;
        }

        [SerializeField]
        private uint priority;

        /// <inheritdoc />
        public uint Priority
        {
            get => priority;
            internal set => priority = value;
        }

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <inheritdoc />
        public SupportedPlatforms RuntimePlatform
        {
            get => runtimePlatform;
            internal set => runtimePlatform = value;
        }

        [SerializeField]
        private BaseMixedRealityProfile configurationProfile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile
        {
            get => configurationProfile;
            internal set => configurationProfile = value;
        }
    }
}