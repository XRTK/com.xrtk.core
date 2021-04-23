// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace XRTK.Utilities.Rendering
{
    /// <summary>
    /// Component to animate and visualize a box that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingBox : ClippingPrimitive
    {
        private int clipBoxSizeId;
        private int clipBoxInverseTransformId;
        private Vector4 boxSize = Vector4.zero;

        protected override string Keyword => "_CLIPPING_BOX";

        protected override string KeywordProperty => "_ClippingBox";

        protected override string ClippingSideProperty => "_ClipBoxSide";

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            clipBoxSizeId = Shader.PropertyToID("_ClipBoxSize");
            clipBoxInverseTransformId = Shader.PropertyToID("_ClipBoxInverseTransform");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock _materialPropertyBlock)
        {
            var _transform = transform;
            Vector3 lossyScale = _transform.lossyScale * 0.5f;
            boxSize.x = lossyScale.x;
            boxSize.y = lossyScale.y;
            boxSize.z = lossyScale.z;
            boxSize.w = 0.0f;
            _materialPropertyBlock.SetVector(clipBoxSizeId, boxSize);
            Matrix4x4 boxInverseTransform = Matrix4x4.TRS(_transform.position, _transform.rotation, Vector3.one).inverse;
            _materialPropertyBlock.SetMatrix(clipBoxInverseTransformId, boxInverseTransform);
        }
    }
}