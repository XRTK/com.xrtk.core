// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Definitions.DiagnosticsSystem
{
    [Serializable]
    public struct DiagnosticsDataProviderConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataProviderType"></param>
        /// <param name="dataProviderName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public DiagnosticsDataProviderConfiguration(SystemType dataProviderType, string dataProviderName, uint priority, SupportedPlatforms runtimePlatform, MixedRealityDiagnosticsDataProviderProfile profile)
        {
            this.dataProviderType = dataProviderType;
            this.dataProviderName = dataProviderName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Tooltip("The concrete type of diagnostics data provider to register.")]
        [Implements(typeof(IMixedRealityDiagnosticsDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataProviderType;

        /// <summary>
        /// The concrete type to use for this data provider.
        /// </summary>
        public SystemType DataProviderType => dataProviderType;

        [SerializeField]
        private string dataProviderName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string DataProviderName => dataProviderName;

        [SerializeField]
        private uint priority;

        /// <summary>
        /// The priority this system, feature, or manager will be initialized in.
        /// </summary>
        public uint Priority => priority;

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <summary>
        /// The runtime platform(s) to run this system, feature, or manager on.
        /// </summary>
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        private MixedRealityDiagnosticsDataProviderProfile profile;

        /// <summary>
        /// The profile settings for the data provider.
        /// </summary>
        public MixedRealityDiagnosticsDataProviderProfile Profile => profile;
    }
}