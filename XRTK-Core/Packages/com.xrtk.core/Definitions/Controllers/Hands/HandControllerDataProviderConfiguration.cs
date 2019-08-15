// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions.Controllers.Hands
{
    [Serializable]
    public struct HandControllerDataProviderConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataProviderType"></param>
        /// <param name="dataProviderName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="profile"></param>
        public HandControllerDataProviderConfiguration(SystemType dataProviderType, string dataProviderName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityControllerDataProviderProfile profile)
        {
            this.dataProviderType = dataProviderType;
            this.dataProviderName = dataProviderName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Tooltip("The concrete type of controller data provider to register.")]
        [Implements(typeof(IMixedRealityPlatformHandControllerDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataProviderType;

        /// <summary>
        /// The concrete type to use for this controller data provider.
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
        private BaseMixedRealityControllerDataProviderProfile profile;

        /// <summary>
        /// The profile settings for the controller data provider.
        /// </summary>
        public BaseMixedRealityControllerDataProviderProfile Profile => profile;
    }
}