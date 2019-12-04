// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Hand Controller Visualization Profile", fileName = "MixedRealityHandControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    public class MixedRealityHandControllerVisualizationProfile : BaseMixedRealityProfile
    {
        #region Joint Visualization Settings

        [Header("Joint Visualization Settings")]

        [SerializeField]
        [Tooltip("Renders the hand joints. Note: this could reduce performance.")]
        private bool enableHandJointVisualization = false;
        /// <summary>
        /// Renders the hand joints. Note: this could reduce performance.
        /// </summary>
        public bool EnableHandJointVisualization
        {
            get => enableHandJointVisualization;
            set
            {
                enableHandJointVisualization = value;
            }
        }

        [SerializeField]
        [Tooltip("The joint prefab to use.")]
        private GameObject jointPrefab = null;

        /// <summary>
        /// The joint prefab to use.
        /// </summary>
        public GameObject JointPrefab => jointPrefab;

        [SerializeField]
        [Tooltip("The joint prefab to use for palm.")]
        private GameObject palmPrefab = null;

        /// <summary>
        /// The joint prefab to use for palm
        /// </summary>
        public GameObject PalmJointPrefab => palmPrefab;

        [SerializeField]
        [Tooltip("The joint prefab to use for the index tip (point of interaction.")]
        private GameObject fingertipPrefab = null;

        /// <summary>
        /// The joint prefab to use for finger tip
        /// </summary>
        public GameObject FingerTipPrefab => fingertipPrefab;

        #endregion

        #region Mesh Visualization Settings

        [Header("Mesh Visualization Settings")]

        [SerializeField]
        [Tooltip("Renders the hand mesh, if available. Note: this could reduce performance.")]
        private bool enableHandMeshVisualization = false;
        /// <summary>
        /// Renders the hand mesh, if available. Note: this could reduce performance.
        /// </summary>
        public bool EnableHandMeshVisualization
        {
            get => enableHandMeshVisualization;
            set
            {
                enableHandMeshVisualization = value;
            }
        }

        [SerializeField]
        [Tooltip("If this is not null and hand system supports hand meshes, use this mesh to render hand mesh.")]
        private GameObject handMeshPrefab = null;

        /// <summary>
        /// The hand mesh prefab to use to render the hand
        /// </summary>
        public GameObject HandMeshPrefab => handMeshPrefab;

        #endregion
    }
}