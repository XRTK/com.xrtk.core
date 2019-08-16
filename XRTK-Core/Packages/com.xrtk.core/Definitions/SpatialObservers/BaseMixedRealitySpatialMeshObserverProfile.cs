// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.SpatialObservers
{
    public abstract class BaseMixedRealitySpatialMeshObserverProfile : BaseMixedRealitySpatialObserverProfile
    {
        [PhysicsLayer]
        [SerializeField]
        [Tooltip("The optional mesh Physics layer override of the base PhysicsLayer")]
        private int meshPhysicsLayerOverride = -1;

        /// <summary>
        /// The optional Physics layer override of the base <see cref="Interfaces.Providers.SpatialObservers.IMixedRealitySpatialObserverDataProvider.PhysicsLayer"/>
        /// </summary>
        public int MeshPhysicsLayerOverride => meshPhysicsLayerOverride;

        [SerializeField]
        [Tooltip("Level of detail for the mesh")]
        private SpatialAwarenessMeshLevelOfDetail meshLevelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <summary>
        /// The desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail => meshLevelOfDetail;

        [SerializeField]
        [Tooltip("Level of detail, in triangles per cubic meter.\nIgnored unless MeshLevelOfDetail is set to Custom.")]
        private uint meshTrianglesPerCubicMeter = 0;

        /// <summary>
        /// The level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>This value is ignored, unless <see cref="MeshLevelOfDetail"/> is set to Coarse.</remarks>
        public uint MeshTrianglesPerCubicMeter => meshTrianglesPerCubicMeter;

        [SerializeField]
        [Tooltip("Should normals be recalculated when a mesh is added or updated?")]
        private bool meshRecalculateNormals = true;

        /// <summary>
        /// Indicates if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh.
        /// </summary>
        public bool MeshRecalculateNormals => meshRecalculateNormals;

        [SerializeField]
        [Tooltip("Material to use when displaying meshes")]
        private Material meshVisibleMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying spatial meshes.
        /// </summary>
        public Material MeshVisibleMaterial => meshVisibleMaterial;

        [SerializeField]
        [Tooltip("Material to use when spatial meshes should occlude other objects")]
        private Material meshOcclusionMaterial = null;

        /// <summary>
        /// The material to be used when spatial meshes should occlude other objects.
        /// </summary>
        public Material MeshOcclusionMaterial => meshOcclusionMaterial;

        [SerializeField]
        [Tooltip("Additional components to add to the generated spatial mesh GameObject")]
        [SystemType(typeof(Component), TypeGrouping.ByAddComponentMenu)]
        private SystemType[] additionalComponents = new SystemType[0];

        /// <summary>
        /// Additional <see cref="Component"/>s to add to the generated spatial mesh <see cref="GameObject"/>s
        /// </summary>
        public SystemType[] AdditionalComponents => additionalComponents;

        [SerializeField]
        [Tooltip("Prefab to use for Object Pool instead of generated object.")]
        private GameObject meshObjectPrefab = null;

        /// <summary>
        /// Prefab to use for Object Pool instead of generated mesh object.
        /// </summary>
        public GameObject MeshObjectPrefab => meshObjectPrefab;
    }
}