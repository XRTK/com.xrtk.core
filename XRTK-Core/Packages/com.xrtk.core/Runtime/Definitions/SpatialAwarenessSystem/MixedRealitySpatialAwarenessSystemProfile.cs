// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Spatial Awareness System Profile", fileName = "MixedRealitySpatialAwarenessSystemProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    public class MixedRealitySpatialAwarenessSystemProfile : BaseMixedRealityServiceProfile<IMixedRealitySpatialAwarenessDataProvider>
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
        [Tooltip("Indicates how the BaseMixedRealitySpatialMeshObserver is to display surface meshes within the application.")]
        private SpatialMeshDisplayOptions meshDisplayOption = SpatialMeshDisplayOptions.None;

        /// <summary>
        /// Indicates how the <see cref="BaseMixedRealitySpatialMeshObserver"/> is to display surface meshes within the application.
        /// </summary>
        public SpatialMeshDisplayOptions MeshDisplayOption => meshDisplayOption;

        [SerializeField]
        [Tooltip("The global mesh observer profile settings to use for the mesh observer data provider if no profile is provided.")]
        private BaseMixedRealitySpatialMeshObserverProfile globalMeshObserverProfile = null;

        /// <summary>
        /// The global mesh observer profile settings to use for the <see cref="IMixedRealitySpatialMeshObserver"/>s if no profile is provided.
        /// </summary>
        public BaseMixedRealitySpatialMeshObserverProfile GlobalMeshObserverProfile => globalMeshObserverProfile;

        [SerializeField]
        [Tooltip("The global mesh observer profile settings to use for the mesh observer data provider if no profile is provided.")]
        private BaseMixedRealitySurfaceObserverProfile globalSurfaceObserverProfile = null;

        /// <summary>
        /// The global mesh observer profile settings to use for the <see cref="IMixedRealitySpatialMeshObserver"/>s if no profile is provided.
        /// </summary>
        public BaseMixedRealitySurfaceObserverProfile GlobalSurfaceObserverProfile => globalSurfaceObserverProfile;
    }
}
