// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Diagnostics System Profile", fileName = "MixedRealityDiagnosticsSystemProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Enable or disable diagnostics globally.")]
        private bool enableDiagnostics = true;

        /// <summary>
        /// Enable / disable diagnostic globally.
        /// </summary>
        /// <remarks>
        /// When set to true, settings for individual diagnostics data providers are honored. When set to false,
        /// all diagnostics are disabled.
        /// </remarks>
        public bool EnableDiagnostics => enableDiagnostics;

        [SerializeField]
        private DiagnosticsDataProviderConfiguration[] registeredDiagnosticsDataProviders = new DiagnosticsDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered diagnostics data providers for the diagnostics system.
        /// </summary>
        public DiagnosticsDataProviderConfiguration[] RegisteredDiagnosticsDataProviders => registeredDiagnosticsDataProviders;
    }
}