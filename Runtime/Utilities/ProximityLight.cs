﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Utilities
{
    /// <summary>
    /// Utility component to animate and visualize a light that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_ProximityLight" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class ProximityLight : MonoBehaviour
    {
        // Two proximity lights are supported at this time.
        private const int PROXIMITY_LIGHT_COUNT = 2;
        private const int PROXIMITY_LIGHT_DATA_SIZE = 6;
        private static readonly List<ProximityLight> ActiveProximityLights = new List<ProximityLight>(PROXIMITY_LIGHT_COUNT);
        private static readonly Vector4[] ProximityLightData = new Vector4[PROXIMITY_LIGHT_COUNT * PROXIMITY_LIGHT_DATA_SIZE];
        private static int proximityLightDataId;
        private static int lastProximityLightUpdate = -1;

        [Serializable]
        public class LightSettings
        {
            /// <summary>
            /// Specifies the radius of the ProximityLight effect when near to a surface.
            /// </summary>
            public float NearRadius
            {
                get => nearRadius;
                set => nearRadius = value;
            }

            [Header("Proximity Settings")]
            [Tooltip("Specifies the radius of the ProximityLight effect when near to a surface.")]
            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float nearRadius = 0.05f;

            /// <summary>
            /// Specifies the radius of the ProximityLight effect when far from a surface.
            /// </summary>
            public float FarRadius
            {
                get => farRadius;
                set => farRadius = value;
            }

            [Tooltip("Specifies the radius of the ProximityLight effect when far from a surface.")]
            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float farRadius = 0.2f;

            /// <summary>
            /// Specifies the distance a ProximityLight must be from a surface to be considered near.
            /// </summary>
            public float NearDistance
            {
                get => nearDistance;
                set => nearDistance = value;
            }

            [Tooltip("Specifies the distance a ProximityLight must be from a surface to be considered near.")]
            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float nearDistance = 0.02f;

            /// <summary>
            /// When a ProximityLight is near, the smallest size percentage from the far size it can shrink to.
            /// </summary>
            public float MinNearSizePercentage
            {
                get => minNearSizePercentage;
                set => minNearSizePercentage = value;
            }

            [Tooltip("When a ProximityLight is near, the smallest size percentage from the far size it can shrink to.")]
            [SerializeField]
            [Range(0.0f, 1.0f)]
            private float minNearSizePercentage = 0.35f;

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color CenterColor
            {
                get => centerColor;
                set => centerColor = value;
            }

            [Header("Color Settings")]
            [Tooltip("The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.")]
            [ColorUsage(true, true)]
            [SerializeField]
            private Color centerColor = new Color(20.0f / 255.0f, 121.0f / 255.0f, 250.0f / 255.0f, 0.0f / 255.0f);

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color MiddleColor
            {
                get => middleColor;
                set => middleColor = value;
            }

            [Tooltip("The color of the ProximityLight gradient at the middle (RGB) and (A) is gradient extent.")]
            [SerializeField]
            [ColorUsage(true, true)]
            private Color middleColor = new Color(3.0f / 255.0f, 111.0f / 255.0f, 255.0f / 255.0f, 84.0f / 255.0f);

            /// <summary>
            /// The color of the ProximityLight gradient at the center (RGB) and (A) is gradient extent.
            /// </summary>
            public Color OuterColor
            {
                get => outerColor;
                set => outerColor = value;
            }

            [Tooltip("The color of the ProximityLight gradient at the outer (RGB) and (A) is gradient extent.")]
            [SerializeField]
            [ColorUsage(true, true)]
            private Color outerColor = new Color((70.0f * 5.0f) / 255.0f, (0.0f * 5.0f) / 255.0f, (191.0f * 5.0f) / 255.0f, 255.0f / 255.0f);
        }

        /// <summary>
        /// Current light settings for this component.
        /// </summary>
        public LightSettings Settings
        {
            get => settings;
            set => settings = value;
        }

        [SerializeField]
        private LightSettings settings = new LightSettings();

        private float pulseTime;
        private float pulseFade;

        /// <summary>
        /// Initiates a pulse, if one is not already occurring, which simulates a user touching a surface.
        /// </summary>
        /// <param name="pulseDuration">How long in seconds should the pulse animate over.</param>
        /// <param name="fadeBegin">At what point during the pulseDuration should the pulse begin to fade out as a percentage. Range should be [0, 1].</param>
        /// <param name="fadeSpeed">The speed to fade in and out.</param>
        public void Pulse(float pulseDuration = 0.2f, float fadeBegin = 0.8f, float fadeSpeed = 10.0f)
        {
            if (pulseTime <= 0.0f)
            {
                StartCoroutine(PulseRoutine(pulseDuration, fadeBegin, fadeSpeed));
            }
        }

        private void OnEnable()
        {
            AddProximityLight(this);
        }

        private void OnDisable()
        {
            RemoveProximityLight(this);
            UpdateProximityLights(true);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateProximityLights();
        }
#endif // UNITY_EDITOR

        private void LateUpdate()
        {
            UpdateProximityLights();
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled)
            {
                return;
            }

            Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

            Gizmos.color = new Color(Settings.CenterColor.r, Settings.CenterColor.g, Settings.CenterColor.b);
            Gizmos.DrawWireSphere(transform.position, Settings.NearRadius);

            foreach (var direction in directions)
            {
                Gizmos.DrawIcon(transform.position + direction * Settings.NearRadius, string.Empty, false);
            }

            Gizmos.color = new Color(Settings.OuterColor.r, Settings.OuterColor.g, Settings.OuterColor.b);
            Gizmos.DrawWireSphere(transform.position, Settings.FarRadius);

            foreach (var direction in directions)
            {
                Gizmos.DrawIcon(transform.position + direction * Settings.FarRadius, string.Empty, false);
            }
        }

        private static void AddProximityLight(ProximityLight light)
        {
            if (ActiveProximityLights.Count >= PROXIMITY_LIGHT_COUNT)
            {
                Debug.LogWarning($"Max proximity light count ({PROXIMITY_LIGHT_COUNT}) exceeded.");
            }

            ActiveProximityLights.Add(light);
        }

        private static void RemoveProximityLight(ProximityLight light)
        {
            ActiveProximityLights.Remove(light);
        }

        private static void Initialize()
        {
            proximityLightDataId = Shader.PropertyToID("_ProximityLightData");
        }

        private static void UpdateProximityLights(bool forceUpdate = false)
        {
            if (lastProximityLightUpdate == -1)
            {
                Initialize();
            }

            if (!forceUpdate && (Time.frameCount == lastProximityLightUpdate))
            {
                return;
            }

            for (int i = 0; i < PROXIMITY_LIGHT_COUNT; ++i)
            {
                var light = (i >= ActiveProximityLights.Count) ? null : ActiveProximityLights[i];
                int dataIndex = i * PROXIMITY_LIGHT_DATA_SIZE;

                if (light)
                {
                    var lightPosition = light.transform.position;

                    ProximityLightData[dataIndex] = new Vector4(lightPosition.x,
                                                                lightPosition.y,
                                                                lightPosition.z,
                                                                1.0f);
                    var pulseScaler = 1.0f + light.pulseTime;
                    ProximityLightData[dataIndex + 1] = new Vector4(light.Settings.NearRadius * pulseScaler,
                                                                    1.0f / Mathf.Clamp(light.Settings.FarRadius * pulseScaler, 0.001f, 1.0f),
                                                                    1.0f / Mathf.Clamp(light.Settings.NearDistance * pulseScaler, 0.001f, 1.0f),
                                                                    Mathf.Clamp01(light.Settings.MinNearSizePercentage));
                    ProximityLightData[dataIndex + 2] = new Vector4(light.Settings.NearDistance * light.pulseTime,
                                                                    Mathf.Clamp01(1.0f - light.pulseFade),
                                                                    0.0f,
                                                                    0.0f);
                    ProximityLightData[dataIndex + 3] = light.Settings.CenterColor;
                    ProximityLightData[dataIndex + 4] = light.Settings.MiddleColor;
                    ProximityLightData[dataIndex + 5] = light.Settings.OuterColor;
                }
                else
                {
                    ProximityLightData[dataIndex] = Vector4.zero;
                }
            }

            Shader.SetGlobalVectorArray(proximityLightDataId, ProximityLightData);

            lastProximityLightUpdate = Time.frameCount;
        }

        private IEnumerator PulseRoutine(float pulseDuration, float fadeBegin, float fadeSpeed)
        {
            var pulseTimer = 0.0f;

            while (pulseTimer < pulseDuration)
            {
                pulseTimer += Time.deltaTime;
                pulseTime = pulseTimer / pulseDuration;

                if (pulseTime > fadeBegin)
                {
                    pulseFade += Time.deltaTime;
                }

                yield return null;
            }

            while (pulseFade < 1.0f)
            {
                pulseFade += Time.deltaTime * fadeSpeed;

                yield return null;
            }

            pulseTime = 0.0f;

            while (pulseFade > 0.0f)
            {
                pulseFade -= Time.deltaTime * fadeSpeed;

                yield return null;
            }

            pulseFade = 0.0f;
        }
    }
}
