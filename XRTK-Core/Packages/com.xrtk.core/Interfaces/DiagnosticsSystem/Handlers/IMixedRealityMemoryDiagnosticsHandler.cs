// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.EventDatum.DiagnosticsSystem;

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityMemoryDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// The current memory usage has changed.
        /// </summary>
        void OnMemoryUsageChanged(MemoryEventData eventData);

        /// <summary>
        /// The available memory limit has changed.
        /// </summary>
        void OnMemoryLimitChanged(MemoryEventData eventData);

        /// <summary>
        /// The peak memory used has changed.
        /// </summary>
        void OnMemoryPeakChanged(MemoryEventData eventData);
    }
}