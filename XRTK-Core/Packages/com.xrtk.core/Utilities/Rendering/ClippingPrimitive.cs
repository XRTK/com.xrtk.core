// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Utilities.Rendering
{
    /// <summary>
    /// An abstract primitive component to animate and visualize a clipping primitive that can be
    /// used to drive per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class ClippingPrimitive : MonoBehaviour
    {
        public enum Side
        {
            Inside = 1,
            Outside = -1
        }

        [SerializeField]
        [Tooltip("The renderer(s) that should be affected by the primitive.")]
        private List<Renderer> renderers = new List<Renderer>();


        [SerializeField]
        [Tooltip("Which side of the primitive to clip pixels against.")]
        private Side clippingSide = Side.Inside;

        /// <summary>
        /// The renderer(s) that should be affected by the primitive.
        /// </summary>
        public Side ClippingSide => clippingSide;

        protected abstract string Keyword { get; }

        protected abstract string KeywordProperty { get; }

        protected abstract string ClippingSideProperty { get; }

        private MaterialPropertyBlock materialPropertyBlock;
        private readonly Dictionary<Material, bool> modifiedMaterials = new Dictionary<Material, bool>();
        private readonly List<Material> allocatedMaterials = new List<Material>();

        private int clippingSideId;

        /// <summary>
        /// Add the specified renderer to the clipping component.
        /// </summary>
        /// <param name="renderer_"></param>
        public void AddRenderer(Renderer renderer_)
        {
            if (renderer_ != null && !renderers.Contains(renderer_))
            {
                renderers.Add(renderer_);

                var material = GetMaterial(renderer_, false);

                if (material != null)
                {
                    ToggleClippingFeature(material, true);
                }
            }
        }

        /// <summary>
        /// Add the specified renderers to the clipping component.
        /// </summary>
        /// <param name="renderers_"></param>
        public void AddRenderers(Renderer[] renderers_)
        {
            for (int i = 0; i < renderers_.Length; i++)
            {
                AddRenderer(renderers_[i]);
            }
        }

        /// <summary>
        /// Remove the specified renderer from the clipping component.
        /// </summary>
        /// <param name="renderer_"></param>
        public void RemoveRenderer(Renderer renderer_)
        {
            if (renderers.Contains(renderer_))
            {
                var material = GetMaterial(renderer_, false);

                if (material != null)
                {
                    ToggleClippingFeature(material, false);
                }

                renderers.Remove(renderer_);
            }
        }

        /// <summary>
        /// clear all the renderers from this clipping component.
        /// </summary>
        public void ClearRenderers()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                var material = GetMaterial(renderers[i], false);

                if (material != null)
                {
                    ToggleClippingFeature(material, false);
                }
            }

            renderers.Clear();
        }

        #region Monobehaviour Implementation

        protected void OnValidate()
        {
            ToggleClippingFeature(true);
            RestoreUnassignedMaterials();
        }

        protected void OnEnable()
        {
            Initialize();
            UpdateRenderers();
            ToggleClippingFeature(true);
        }

        protected void OnDisable()
        {
            UpdateRenderers();
            ToggleClippingFeature(false);
        }

#if UNITY_EDITOR
        // We need this class to be updated once per frame even when in edit mode. Ideally this would
        // occur after all other objects are updated in LateUpdate(), but because the ExecuteInEditMode
        // attribute only invokes Update() we handle edit mode updating in Update() and runtime updating
        // in LateUpdate().
        protected void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateRenderers();
        }
#endif

        protected void LateUpdate()
        {
            UpdateRenderers();
        }

        protected void OnDestroy()
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                var material = GetMaterial(renderers[i]);

                if (material != null &&
                    modifiedMaterials.TryGetValue(material, out bool clippingPlaneOn))
                {
                    ToggleClippingFeature(material, clippingPlaneOn);
                    modifiedMaterials.Remove(material);
                }
            }

            RestoreUnassignedMaterials();

            for (int i = 0; i < allocatedMaterials.Count; ++i)
            {
                Destroy(allocatedMaterials[i]);
            }
        }

        #endregion  Monobehaviour Implementation

        protected virtual void Initialize()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            clippingSideId = Shader.PropertyToID(ClippingSideProperty);
        }

        protected virtual void UpdateRenderers()
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                var _renderer = renderers[i];

                if (_renderer == null)
                {
                    continue;
                }

                _renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(clippingSideId, (float)clippingSide);
                UpdateShaderProperties(materialPropertyBlock);
                _renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        protected abstract void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock);

        protected void ToggleClippingFeature(bool keywordOn)
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                var material = GetMaterial(renderers[i]);

                if (material != null)
                {
                    // Cache the initial keyword state of the material.
                    if (!modifiedMaterials.ContainsKey(material))
                    {
                        modifiedMaterials[material] = material.IsKeywordEnabled(Keyword);
                    }

                    ToggleClippingFeature(material, keywordOn);
                }
            }
        }

        protected void ToggleClippingFeature(Material material, bool keywordOn)
        {
            if (keywordOn)
            {
                material.EnableKeyword(Keyword);
                material.SetFloat(KeywordProperty, 1.0f);
            }
            else
            {
                material.DisableKeyword(Keyword);
                material.SetFloat(KeywordProperty, 0.0f);
            }
        }

        protected Material GetMaterial(Renderer _renderer, bool trackAllocations = true)
        {
            if (_renderer == null)
            {
                return null;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                return _renderer.sharedMaterial;
            }

            var material = _renderer.material;

            if (trackAllocations && !allocatedMaterials.Contains(material))
            {
                allocatedMaterials.Add(material);
            }

            return material;
        }

        protected void RestoreUnassignedMaterials()
        {
            var toRemove = new List<Material>();

            foreach (var modifiedMaterial in modifiedMaterials)
            {
                if (modifiedMaterial.Key == null)
                {
                    toRemove.Add(modifiedMaterial.Key);
                }
                else if (renderers.Find(_renderer => GetMaterial(_renderer) == modifiedMaterial.Key) == null)
                {
                    ToggleClippingFeature(modifiedMaterial.Key, modifiedMaterial.Value);
                    toRemove.Add(modifiedMaterial.Key);
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                modifiedMaterials.Remove(toRemove[i]);
            }
        }
    }
}