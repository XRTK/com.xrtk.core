// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace XRTK.Utilities.Rendering
{
    /// <summary>
    /// Component to animate and visualize a plane that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingPlane : ClippingPrimitive
    {
        private int clipPlaneId;

        private Vector4 planeSize = Vector4.zero;

        private readonly Vector3 gizmoSize = new Vector3(1.0f, 0.0f, 1.0f);

        protected override string Keyword => "_CLIPPING_PLANE";

        protected override string KeywordProperty => "_ClippingPlane";

        protected override string ClippingSideProperty => "_ClipPlaneSide";

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, gizmoSize);
                Gizmos.DrawLine(Vector3.zero, Vector3.up * -0.5f);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            clipPlaneId = Shader.PropertyToID("_ClipPlane");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            var clipTransform = transform;
            var clipTransformUp = clipTransform.up;
            planeSize.x = clipTransformUp.x;
            planeSize.y = clipTransformUp.y;
            planeSize.z = clipTransformUp.z;
            planeSize.w = Vector3.Dot(clipTransformUp, clipTransform.position);
            materialPropertyBlock.SetVector(clipPlaneId, planeSize);
        }
    }
}