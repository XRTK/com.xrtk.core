// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    public interface IMixedRealityServiceProfile<out TService> where TService : IMixedRealityService
    {
        /// <summary>
        /// The <see cref="IMixedRealityServiceConfiguration"/>s registered for this profile.
        /// </summary>
        IMixedRealityServiceConfiguration<TService>[] RegisteredServiceConfigurations { get; }
    }

    /// <summary>
    /// The base profile type to derive all <see cref="IMixedRealityService"/>s from.
    /// </summary>
    /// <typeparam name="TService">
    /// The <see cref="IMixedRealityService"/> type to constrain all of the valid <see cref="IMixedRealityServiceConfiguration.InstancedType"/>s to.
    /// Only types that implement the <see cref="TService"/> will show up in the inspector dropdown for the <see cref="IMixedRealityServiceConfiguration.InstancedType"/>
    /// </typeparam>
    public abstract class BaseMixedRealityServiceProfile<TService> : BaseMixedRealityProfile, IMixedRealityServiceProfile<TService> where TService : IMixedRealityService
    {
        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        private IMixedRealityServiceConfiguration<TService>[] registeredServiceConfigurations;

        /// <inheritdoc />
        public IMixedRealityServiceConfiguration<TService>[] RegisteredServiceConfigurations
        {
            get
            {
                if (configurations == null)
                {
                    configurations = new MixedRealityServiceConfiguration[0];
                }

                if (registeredServiceConfigurations == null ||
                    registeredServiceConfigurations.Length != configurations.Length)
                {
                    registeredServiceConfigurations = new IMixedRealityServiceConfiguration<TService>[configurations.Length];
                }

                for (int i = 0; i < registeredServiceConfigurations.Length; i++)
                {
                    var cachedConfig = configurations[i];
                    Debug.Assert(cachedConfig != null);
                    var serviceConfig = new MixedRealityServiceConfiguration<TService>(cachedConfig);
                    Debug.Assert(serviceConfig != null);
                    registeredServiceConfigurations[i] = serviceConfig;
                }

                return registeredServiceConfigurations;
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
                        var newConfig = new MixedRealityServiceConfiguration(serviceConfig.InstancedType, serviceConfig.Name, serviceConfig.Priority, serviceConfig.RuntimePlatforms, serviceConfig.Profile);
                        Debug.Assert(newConfig != null);
                        configurations[i] = newConfig;
                    }
                }
            }
        }

        internal void AddConfiguration(IMixedRealityServiceConfiguration<TService> configuration)
        {
            var newConfigs = new IMixedRealityServiceConfiguration<TService>[RegisteredServiceConfigurations.Length + 1];

            for (int i = 0; i < newConfigs.Length; i++)
            {
                if (i != newConfigs.Length - 1)
                {
                    newConfigs[i] = RegisteredServiceConfigurations[i];
                }
                else
                {
                    newConfigs[i] = configuration;
                }
            }

            RegisteredServiceConfigurations = newConfigs;
        }
    }
}