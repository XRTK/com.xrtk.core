// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Utilities
{
    /// <summary>
    /// <see cref="Component"/> used to cache <see cref="UnityEngine.Bounds"/>s information about this <see cref="UnityEngine.GameObject"/>
    /// </summary>
    public class BoundsCache : MonoBehaviour
    {
        private Bounds bounds = default;

        /// <summary>
        /// The bounds of the object
        /// </summary>
        public Bounds Bounds => bounds;

        [SerializeField]
        private bool useColliderData = true;

        [SerializeField]
        private Collider[] colliders = null;

        /// <summary>
        /// The collection of <see cref="Collider"/>s for this object.
        /// </summary>
        public Collider[] Colliders => colliders;

        [SerializeField]
        private bool useRendererData = true;

        [SerializeField]
        private Renderer[] renderers = null;

        /// <summary>
        /// The collection of <see cref="Renderer"/>s for this object.
        /// </summary>
        public Renderer[] Renderers => renderers;

        private Vector3[] boundsCornerPoints = new Vector3[0];

        /// <summary>
        /// The bounds corner points in world space.
        /// </summary>
        public Vector3[] BoundsCornerPoints => boundsCornerPoints;

        private void Start()
        {
            if (useColliderData)
            {
                bounds.Encapsulate(transform.GetColliderBounds(ref colliders));
            }

            if (useRendererData)
            {
                bounds.Encapsulate(transform.GetRenderBounds(ref renderers));
            }

            if (!bounds.IsValid())
            {
                Debug.LogError($"Bounds calculation was invalid for {gameObject.name}!");
                return;
            }

            bounds.GetCornerPositionsWorldSpace(transform, ref boundsCornerPoints);
        }

        private void Update()
        {
            if (bounds.IsValid())
            {
                // update the bounds corner points.
                bounds.GetCornerPositionsWorldSpace(transform, ref boundsCornerPoints);
            }
        }
    }
}