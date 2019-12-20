// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Diagnostics System Profile", fileName = "MixedRealityDiagnosticsSystemProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsSystemProfile : BaseMixedRealityProfile
    {
        #region Global Diagnostics Settings

        [SerializeField]
        [Tooltip("Display all enabled diagnostics")]
        private bool showDiagnostics = true;

        /// <summary>
        /// Show or hide diagnostic visualizations in general.
        /// </summary>
        public bool ShowDiagnostics => showDiagnostics;

        [SerializeField]
        [Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.LowerCenter;

        /// <summary>
        /// What part of the view port to anchor the window to.
        /// </summary>
        public TextAnchor WindowAnchor => windowAnchor;

        [SerializeField]
        [Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

        /// <summary>
        /// The offset from the view port center applied based on the window anchor selection.
        /// </summary>
        public Vector2 WindowOffset => windowOffset;

        [SerializeField]
        [Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 1.0f;

        /// <summary>
        /// Use to scale the window size up or down, can simulate a zooming effect.
        /// </summary>
        public float WindowScale => windowScale;

        [SerializeField]
        [Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 5.0f;

        /// <summary>
        /// How quickly to interpolate the window towards its target position and rotation.
        /// </summary>
        public float WindowFollowSpeed => windowFollowSpeed;

        [SerializeField]
        [Tooltip("The color of the window backplate.")]
        private Color windowBackgroundColor = new Color(80 / 256.0f, 80 / 256.0f, 80 / 256.0f, 1.0f);

        /// <summary>
        /// The background color of the diagnostics window.
        /// </summary>
        public Color WindowBackgroundColor => windowBackgroundColor;

        #endregion

        #region Profiler Diagnostics Settings

        [SerializeField]
        [Tooltip("Display profiler")]
        private bool showProfiler = true;

        /// <summary>
        /// Show or hide the profiler UI.
        /// </summary>
        public bool ShowProfiler => showProfiler;

        [SerializeField]
        [FormerlySerializedAs("frameRateDuration")]
        [Tooltip("The amount of time, in seconds, to collect frames for frame rate calculation.")]
        [Range(0, 5)]
        private float frameSampleRate = 0.1f;

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        public float FrameSampleRate => frameSampleRate;

        [Range(0, 3)]
        [SerializeField]
        [Tooltip("How many decimal places to display on numeric strings.")]
        private int displayedDecimalDigits = 1;

        /// <summary>
        /// How many decimal places to display on numeric strings.
        /// </summary>
        public int DisplayedDecimalDigits => displayedDecimalDigits;

        [SerializeField]
        [Tooltip("The color to display on frames which meet or exceed the target frame rate.")]
        private Color targetFrameRateColor = new Color(127 / 256.0f, 186 / 256.0f, 0 / 256.0f, 1.0f);

        /// <summary>
        /// The color to display on frames which meet or exceed the target frame rate.
        /// </summary>
        public Color TargetFrameRateColor => targetFrameRateColor;

        [SerializeField]
        [Tooltip("The color to display on frames which fall below the target frame rate.")]
        private Color missedFrameRateColor = new Color(242 / 256.0f, 80 / 256.0f, 34 / 256.0f, 1.0f);

        /// <summary>
        /// The color to display on frames which fall below the target frame rate.
        /// </summary>
        public Color MissedFrameRateColor => missedFrameRateColor;

        [SerializeField]
        [Tooltip("The color to display for current memory usage values.")]
        private Color memoryUsedColor = new Color(0 / 256.0f, 164 / 256.0f, 239 / 256.0f, 1.0f);

        /// <summary>
        /// The color to display for current memory usage values.
        /// </summary>
        public Color MemoryUsedColor => memoryUsedColor;

        [SerializeField]
        [Tooltip("The color to display for peak (aka max) memory usage values.")]
        private Color memoryPeakColor = new Color(255 / 256.0f, 185 / 256.0f, 0 / 256.0f, 1.0f);

        /// <summary>
        /// The color to display for peak (aka max) memory usage values.
        /// </summary>
        public Color MemoryPeakColor => memoryPeakColor;

        [SerializeField]
        [Tooltip("The color to display for the platforms memory usage limit.")]
        private Color memoryLimitColor = new Color(150 / 256.0f, 150 / 256.0f, 150 / 256.0f, 1.0f);

        /// <summary>
        /// The color to display for the platforms memory usage limit.
        /// </summary>
        public Color MemoryLimitColor => memoryLimitColor;

        #endregion

        #region Console Diagnostics Settings

        #endregion
    }
}