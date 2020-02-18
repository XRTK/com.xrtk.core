// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Definitions.DiagnosticsSystem
{
    [Serializable]
    public struct DiagnosticsDataProviderConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="instancedType"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public DiagnosticsDataProviderConfiguration(SystemType instancedType, string name, uint priority, SupportedPlatforms runtimePlatform, MixedRealityDiagnosticsDataProviderProfile profile)
        {
            this.instancedType = instancedType;
            this.name = name;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [FormerlySerializedAs("dataProviderType")]
        [Tooltip("The concrete type of diagnostics data provider to register.")]
        [Implements(typeof(IMixedRealityDiagnosticsDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType instancedType;

        /// <inheritdoc />
        public SystemType InstancedType => instancedType;

        [SerializeField]
        [FormerlySerializedAs("dataProviderName")]
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
        private MixedRealityDiagnosticsDataProviderProfile profile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => profile;
    }
}