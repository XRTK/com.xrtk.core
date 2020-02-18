// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration profile settings for setting up networking.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Network System Profile", fileName = "MixedRealityNetworkSystemProfile", order = (int)CreateProfileMenuItemIndices.Networking)]
    public class MixedRealityNetworkSystemProfile : BaseMixedRealityServiceProfile
    {
        [SerializeField]
        [FormerlySerializedAs("registeredNetworkDataProviders")]
        private NetworkDataProviderConfiguration[] configurations = new NetworkDataProviderConfiguration[0];

        /// <summary>
        /// The list of registered network data providers.
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
                    configurations = new NetworkDataProviderConfiguration[serviceConfigurations.Length];
                    serviceConfigurations.CopyTo(configurations, 0);
                }
            }
        }
    }
}