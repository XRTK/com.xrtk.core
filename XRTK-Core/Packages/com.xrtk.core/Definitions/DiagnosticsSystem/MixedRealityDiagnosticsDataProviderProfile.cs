// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.DiagnosticsSystem
{
    /// <summary>
    /// Configuration profile settings for setting up a diagnostics data provider.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Diagnostics System/Diagnostics Data Provider Profile", fileName = "MixedRealityDiagnosticsDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsDataProviderProfile : BaseMixedRealityProfile
    {
        #region Profiler Diagnostics Settings

        [SerializeField]
        [Tooltip("The amount of time, in seconds, to collect frames for frame rate calculation.")]
        [Range(0, 5)]
        private float frameSampleRate = 0.1f;

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        public float FrameSampleRate => frameSampleRate;

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

        #endregion
    }
}