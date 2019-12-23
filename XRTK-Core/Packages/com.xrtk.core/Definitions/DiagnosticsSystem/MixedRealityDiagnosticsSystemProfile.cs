// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.DiagnosticsSystem
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Diagnostics System Profile", fileName = "MixedRealityDiagnosticsSystemProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsSystemProfile : BaseMixedRealityProfile
    {
        [Prefab]
        [SerializeField]
        [Tooltip("The prefab instantiated to visualize diagnostics data.")]
        private GameObject diagnosticsWindowPrefab = null;

        /// <summary>
        /// The prefab instantiated to visualize diagnostics data.
        /// </summary>
        public GameObject DiagnosticsWindowPrefab
        {
            get => diagnosticsWindowPrefab;
            private set => diagnosticsWindowPrefab = value;
        }

        [SerializeField]
        [Tooltip("Should the diagnostics window be opened on application start?")]
        private AutoStartBehavior showDiagnosticsWindowOnStart = AutoStartBehavior.ManualStart;

        /// <summary>
        /// Should the diagnostics window be opened on application start?
        /// </summary>
        public AutoStartBehavior ShowDiagnosticsWindowOnStart => showDiagnosticsWindowOnStart;

        [SerializeField]
        private DiagnosticsDataProviderConfiguration[] registeredDiagnosticsDataProviders = new DiagnosticsDataProviderConfiguration[0];

        /// <summary>
        /// The currently registered diagnostics data providers for the diagnostics system.
        /// </summary>
        public DiagnosticsDataProviderConfiguration[] RegisteredDiagnosticsDataProviders => registeredDiagnosticsDataProviders;
    }
}