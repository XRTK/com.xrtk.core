// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services.DiagnosticsSystem;

namespace XRTK.Interfaces.Diagnostics
{
    public interface IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// Updates the visuailzed diagnostics.
        /// </summary>
        /// <param name="data">The new diagnostics data.</param>
        void UpdateDiagnostics(DiagnosticsData data);
    }
}