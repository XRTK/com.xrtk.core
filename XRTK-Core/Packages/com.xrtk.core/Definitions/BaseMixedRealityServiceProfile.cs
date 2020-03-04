// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    public abstract class BaseMixedRealityServiceProfile<TService> : BaseMixedRealityProfile where TService : IMixedRealityService
    {
        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        /// <summary>
        /// The <see cref="IMixedRealityServiceConfiguration"/>s registered for this profile.
        /// </summary>
        public MixedRealityServiceConfiguration<TService>[] RegisteredServiceConfigurations
        {
            get
            {
                if (configurations == null)
                {
                    configurations = new MixedRealityServiceConfiguration[0];
                }

                var serviceConfigurations = new MixedRealityServiceConfiguration<TService>[configurations.Length];

                for (int i = 0; i < serviceConfigurations.Length; i++)
                {
                    var cachedConfig = configurations[i];
                    Debug.Assert(cachedConfig != null);
                    var serviceConfig = new MixedRealityServiceConfiguration<TService>(cachedConfig.InstancedType, cachedConfig.Name, cachedConfig.Priority, cachedConfig.RuntimePlatform, cachedConfig.ConfigurationProfile);
                    Debug.Assert(serviceConfig != null);
                    serviceConfigurations[i] = serviceConfig;
                }

                return serviceConfigurations;
            }
            internal set
            {
                var serviceConfigurations = value;

                if (serviceConfigurations == null)
                {
                    configurations = new MixedRealityServiceConfiguration[0];
                }
                else
                {
                    configurations = new MixedRealityServiceConfiguration[serviceConfigurations.Length];

                    for (int i = 0; i < serviceConfigurations.Length; i++)
                    {
                        var serviceConfig = serviceConfigurations[i];
                        Debug.Assert(serviceConfig != null);
                        var newConfig = new MixedRealityServiceConfiguration(serviceConfig.InstancedType, serviceConfig.Name, serviceConfig.Priority, serviceConfig.RuntimePlatform, serviceConfig.ConfigurationProfile);
                        Debug.Assert(newConfig != null);
                        configurations[i] = newConfig;
                    }
                }
            }
        }
    }
}