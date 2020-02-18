// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    /// <summary>
    /// The base profile to use for custom <see cref="IMixedRealityExtensionService"/>s
    /// </summary>
    public abstract class BaseMixedRealityExtensionServiceProfile : BaseMixedRealityServiceProfile
    {
        [SerializeField]
        [FormerlySerializedAs("registeredDataProviders")]
        [Tooltip("Currently registered IMixedRealityDataProvider configurations for this extension service.")]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        /// <summary>
        /// Currently registered <see cref="IMixedRealityDataProvider"/> configurations for this extension service.
        /// </summary>
        /// <summary>
        /// Currently registered system and manager configurations.
        /// </summary>
        public override IMixedRealityServiceConfiguration[] RegisteredServiceConfigurations
        {
            get
            {
                IMixedRealityServiceConfiguration[] serviceConfigurations;

                if (configurations == null)
                {
                    return null;
                }
                else
                {
                    serviceConfigurations = new IMixedRealityServiceConfiguration[configurations.Length];
                    configurations.CopyTo(serviceConfigurations, 0);
                }

                return serviceConfigurations;
            }
            internal set
            {
                var serviceConfigurations = value;

                if (serviceConfigurations == null)
                {
                    configurations = null;
                }
                else
                {
                    configurations = new MixedRealityServiceConfiguration[serviceConfigurations.Length];
                    serviceConfigurations.CopyTo(configurations, 0);
                }
            }
        }
    }
}