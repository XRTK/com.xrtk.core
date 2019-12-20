// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Services.DiagnosticsSystem
{
    public class MixedRealityConsoleDiagnosticsVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Is the console currently visible.")]
        private bool isVisible = false;

        /// <summary>
        /// Is the console currently visible.
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set => isVisible = value;
        }
    }
}