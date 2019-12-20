// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Globalization;
using System.Text;
using UnityEngine;
using XRTK.Utilities;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;

#if WINDOWS_UWP
using Windows.System;
#else
using UnityEngine.Profiling;
#endif

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default profiler diagnostics visualizer implementation.
    /// </summary>
    public class DefaultMixedRealityDiagnosticsVisualizer : BaseMixedRealityDiagnosticsHandler
    {
        private static readonly int MaxStringLength = 32;
        private static readonly int MaxTargetFrameRate = 120;
        private static readonly int FrameRange = 30;

        [Header("Profiler")]
        [SerializeField]
        private TextMeshProUGUI

        private static readonly Vector2 DefaultWindowRotation = new Vector2(10.0f, 20.0f);
        private static readonly Vector3 DefaultWindowScale = new Vector3(0.2f, 0.04f, 1.0f);

        private static readonly string UsedMemoryString = "Used: ";
        private static readonly string PeakMemoryString = "Peak: ";
        private static readonly string LimitMemoryString = "Limit: ";

        private readonly int colorId = Shader.PropertyToID("_Color");
        private readonly int parentMatrixId = Shader.PropertyToID("_ParentLocalToWorldMatrix");

        private readonly char[] stringBuffer = new char[MaxStringLength];


        public MixedRealityDiagnosticsDataProviderProfile profile;

        /// <summary>
        /// The <see cref="Transform"/> parent of the <see cref="MixedRealityToolkitVisualProfiler"/>
        /// </summary>
        public Transform WindowParent { get; set; } = null;

        [Header("Profiler Settings")]

        [SerializeField]
        [Tooltip("Is the profiler currently visible.")]
        private bool isVisible = false;

        [Header("Window Settings")]

        [SerializeField]
        [Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        /// <summary>
        /// What part of the view port to anchor the window to.
        /// </summary>
        public TextAnchor WindowAnchor
        {
            get => windowAnchor;
            set => windowAnchor = value;
        }

        [SerializeField]
        [Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        /// <summary>
        /// The offset from the view port center applied based on the window anchor selection.
        /// </summary>
        public Vector2 WindowOffset
        {
            get => windowOffset;
            set => windowOffset = value;
        }

        [SerializeField]
        [Range(0.5f, 5.0f)]
        [Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 1.0f;

        /// <summary>
        /// Use to scale the window size up or down, can simulate a zooming effect.
        /// </summary>
        public float WindowScale
        {
            get => windowScale;
            set => windowScale = Mathf.Clamp(value, 0.5f, 5.0f);
        }

        [SerializeField]
        [Range(0.0f, 100.0f)]
        [Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 5.0f;

        /// <summary>
        /// How quickly to interpolate the window towards its target position and rotation.
        /// </summary>
        public float WindowFollowSpeed
        {
            get => windowFollowSpeed;
            set => windowFollowSpeed = Mathf.Abs(value);
        }

        private GameObject window;
        private TextMesh applicationDetailsText;
        private TextMesh cpuFrameRateText;
        private TextMesh gpuFrameRateText;
        private TextMesh usedMemoryText;
        private TextMesh peakMemoryText;
        private TextMesh limitMemoryText;
        private Transform usedAnchor;
        private Transform peakAnchor;
        private Quaternion windowHorizontalRotation;
        private Quaternion windowHorizontalRotationInverse;
        private Quaternion windowVerticalRotation;
        private Quaternion windowVerticalRotationInverse;

        private Matrix4x4[] frameInfoMatrices;
        private Vector4[] frameInfoColors;
        private MaterialPropertyBlock frameInfoPropertyBlock;

        private string[] cpuFrameRateStrings;
        private string[] gpuFrameRateStrings;

        private ulong memoryUsage;
        private ulong peakMemoryUsage;
        private ulong limitMemoryUsage;

        // Rendering resources.
        [SerializeField]
        [HideInInspector]
        private Material defaultMaterial;

        [SerializeField]
        [HideInInspector]
        private Material defaultInstancedMaterial;

        private Material backgroundMaterial;
        private Material foregroundMaterial;
        private Material textMaterial;
        private Mesh quadMesh;
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");

        private void Reset()
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                defaultMaterial.SetFloat(ZWrite, 0.0f);
                defaultMaterial.SetFloat(ZTest, (float)UnityEngine.Rendering.CompareFunction.Disabled);
                defaultMaterial.renderQueue = 5000;
            }

            if (defaultInstancedMaterial == null)
            {
                var defaultInstancedShader = Shader.Find("Hidden/Instanced-Colored");

                if (defaultInstancedShader != null)
                {
                    defaultInstancedMaterial = new Material(defaultInstancedShader)
                    {
                        enableInstancing = true
                    };

                    defaultInstancedMaterial.SetFloat(ZWrite, 0.0f);
                    defaultInstancedMaterial.SetFloat(ZTest, (float)UnityEngine.Rendering.CompareFunction.Disabled);
                    defaultInstancedMaterial.renderQueue = 5000;
                }
                else
                {
                    Debug.LogWarning("A shader supporting instancing could not be found for the VisualProfiler, falling back to traditional rendering. This may impact performance.");
                }
            }

            if (Application.isPlaying)
            {
                backgroundMaterial = new Material(defaultMaterial);
                foregroundMaterial = new Material(defaultMaterial);
                defaultMaterial.renderQueue = foregroundMaterial.renderQueue - 1;
                backgroundMaterial.renderQueue = defaultMaterial.renderQueue - 1;

                var meshRenderer = new GameObject().AddComponent<TextMesh>().GetComponent<MeshRenderer>();

                textMaterial = new Material(meshRenderer.sharedMaterial)
                {
                    renderQueue = defaultMaterial.renderQueue
                };

                Destroy(meshRenderer.gameObject);

                var quadMeshFilter = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>();

                if (defaultInstancedMaterial != null)
                {
                    // Create a quad mesh with artificially large bounds to disable culling for instanced rendering.
                    // TODO: Use shared mesh with normal bounds once Unity allows for more control over instance culling.
                    quadMesh = quadMeshFilter.mesh;
                    quadMesh.bounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);
                }
                else
                {
                    quadMesh = quadMeshFilter.sharedMesh;
                }

                Destroy(quadMeshFilter.gameObject);
            }
        }

        private void Start()
        {
            Reset();
            BuildWindow();
            BuildFrameRateStrings();
        }

        private void OnDestroy()
        {
            Destroy(window);
        }

        /// <inheritdoc />
        public override void UpdateDiagnostics(DiagnosticsData data)
        {
            if (window == null)
            {
                return;
            }

            // Update window transformation.
            var cameraTransform = CameraCache.Main.transform;

            if (window.activeSelf)
            {
                var time = Time.deltaTime * windowFollowSpeed;
                window.transform.position = Vector3.Lerp(window.transform.position, CalculateWindowPosition(cameraTransform), time);
                window.transform.rotation = Quaternion.Slerp(window.transform.rotation, CalculateWindowRotation(cameraTransform), time);
                window.transform.localScale = DefaultWindowScale * windowScale;
            }

            // Draw frame info.
            if (window.activeSelf)
            {
                var parentLocalToWorldMatrix = window.transform.localToWorldMatrix;

                if (defaultInstancedMaterial != null)
                {
                    frameInfoPropertyBlock.SetMatrix(parentMatrixId, parentLocalToWorldMatrix);
                    Graphics.DrawMeshInstanced(quadMesh, 0, defaultInstancedMaterial, frameInfoMatrices, frameInfoMatrices.Length, frameInfoPropertyBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
                }
                else
                {
                    // If a instanced material is not available, fall back to non-instanced rendering.
                    for (int i = 0; i < frameInfoMatrices.Length; ++i)
                    {
                        frameInfoPropertyBlock.SetColor(colorId, frameInfoColors[i]);
                        Graphics.DrawMesh(quadMesh, parentLocalToWorldMatrix * frameInfoMatrices[i], defaultMaterial, 0, null, 0, frameInfoPropertyBlock, false, false, false);
                    }
                }
            }

            window.SetActive(isVisible);
        }

        private Vector3 CalculateWindowPosition(Transform cameraTransform)
        {
            var windowDistance = Mathf.Max(16.0f / CameraCache.Main.fieldOfView, CameraCache.Main.nearClipPlane + 0.25f);
            var position = cameraTransform.position + (cameraTransform.forward * windowDistance);
            var horizontalOffset = cameraTransform.right * windowOffset.x;
            var verticalOffset = cameraTransform.up * windowOffset.y;

            switch (windowAnchor)
            {
                case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
                case TextAnchor.UpperCenter: position += verticalOffset; break;
                case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
                case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
                case TextAnchor.MiddleRight: position += horizontalOffset; break;
                case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
                case TextAnchor.LowerCenter: position -= verticalOffset; break;
                case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
            }

            return position;
        }

        private Quaternion CalculateWindowRotation(Transform cameraTransform)
        {
            var rotation = cameraTransform.rotation;

            switch (windowAnchor)
            {
                case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
                case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
                case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
                case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
                case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
                case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
                case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
                case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
            }

            return rotation;
        }

        private void BuildWindow()
        {
            // Build the window root.
            {
                window = CreateQuad("VisualProfiler", null);
                window.transform.parent = WindowParent;
                //InitializeRenderer(window, backgroundMaterial, colorId, profile.WindowBackgroundColor);
                window.transform.localScale = DefaultWindowScale;
                windowHorizontalRotation = Quaternion.AngleAxis(DefaultWindowRotation.y, Vector3.right);
                windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
                windowVerticalRotation = Quaternion.AngleAxis(DefaultWindowRotation.x, Vector3.up);
                windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
            }

            // Add Application details text
            {
                applicationDetailsText = CreateText("ApplicationDetails", new Vector3(0.495f, 0.75f, 0.0f), window.transform, TextAnchor.UpperRight, textMaterial, Color.white, string.Empty);
                applicationDetailsText.text = $"{Application.productName} v{Application.version}";
            }

            // Add frame rate text and frame indicators.
            {
                cpuFrameRateText = CreateText("CPUFrameRateText", new Vector3(-0.495f, 0.5f, 0.0f), window.transform, TextAnchor.UpperLeft, textMaterial, Color.white, string.Empty);
                gpuFrameRateText = CreateText("GPUFrameRateText", new Vector3(0.495f, 0.5f, 0.0f), window.transform, TextAnchor.UpperRight, textMaterial, Color.white, string.Empty);
                gpuFrameRateText.gameObject.SetActive(false);

                frameInfoMatrices = new Matrix4x4[FrameRange];
                frameInfoColors = new Vector4[FrameRange];
                var scale = new Vector3(1.0f / FrameRange, 0.2f, 1.0f);
                var position = new Vector3(0.5f - (scale.x * 0.5f), 0.15f, 0.0f);

                for (int i = 0; i < FrameRange; ++i)
                {
                    frameInfoMatrices[i] = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(scale.x * 0.8f, scale.y, scale.z));
                    position.x -= scale.x;
                    frameInfoColors[i] = profile.TargetFrameRateColor;
                }

                frameInfoPropertyBlock = new MaterialPropertyBlock();
                frameInfoPropertyBlock.SetVectorArray(colorId, frameInfoColors);
            }

            // Add memory usage text and bars.
            {
                usedMemoryText = CreateText("UsedMemoryText", new Vector3(-0.495f, 0.0f, 0.0f), window.transform, TextAnchor.UpperLeft, textMaterial, profile.MemoryUsedColor, UsedMemoryString);
                peakMemoryText = CreateText("PeakMemoryText", new Vector3(0.0f, 0.0f, 0.0f), window.transform, TextAnchor.UpperCenter, textMaterial, profile.MemoryPeakColor, PeakMemoryString);
                limitMemoryText = CreateText("LimitMemoryText", new Vector3(0.495f, 0.0f, 0.0f), window.transform, TextAnchor.UpperRight, textMaterial, Color.white, LimitMemoryString);

                var limitBar = CreateQuad("LimitBar", window.transform);
                InitializeRenderer(limitBar, defaultMaterial, colorId, profile.MemoryLimitColor);
                limitBar.transform.localScale = new Vector3(0.99f, 0.2f, 1.0f);
                limitBar.transform.localPosition = new Vector3(0.0f, -0.37f, 0.0f);

                {
                    usedAnchor = CreateAnchor("UsedAnchor", limitBar.transform);
                    var bar = CreateQuad("UsedBar", usedAnchor);
                    var material = new Material(foregroundMaterial);
                    material.renderQueue += 1;
                    InitializeRenderer(bar, material, colorId, profile.MemoryUsedColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
                {
                    peakAnchor = CreateAnchor("PeakAnchor", limitBar.transform);
                    var bar = CreateQuad("PeakBar", peakAnchor);
                    InitializeRenderer(bar, foregroundMaterial, colorId, profile.MemoryPeakColor);
                    bar.transform.localScale = Vector3.one;
                    bar.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                }
            }

            window.SetActive(isVisible);
        }

        private void BuildFrameRateStrings()
        {
            cpuFrameRateStrings = new string[MaxTargetFrameRate + 1];
            gpuFrameRateStrings = new string[MaxTargetFrameRate + 1];
            var displayedDecimalFormat = $"{{0:F{profile.DisplayedDecimalDigits}}}";

            var stringBuilder = new StringBuilder(32);
            var millisecondStringBuilder = new StringBuilder(16);

            for (int i = 0; i < cpuFrameRateStrings.Length; ++i)
            {
                float milliseconds = (i == 0) ? 0.0f : (1.0f / i) * 1000.0f;
                millisecondStringBuilder.AppendFormat(displayedDecimalFormat, milliseconds.ToString(CultureInfo.InvariantCulture));
                stringBuilder.AppendFormat("CPU: {0} fps ({1} ms)", i.ToString(), millisecondStringBuilder);
                cpuFrameRateStrings[i] = stringBuilder.ToString();
                stringBuilder.Length = 0;
                stringBuilder.AppendFormat("GPU: {0} fps ({1} ms)", i.ToString(), millisecondStringBuilder);
                gpuFrameRateStrings[i] = stringBuilder.ToString();
                millisecondStringBuilder.Length = 0;
                stringBuilder.Length = 0;
            }
        }

        private static Transform CreateAnchor(string name, Transform parent)
        {
            var anchor = new GameObject(name).transform;
            anchor.parent = parent;
            anchor.localScale = Vector3.one;
            anchor.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);

            return anchor;
        }

        private static GameObject CreateQuad(string name, Transform parent)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.name = name;
            quad.transform.parent = parent;

            return quad;
        }

        private static TextMesh CreateText(string name, Vector3 position, Transform parent, TextAnchor anchor, Material material, Color color, string text)
        {
            var obj = new GameObject(name);
            obj.transform.localScale = Vector3.one * 0.0016f;
            obj.transform.parent = parent;
            obj.transform.localPosition = position;

            var textMesh = obj.AddComponent<TextMesh>();
            textMesh.fontSize = 48;
            textMesh.anchor = anchor;
            textMesh.color = color;
            textMesh.text = text;
            textMesh.richText = false;

            var renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            OptimizeRenderer(renderer);

            return textMesh;
        }

        private static void InitializeRenderer(GameObject obj, Material material, int colorId, Color color)
        {
            var renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            var propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorId, color);
            renderer.SetPropertyBlock(propertyBlock);

            OptimizeRenderer(renderer);

            return;
        }

        private static void OptimizeRenderer(Renderer renderer)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.allowOcclusionWhenDynamic = false;
        }
    }
}