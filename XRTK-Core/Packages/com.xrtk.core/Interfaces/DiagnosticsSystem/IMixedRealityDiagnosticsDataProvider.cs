// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.DiagnosticsSystem.Handlers;

namespace XRTK.Interfaces.DiagnosticsSystem
{
    public interface IMixedRealityDiagnosticsDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Registers the diagnostics handler to receive updates.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        void Register(IMixedRealityDiagnosticsHandler handler);

        /// <summary>
        /// Unregisters the diagnostics handler from receiving updates.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        void Unregister(IMixedRealityDiagnosticsHandler handler);
    }
}