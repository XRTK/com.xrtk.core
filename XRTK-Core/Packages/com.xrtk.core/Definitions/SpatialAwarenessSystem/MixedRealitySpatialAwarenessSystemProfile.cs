// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Spatial Awareness System Profile", fileName = "MixedRealitySpatialAwarenessSystemProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    public class MixedRealitySpatialAwarenessSystemProfile : BaseMixedRealityServiceProfile
    {
        /// <summary>
        /// The name of the Spatial Awareness Mesh Physics Layer.
        /// </summary>
        public const string SpatialAwarenessMeshesLayerName = "Spatial Awareness Meshes";

        /// <summary>
        /// The name of the Spatial Awareness Surfaces Physics Layer.
        /// </summary>
        public const string SpatialAwarenessSurfacesLayerName = "Spatial Awareness Surfaces";

        [SerializeField]
        [FormerlySerializedAs("registeredSpatialObserverDataProviders")]
        [Tooltip("The list of registered spatial observer data providers.")]
        private SpatialObserverDataProviderConfiguration[] configurations = new SpatialObserverDataProviderConfiguration[0];

        /// <summary>
        /// The list of registered <see cref="IMixedRealitySpatialObserverDataProvider"/>s.
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
                    configurations = new SpatialObserverDataProviderConfiguration[serviceConfigurations.Length];
                    serviceConfigurations.CopyTo(configurations, 0);
                }
            }
        }

        [SerializeField]
        [Tooltip("Indicates how the BaseMixedRealitySpatialMeshObserver is to display surface meshes within the application.")]
        private SpatialMeshDisplayOptions meshDisplayOption = SpatialMeshDisplayOptions.None;

        /// <summary>
        /// Indicates how the <see cref="BaseMixedRealitySpatialMeshObserver"/> is to display surface meshes within the application.
        /// </summary>
        public SpatialMeshDisplayOptions MeshDisplayOption => meshDisplayOption;
    }
}
