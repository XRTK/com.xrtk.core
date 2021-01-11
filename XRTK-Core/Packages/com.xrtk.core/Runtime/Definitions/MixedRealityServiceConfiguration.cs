// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
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
        public MixedRealityServiceConfiguration(IMixedRealityServiceConfiguration configuration)
            : base(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile)
        {
        }

        /// <inheritdoc />
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseMixedRealityProfile profile)
            : base(instancedType, name, priority, runtimePlatforms, profile)
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
        /// <param name="runtimePlatforms">runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.</param>
        /// <param name="profile">The <see cref="BaseMixedRealityProfile"/> for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, IReadOnlyList<IMixedRealityPlatform> runtimePlatforms, BaseMixedRealityProfile profile)
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

            this.profile = profile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityService), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType
        {
            get => instancedType;
            internal set => instancedType = value;
        }

        [SerializeField]
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

                    for (int i = 0; i < platformEntries?.RuntimePlatforms?.Length; i++)
                    {
                        var platformType = platformEntries.RuntimePlatforms[i]?.Type;

                        if (platformType == null)
                        {
                            continue;
                        }

                        IMixedRealityPlatform platformInstance;

                        try
                        {
                            platformInstance = Activator.CreateInstance(platformType) as IMixedRealityPlatform;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            continue;
                        }

                        runtimePlatforms.Add(platformInstance);
                    }
                }

                return runtimePlatforms;
            }
        }

        [SerializeField]
        private BaseMixedRealityProfile profile;

        /// <inheritdoc />
        public BaseMixedRealityProfile Profile
        {
            get => profile;
            internal set => profile = value;
        }
    }
}