// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Utilities.Lines.Renderers
{
    /// <summary>
    /// Creates instances of a mesh along the line
    /// </summary>
    public class MeshLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Instanced Mesh Settings")]

        [SerializeField]
        private Mesh lineMesh = null;

        public Mesh LineMesh
        {
            get => lineMesh;
            set
            {
                lineMesh = value;

                if (!IsInitialized)
                {
                    enabled = false;
                }
            }
        }

        [SerializeField]
        private Material lineMaterial = null;

        public Material LineMaterial
        {
            get => lineMaterial;
            set
            {
                lineMaterial = value;

                if (!IsInitialized)
                {
                    enabled = false;
                }
                else
                {
                    lineMaterial.enableInstancing = true;
                }
            }
        }

        [SerializeField]
        private string colorProperty = "_Color";

        [Range(0, 10)]
        [SerializeField]
        [Tooltip("How many line steps to skip before a mesh is drawn")]
        private int lineStepSkip = 0;

        public string ColorProperty
        {
            get => colorProperty;
            set
            {
                colorProperty = value;

                if (!IsInitialized)
                {
                    enabled = false;
                }
            }
        }

        private bool IsInitialized
        {
            get
            {
                if (lineMesh != null &&
                    lineMaterial != null &&
                    lineMaterial.HasProperty(colorProperty))
                {
                    return true;
                }

                Debug.Assert(lineMesh != null, "Missing assigned line mesh.");
                Debug.Assert(lineMaterial != null, "Missing assigned line material.");
                Debug.Assert(lineMaterial != null && lineMaterial.HasProperty(colorProperty), $"Unable to find the property \"{colorProperty}\" for the line material");
                return false;
            }
        }

        private int colorId;
        private MaterialPropertyBlock linePropertyBlock;

        private readonly List<Vector4> colorValues = new List<Vector4>();
        private readonly List<Matrix4x4> meshTransforms = new List<Matrix4x4>();

        private void OnValidate()
        {
            enabled = IsInitialized;
        }

        protected virtual void OnEnable()
        {
            if (!IsInitialized)
            {
                enabled = false;
                return;
            }

            if (linePropertyBlock == null)
            {
                linePropertyBlock = new MaterialPropertyBlock();
            }
        }

        protected override void UpdateLine()
        {
            if (IsInitialized && LineDataSource.enabled)
            {
                meshTransforms.Clear();
                colorValues.Clear();

                int skipCount = 0;

                for (int i = 0; i < LineStepCount; i++)
                {
                    if (lineStepSkip > 0)
                    {
                        skipCount++;

                        if (skipCount < lineStepSkip)
                        {
                            continue;
                        }

                        skipCount = 0;
                    }

                    float normalizedDistance = GetNormalizedPointAlongLine(i);
                    colorValues.Add(GetColor(normalizedDistance));
                    meshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance),
                                                     LineDataSource.GetRotation(normalizedDistance),
                                                     Vector3.one * GetWidth(normalizedDistance)));
                }

                colorId = Shader.PropertyToID(colorProperty);
                linePropertyBlock.Clear();
                linePropertyBlock.SetVectorArray(colorId, colorValues);
                Graphics.DrawMeshInstanced(lineMesh, 0, lineMaterial, meshTransforms, linePropertyBlock);
            }
        }
    }
}