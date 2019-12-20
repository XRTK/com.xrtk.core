// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;

namespace XRTK.Services.DiagnosticsSystem
{
    public class MixedRealityDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        private MixedRealityDiagnosticsDataProviderProfile profile;
        private DefaultMixedRealityDiagnosticsVisualizer profilerDiagnosticsVisualizer = null;
        private GameObject diagnosticVisualizationParent = null;

        private bool showProfiler;
        /// <inheritdoc />
        public bool ShowProfiler
        {
            get => showProfiler;
            set
            {
                if (value != showProfiler)
                {
                    showProfiler = value;
                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.IsVisible = value;
                    }
                }
            }
        }

        private float frameSampleRate = 0.1f;
        /// <inheritdoc />
        public float FrameSampleRate
        {
            get => frameSampleRate;
            set
            {
                if (!Mathf.Approximately(frameSampleRate, value))
                {
                    frameSampleRate = value;

                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.FrameSampleRate = frameSampleRate;
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool ShowConsole { get; set; }

        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        /// <summary>
        /// What part of the view port to anchor the window to.
        /// </summary>
        public TextAnchor WindowAnchor
        {
            get => windowAnchor;

            set
            {
                if (value != windowAnchor)
                {
                    windowAnchor = value;

                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.WindowAnchor = windowAnchor;
                    }
                }
            }
        }

        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        /// <summary>
        /// The offset from the view port center applied based on the window anchor selection.
        /// </summary>
        public Vector2 WindowOffset
        {
            get => windowOffset;

            set
            {
                if (value != windowOffset)
                {
                    windowOffset = value;

                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.WindowOffset = windowOffset;
                    }
                }
            }
        }

        private float windowScale = 1.0f;

        /// <summary>
        /// Use to scale the window size up or down, can simulate a zooming effect.
        /// </summary>
        public float WindowScale
        {
            get => windowScale;

            set
            {
                if (!value.Equals(windowScale))
                {
                    windowScale = value;

                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.WindowScale = windowScale;
                    }
                }
            }
        }

        private float windowFollowSpeed = 5.0f;

        /// <summary>
        /// How quickly to interpolate the window towards its target position and rotation.
        /// </summary>
        public float WindowFollowSpeed
        {
            get => windowFollowSpeed;

            set
            {
                if (!value.Equals(windowFollowSpeed))
                {
                    windowFollowSpeed = value;

                    if (profilerDiagnosticsVisualizer != null)
                    {
                        profilerDiagnosticsVisualizer.WindowFollowSpeed = windowFollowSpeed;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        public MixedRealityDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            // Apply profile settings
            ShowProfiler = profile.ShowProfiler;
            FrameSampleRate = profile.FrameSampleRate;
            WindowAnchor = profile.WindowAnchor;
            WindowOffset = profile.WindowOffset;
            WindowScale = profile.WindowScale;
            WindowFollowSpeed = profile.WindowFollowSpeed;

            CreateDiagnosticsGameObject();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (diagnosticVisualizationParent != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(diagnosticVisualizationParent);
                }
                else
                {
                    diagnosticVisualizationParent.transform.DetachChildren();
                    Object.Destroy(diagnosticVisualizationParent);
                }

                diagnosticVisualizationParent = null;
            }
        }

        /// <summary>
        /// Creates the diagnostic visualizations and parents them so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        private void CreateDiagnosticsGameObject()
        {
            diagnosticVisualizationParent = new GameObject("Diagnostics");
            diagnosticVisualizationParent.transform.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;

            // visual profiler settings
            profilerDiagnosticsVisualizer = diagnosticVisualizationParent.AddComponent<DefaultMixedRealityDiagnosticsVisualizer>();
            profilerDiagnosticsVisualizer.profile = profile;
            profilerDiagnosticsVisualizer.WindowParent = diagnosticVisualizationParent.transform;
            profilerDiagnosticsVisualizer.IsVisible = ShowProfiler;
            profilerDiagnosticsVisualizer.FrameSampleRate = FrameSampleRate;
            profilerDiagnosticsVisualizer.WindowAnchor = WindowAnchor;
            profilerDiagnosticsVisualizer.WindowOffset = WindowOffset;
            profilerDiagnosticsVisualizer.WindowScale = WindowScale;
            profilerDiagnosticsVisualizer.WindowFollowSpeed = WindowFollowSpeed;
        }
    }
}