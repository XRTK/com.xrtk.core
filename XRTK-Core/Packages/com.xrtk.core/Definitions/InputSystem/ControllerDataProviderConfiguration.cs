// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Definitions.Controllers;
using XRTK.Interfaces;

namespace XRTK.Definitions.InputSystem
{
    [Serializable]
    public struct ControllerDataProviderConfiguration : IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataProviderType"></param>
        /// <param name="dataProviderName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public ControllerDataProviderConfiguration(SystemType dataProviderType, string dataProviderName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityControllerDataProviderProfile profile)
        {
            this.dataProviderType = dataProviderType;
            this.dataProviderName = dataProviderName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Tooltip("The concrete type of controller data provider to register.")]
        [Implements(typeof(IMixedRealityControllerDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataProviderType;

        /// <inheritdoc />
        public SystemType InstancedType => dataProviderType;

        [SerializeField]
        private string dataProviderName;

        /// <inheritdoc />
        public string Name => dataProviderName;

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
        private BaseMixedRealityControllerDataProviderProfile profile;

        /// <inheritdoc />
        public BaseMixedRealityProfile ConfigurationProfile => profile;
    }
}