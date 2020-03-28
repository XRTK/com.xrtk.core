// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Definitions.DiagnosticsSystem
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Diagnostics System Profile", fileName = "MixedRealityDiagnosticsSystemProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityDiagnosticsDataProvider>
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
    }
}