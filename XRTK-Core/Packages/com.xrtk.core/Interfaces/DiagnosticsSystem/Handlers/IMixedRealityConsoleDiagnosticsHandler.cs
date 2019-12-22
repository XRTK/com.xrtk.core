// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityConsoleDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// A new log entry was received.
        /// </summary>
        /// <param name="message">Message text of the entry.</param>
        /// <param name="type">The type of the log entry.</param>
        void OnLogReceived(string message, LogType type);
    }
}