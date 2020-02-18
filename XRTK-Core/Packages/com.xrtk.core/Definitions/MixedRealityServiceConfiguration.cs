using System;
using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Definitions
{
    /// <summary>
    /// Defines a <see cref="IMixedRealityService"/> to be registered with the <see cref="MixedRealityToolkit"/>.
    /// </summary>
    [Serializable]
    public struct MixedRealityServiceConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType">The concrete type for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="name">The simple, human readable name for the <see cref="IMixedRealityService"/>.</param>
        /// <param name="priority">The priority this <see cref="IMixedRealityService"/> will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this <see cref="IMixedRealityService"/> to run on.</param>
        /// <param name="configurationProfile">The configuration profile for <see cref="IMixedRealityService"/>.</param>
        public MixedRealityServiceConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityServiceProfile configurationProfile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }

        [SerializeField]
        [FormerlySerializedAs("componentType")]
        [Implements(typeof(IMixedRealityService), TypeGrouping.ByNamespaceFlat)]
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
        private BaseMixedRealityServiceProfile configurationProfile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => configurationProfile;
    }
}