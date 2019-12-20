// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Interfaces;
using XRTK.Interfaces.Diagnostics;

namespace XRTK.Services.DiagnosticsSystem
{
    public abstract class BaseMixedRealityDiagnosticsHandler : MonoBehaviour, IMixedRealityDiagnosticsHandler
    {
        protected virtual void OnEnable()
        {
            List<IMixedRealityService> dataProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityDiagnosticsDataProvider>();
            for (int i = 0; i < dataProviders.Count; i++)
            {
                if (dataProviders[i] is IMixedRealityDiagnosticsDataProvider diagnosticsDataProvider)
                {
                    diagnosticsDataProvider.Register(this);
                }
            }
        }

        protected virtual void OnDisable()
        {
            List<IMixedRealityService> dataProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityDiagnosticsDataProvider>();
            for (int i = 0; i < dataProviders.Count; i++)
            {
                if (dataProviders[i] is IMixedRealityDiagnosticsDataProvider diagnosticsDataProvider)
                {
                    diagnosticsDataProvider.Unregister(this);
                }
            }
        }

        /// <inheritdoc />
        public abstract void UpdateDiagnostics(DiagnosticsData data);
    }
}