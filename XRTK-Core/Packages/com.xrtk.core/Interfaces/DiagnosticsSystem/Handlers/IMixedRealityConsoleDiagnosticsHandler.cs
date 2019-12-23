// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.EventDatum.DiagnosticsSystem;

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityConsoleDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// A new log entry was received.
        /// </summary>
        void OnLogReceived(ConsoleEventData eventData);
    }
}