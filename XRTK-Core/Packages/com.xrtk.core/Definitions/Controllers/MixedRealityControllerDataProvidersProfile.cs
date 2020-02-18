// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers Profiles", fileName = "MixedRealityControllerDataModelsProfile", order = (int)CreateProfileMenuItemIndices.ControllerDataProviders)]
    public class MixedRealityControllerDataProvidersProfile : BaseMixedRealityServiceProfile
    {
        [SerializeField]
        [FormerlySerializedAs("registeredControllerDataProviders")]
        private ControllerDataProviderConfiguration[] configurations = new ControllerDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered controller data providers for this input system.
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
                    configurations = new ControllerDataProviderConfiguration[serviceConfigurations.Length];
                    serviceConfigurations.CopyTo(configurations, 0);
                }
            }
        }

    }
}