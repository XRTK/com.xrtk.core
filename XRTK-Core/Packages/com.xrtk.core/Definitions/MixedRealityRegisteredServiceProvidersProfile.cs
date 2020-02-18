// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Registered Service Providers Profile", fileName = "MixedRealityRegisteredServiceProvidersProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class MixedRealityRegisteredServiceProvidersProfile : BaseMixedRealityServiceProfile
    {
        [SerializeField]
        private MixedRealityExtensionServiceConfiguration[] configurations = new MixedRealityExtensionServiceConfiguration[0];

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
                    configurations = new MixedRealityExtensionServiceConfiguration[serviceConfigurations.Length];
                    serviceConfigurations.CopyTo(configurations, 0);
                }
            }
        }
    }
}