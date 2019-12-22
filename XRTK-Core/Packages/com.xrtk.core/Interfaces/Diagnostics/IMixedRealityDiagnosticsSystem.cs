// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.Diagnostics
{
    /// <summary>
    /// The interface contract that defines the Diagnostics system in the Mixed Reality Toolkit
    /// </summary>
    public interface IMixedRealityDiagnosticsSystem : IMixedRealityService
    {
        /// <summary>
        /// Gets the diagnostics transform where any visualized diganostics game objects should live.
        /// </summary>
        Transform DiagnosticsTransform { get; }

        /// <summary>
        /// Gets the application product name and build version. May be used to identify
        /// the build the diagnostics belong to.
        /// </summary>
        string ApplicationSignature { get; }

        /// <summary>
        /// Should the diagnostics window be displayed?
        /// </summary>
        bool ShowWindow { get; set; }
    }
}