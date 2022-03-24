// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Utilities
{
    /// <summary>
    /// Utility component to animate and visualize a light that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_HoverLight" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class HoverLight : MonoBehaviour
    {
        // Three hover lights are supported at this time.
        private const int HOVER_LIGHT_COUNT = 3;
        private const int HOVER_LIGHT_DATA_SIZE = 2;
        private const string MULTI_HOVER_LIGHT_KEYWORD = "_MULTI_HOVER_LIGHT";
        private static readonly List<HoverLight> ActiveHoverLights = new List<HoverLight>(HOVER_LIGHT_COUNT);
        private static readonly Vector4[] HoverLightData = new Vector4[HOVER_LIGHT_COUNT * HOVER_LIGHT_DATA_SIZE];
        private static int hoverLightDataId;
        private static int lastHoverLightUpdate = -1;

        /// <summary>
        /// Specifies the Radius of the HoverLight effect
        /// </summary>
        public float Radius => radius;

        [Tooltip("Specifies the radius of the HoverLight effect")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float radius = 0.15f;

        /// <summary>
        /// Specifies the highlight color
        /// </summary>
        public Color Color => color;

        [Tooltip("Specifies the highlight color")]
        [SerializeField]
        private Color color = new Color(0.3f, 0.3f, 0.3f, 1.0f);

        private void OnEnable()
        {
            AddHoverLight(this);
        }

        private void OnDisable()
        {
            RemoveHoverLight(this);
            UpdateHoverLights(true);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateHoverLights();
        }
#endif // UNITY_EDITOR

        private void LateUpdate()
        {
            UpdateHoverLights();
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled)
            {
                return;
            }

            var position = transform.position;

            Gizmos.color = Color;
            Gizmos.DrawWireSphere(position, Radius);
            Gizmos.DrawIcon(position + Vector3.right * Radius, string.Empty, false);
            Gizmos.DrawIcon(position + Vector3.left * Radius, string.Empty, false);
            Gizmos.DrawIcon(position + Vector3.up * Radius, string.Empty, false);
            Gizmos.DrawIcon(position + Vector3.down * Radius, string.Empty, false);
            Gizmos.DrawIcon(position + Vector3.forward * Radius, string.Empty, false);
            Gizmos.DrawIcon(position + Vector3.back * Radius, string.Empty, false);
        }

        private void AddHoverLight(HoverLight hoverLight)
        {
            if (ActiveHoverLights.Count >= HOVER_LIGHT_COUNT)
            {
                Debug.LogWarning($"Max hover hoverLight count ({HOVER_LIGHT_COUNT}) exceeded.");
            }

            ActiveHoverLights.Add(hoverLight);
        }

        private void RemoveHoverLight(HoverLight hoverLight)
        {
            ActiveHoverLights.Remove(hoverLight);
        }

        private void Initialize()
        {
            hoverLightDataId = Shader.PropertyToID("_HoverLightData");
        }

        private void UpdateHoverLights(bool forceUpdate = false)
        {
            if (lastHoverLightUpdate == -1)
            {
                Initialize();
            }

            if (!forceUpdate && (Time.frameCount == lastHoverLightUpdate))
            {
                return;
            }

            if (ActiveHoverLights.Count > 1)
            {
                Shader.EnableKeyword(MULTI_HOVER_LIGHT_KEYWORD);
            }
            else
            {
                Shader.DisableKeyword(MULTI_HOVER_LIGHT_KEYWORD);
            }

            for (int i = 0; i < HOVER_LIGHT_COUNT; ++i)
            {
                var hoverLight = (i >= ActiveHoverLights.Count) ? null : ActiveHoverLights[i];
                int dataIndex = i * HOVER_LIGHT_DATA_SIZE;

                if (hoverLight)
                {
                    var lightPosition = hoverLight.transform.position;
                    HoverLightData[dataIndex] = new Vector4(lightPosition.x,
                                                            lightPosition.y,
                                                            lightPosition.z,
                                                            1.0f);
                    HoverLightData[dataIndex + 1] = new Vector4(hoverLight.Color.r,
                                                                hoverLight.Color.g,
                                                                hoverLight.Color.b,
                                                                1.0f / Mathf.Clamp(hoverLight.Radius, 0.001f, 1.0f));
                }
                else
                {
                    HoverLightData[dataIndex] = Vector4.zero;
                }
            }

            Shader.SetGlobalVectorArray(hoverLightDataId, HoverLightData);

            lastHoverLightUpdate = Time.frameCount;
        }
    }
}