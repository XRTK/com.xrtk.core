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
        public IMixedRealityServiceConfiguration<TService>[] RegisteredServiceConfigurations
        {
            get
            {
                if (configurations == null)
                {
                    configurations = new MixedRealityServiceConfiguration[0];
                }

                var serviceConfigurations = new IMixedRealityServiceConfiguration[configurations.Length];
                configurations.CopyTo(serviceConfigurations, 0);
                return serviceConfigurations as IMixedRealityServiceConfiguration<TService>[];
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

                    for (int i = 0; i < serviceConfigurations.Length; i++)
                    {
                        var oldConfig = configurations[i];
                        var newConfig = serviceConfigurations[i];

                        oldConfig.InstancedType = newConfig.InstancedType;
                        oldConfig.Name = newConfig.Name;
                        oldConfig.Priority = newConfig.Priority;
                        oldConfig.RuntimePlatform = newConfig.RuntimePlatform;
                        configurations[i] = oldConfig;
                    }
                }
            }
        }
    }
}