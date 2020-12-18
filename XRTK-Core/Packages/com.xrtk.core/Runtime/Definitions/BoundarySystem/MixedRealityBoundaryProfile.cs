// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.BoundarySystem;

namespace XRTK.Definitions.BoundarySystem
{
    /// <summary>
    /// Configuration profile settings for setting up the <see cref="IMixedRealityBoundarySystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Boundary Profile", fileName = "MixedRealityBoundaryProfile", order = (int)CreateProfileMenuItemIndices.Boundary)]
    public class MixedRealityBoundaryProfile : BaseMixedRealityServiceProfile<IMixedRealityBoundaryDataProvider>
    {
        #region General Settings

        [SerializeField]
        [Tooltip("Force the boundary to always be displayed in the scene?")]
        private bool showBoundary = false;

        /// <summary>
        /// Should the boundary system display the play area?
        /// </summary>
        public bool ShowBoundary => showBoundary;

        [Min(1f)]
        [SerializeField]
        [Tooltip("The height of the boundary, in meters.")]
        private float boundaryHeight = 3.0f;

        /// <summary>
        /// The developer defined height of the boundary, in meters.
        /// </summary>
        /// <remarks>
        /// The BoundaryHeight property is used to create a three dimensional volume for the play space.
        /// </remarks>
        public float BoundaryHeight => boundaryHeight;

        [SerializeField]
        [Tooltip("The material to use when displaying the boundary.")]
        private Material boundaryMaterial = null;

        /// <summary>
        /// The material to use for the rectangular play area <see cref="GameObject"/>.
        /// </summary>
        public Material BoundaryMaterial => boundaryMaterial;

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("The physics layer to assign to all the generated boundary GameObjects.")]
        private int physicsLayer = 2;

        /// <summary>
        /// The physics layer to assign to all the generated boundary <see cref="GameObject"/>s.
        /// </summary>
        public int PhysicsLayer => physicsLayer;

        #endregion General Settings

        #region Floor settings

        [SerializeField]
        [Tooltip("Force the boundary system display the floor?")]
        private bool showFloor = true;

        /// <summary>
        /// Force the boundary system display the floor?
        /// </summary>
        public bool ShowFloor => showFloor;

        [SerializeField]
        [Tooltip("The material to use when displaying the floor.\n\nNote: if none is set, boundary material is used.")]
        private Material floorMaterial = null;

        /// <summary>
        /// The material to use for the floor <see cref="GameObject"/> when created by the boundary system.
        /// </summary>
        public Material FloorMaterial => floorMaterial;

        #endregion Floor settings

        #region Boundary wall settings

        [SerializeField]
        [Tooltip("Force the boundary walls be displayed?")]
        private bool showWalls = false;

        /// <summary>
        /// Force the boundary walls be displayed?
        /// </summary>
        public bool ShowWalls => showWalls;

        [SerializeField]
        [Tooltip("The material to use when displaying the boundary walls.\n\nNote: if none is set, boundary material is used.")]
        private Material wallMaterial = null;

        /// <summary>
        /// The material to use for displaying the boundary geometry walls.
        /// </summary>
        public Material WallMaterial => wallMaterial;

        #endregion Boundary wall settings

        #region Boundary ceiling settings

        [SerializeField]
        [Tooltip("Force the boundary ceiling be displayed?")]
        private bool showCeiling = false;

        /// <summary>
        /// Force the boundary ceiling be displayed?
        /// </summary>
        public bool ShowCeiling => showCeiling;

        [SerializeField]
        [Tooltip("The material to use when displaying the boundary ceiling.\n\nNote: if none is set, boundary material is used.")]
        private Material ceilingMaterial = null;

        /// <summary>
        /// The material to use for displaying the boundary ceiling.
        /// </summary>
        public Material CeilingMaterial => ceilingMaterial;

        #endregion Boundary ceiling settings
    }
}
