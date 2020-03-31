// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
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
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseMixedRealityProfile configurationProfile)
            : base(instancedType, name, priority, runtimePlatforms, configurationProfile)
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
        /// <param name="runtimePlatforms"></param>
        /// <param name="configurationProfile">The configuration profile for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseMixedRealityProfile configurationProfile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;

            if (runtimePlatforms != null)
            {
                this.runtimePlatforms = new List<IMixedRealityPlatform>(runtimePlatforms.Count);

                for (int i = 0; i < runtimePlatforms.Count; i++)
                {
                    this.runtimePlatforms.Add(runtimePlatforms[i]);
                }

                platformEntries = new RuntimePlatformEntry(runtimePlatforms);
            }

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

        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        [NonSerialized]
        private List<IMixedRealityPlatform> runtimePlatforms = null;

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityPlatform> RuntimePlatforms
        {
            get
            {
                if (runtimePlatforms == null ||
                    runtimePlatforms.Count == 0 ||
                    runtimePlatforms.Count != platformEntries?.RuntimePlatforms?.Length)
                {
                    runtimePlatforms = new List<IMixedRealityPlatform>();

                    for (int i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                    {
                        var availablePlatform = MixedRealityToolkit.AvailablePlatforms[i];
                        var availablePlatformType = availablePlatform.GetType();

                        for (int j = 0; j < platformEntries?.RuntimePlatforms?.Length; j++)
                        {
                            var platformType = platformEntries.RuntimePlatforms[j]?.Type;

                            if (platformType == null)
                            {
                                Debug.LogError($"Failed to resolve {platformEntries.RuntimePlatforms[j]} for {name}");
                                continue;
                            }

                            if (availablePlatformType == platformType)
                            {
                                runtimePlatforms.Add(availablePlatform);
                            }
                        }
                    }
                }

                return runtimePlatforms;
            }
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