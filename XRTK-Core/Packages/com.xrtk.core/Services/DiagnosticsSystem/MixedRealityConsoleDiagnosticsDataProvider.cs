// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;

namespace XRTK.Services.DiagnosticsSystem
{
    public class MixedRealityConsoleDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        private readonly MixedRealityDiagnosticsDataProviderProfile profile;
        private readonly List<IMixedRealityDiagnosticsHandler> handlers = new List<IMixedRealityDiagnosticsHandler>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public MixedRealityConsoleDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority)
        {
            this.profile = profile;
        }

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

        /// <inheritdoc />
        public void Register(IMixedRealityDiagnosticsHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityDiagnosticsHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].OnLogReceived(condition, type);
            }
        }
    }
}