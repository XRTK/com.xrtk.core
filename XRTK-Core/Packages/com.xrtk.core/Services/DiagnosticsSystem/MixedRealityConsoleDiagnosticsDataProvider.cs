// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Console diagnostics data providers mirrors the Unity console and digests logs so the
    /// diagnostics system can work with it.
    /// </summary>
    public class MixedRealityConsoleDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider<IMixedRealityConsoleDiagnosticsHandler>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public MixedRealityConsoleDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile)
            : base(name, priority, profile) { }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            Application.logMessageReceived += Application_logMessageReceived;
        }

        /// <inheritdoc />
        public override void Disable()
        {
            Application.logMessageReceived -= Application_logMessageReceived;
            base.Disable();
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            for (int i = 0; i < Handlers.Count; i++)
            {
                Handlers[i].OnLogReceived(condition, type);
            }
        }
    }
}