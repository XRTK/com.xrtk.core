// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace XRTK.Utilities.Rendering
{
    /// <summary>
    /// Component to animate and visualize a sphere that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingSphere : ClippingPrimitive
    {
        [Tooltip("The radius of the clipping sphere.")]
        [SerializeField]
        private float radius = 0.5f;

        private Vector4 sphereSize = Vector4.zero;

        /// <summary>
        /// The radius of the clipping sphere.
        /// </summary>
        public float Radius => radius;

        private int clipSphereId;

        protected override string Keyword => "_CLIPPING_SPHERE";

        protected override string KeywordProperty => "_ClippingSphere";

        protected override string ClippingSideProperty => "_ClipSphereSide";

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            clipSphereId = Shader.PropertyToID("_ClipSphere");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            var clipTransformPosition = transform.position;
            sphereSize.x = clipTransformPosition.x;
            sphereSize.y = clipTransformPosition.y;
            sphereSize.z = clipTransformPosition.z;
            sphereSize.w = radius;
            materialPropertyBlock.SetVector(clipSphereId, sphereSize);
        }
    }
}