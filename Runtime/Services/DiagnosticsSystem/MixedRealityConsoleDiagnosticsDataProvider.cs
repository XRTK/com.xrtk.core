// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Console diagnostics data providers mirrors the Unity console and digests logs so the
    /// diagnostics system can work with it.
    /// </summary>
    [System.Runtime.InteropServices.Guid("06916F29-4640-475E-8BF6-313C6B831FCF")]
    public class MixedRealityConsoleDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider
    {
        /// <inheritdoc />
        public MixedRealityConsoleDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile, IMixedRealityDiagnosticsSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        #region IMixedRealityServce Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (DiagnosticsSystem != null)
            {
                Application.logMessageReceived += DiagnosticsSystem.RaiseLogReceived;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (DiagnosticsSystem != null)
            {
                Application.logMessageReceived -= DiagnosticsSystem.RaiseLogReceived;
            }
        }

        #endregion IMixedRealityServce Implementation
    }
}