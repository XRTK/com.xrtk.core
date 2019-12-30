// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Console diagnostics data providers mirrors the Unity console and digests logs so the
    /// diagnostics system can work with it.
    /// </summary>
    public class MixedRealityConsoleDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public MixedRealityConsoleDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        #region IMixedRealityServce Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            Application.logMessageReceived += MixedRealityToolkit.DiagnosticsSystem.RaiseLogReceived;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (MixedRealityToolkit.DiagnosticsSystem != null)
            {
                Application.logMessageReceived -= MixedRealityToolkit.DiagnosticsSystem.RaiseLogReceived;
            }
        }

        #endregion IMixedRealityServce Implementation
    }
}