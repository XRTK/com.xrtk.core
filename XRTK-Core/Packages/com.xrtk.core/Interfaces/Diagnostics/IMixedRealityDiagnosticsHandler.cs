// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Diagnostics;

namespace XRTK.Interfaces.Diagnostics
{
    /// <summary>
    /// Interface to implement to receive data from the <see cref="IMixedRealityDiagnosticsSystem"/>.
    /// </summary>
    public interface IMixedRealityDiagnosticsHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when the diagnostics system sends an update to the diagnostic data.
        /// </summary>
        /// <param name="eventData"></param>
        void OnDiagnosticSettingsChanged(DiagnosticsEventData eventData);
    }
}
