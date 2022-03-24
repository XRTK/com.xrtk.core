// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Abstract base implementation for diagnostics data providers. Provides needed implementations to register and unregister
    /// diagnostics handlers.
    /// </summary>
    public abstract class BaseMixedRealityDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        /// <inheritdoc />
        protected BaseMixedRealityDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile, IMixedRealityDiagnosticsSystem parentService)
            : base(name, priority, profile, parentService)
        {
            DiagnosticsSystem = parentService;
        }

        protected IMixedRealityDiagnosticsSystem DiagnosticsSystem;
    }
}