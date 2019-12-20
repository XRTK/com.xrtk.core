// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default implementation of the <see cref="IMixedRealityDiagnosticsSystem"/>
    /// </summary>
    public class MixedRealityDiagnosticsSystem : BaseSystem, IMixedRealityDiagnosticsSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">Diagnostics service configuration profile.</param>
        public MixedRealityDiagnosticsSystem(MixedRealityDiagnosticsSystemProfile profile)
            : base(profile)
        {
            this.profile = profile;
        }

        private readonly MixedRealityDiagnosticsSystemProfile profile;

        #region IMixedRealityService

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!Application.isPlaying) { return; }

            // Apply profile settings
            ShowDiagnostics = profile.ShowDiagnostics;
            ShowProfiler = profile.ShowProfiler;
            FrameSampleRate = profile.FrameSampleRate;
            WindowAnchor = profile.WindowAnchor;
            WindowOffset = profile.WindowOffset;
            WindowScale = profile.WindowScale;
            WindowFollowSpeed = profile.WindowFollowSpeed;

            CreateVisualizations();
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

        #endregion IMixedRealityService Implementation

        #region IMixedRealityDiagnosticsSystem

        private bool showDiagnostics;

        /// <inheritdoc />
        public bool ShowDiagnostics
        {
            get => showDiagnostics;
            set
            {
                if (value != showDiagnostics)
                {
                    showDiagnostics = value;

                    if (diagnosticVisualizationParent != null)
                    {
                        diagnosticVisualizationParent.SetActive(value);
                    }
                }
            }
        }

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
                    if (visualProfiler != null)
                    {
                        visualProfiler.IsVisible = value;
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

                    if (visualProfiler != null)
                    {
                        visualProfiler.FrameSampleRate = frameSampleRate;
                    }
                }
            }
        }

        #endregion IMixedRealityDiagnosticsSystem Implementation

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

                    if (visualProfiler != null)
                    {
                        visualProfiler.WindowAnchor = windowAnchor;
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

                    if (visualProfiler != null)
                    {
                        visualProfiler.WindowOffset = windowOffset;
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

                    if (visualProfiler != null)
                    {
                        visualProfiler.WindowScale = windowScale;
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

                    if (visualProfiler != null)
                    {
                        visualProfiler.WindowFollowSpeed = windowFollowSpeed;
                    }
                }
            }
        }

        /// <summary>
        /// The parent object under which all visualization game objects will be placed.
        /// </summary>
        private GameObject diagnosticVisualizationParent = null;

        private MixedRealityProfilerDiagnosticsVisualizer visualProfiler = null;

        /// <summary>
        /// Creates the diagnostic visualizations and parents them so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        private void CreateVisualizations()
        {
            diagnosticVisualizationParent = new GameObject("Diagnostics");
            diagnosticVisualizationParent.transform.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;
            diagnosticVisualizationParent.SetActive(ShowDiagnostics);

            // visual profiler settings
            visualProfiler = diagnosticVisualizationParent.AddComponent<MixedRealityProfilerDiagnosticsVisualizer>();
            visualProfiler.WindowParent = diagnosticVisualizationParent.transform;
            visualProfiler.IsVisible = ShowProfiler;
            visualProfiler.FrameSampleRate = FrameSampleRate;
            visualProfiler.WindowAnchor = WindowAnchor;
            visualProfiler.WindowOffset = WindowOffset;
            visualProfiler.WindowScale = WindowScale;
            visualProfiler.WindowFollowSpeed = WindowFollowSpeed;
        }
    }
}
