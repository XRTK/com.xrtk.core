// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.EventDatum.DiagnosticsSystem;

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityFrameDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// Raised when the <see cref="IMixedRealityDiagnosticsSystem"/> frame rate has changed.
        /// </summary>
        void OnFrameRateChanged(FrameEventData eventData);

        /// <summary>
        /// Missed frames diagnostics have changed.
        /// </summary>
        void OnMissedFramesChanged(MissedFrameEventData eventData);
    }
}