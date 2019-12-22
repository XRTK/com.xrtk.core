// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityMemoryDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// The current memory usage has changed.
        /// </summary>
        /// <param name="oldMemoryUsage">The old memory usage in bytes.</param>
        /// <param name="newMemoryUsage">New memory usage in bytes.</param>
        void OnMemoryUsageChanged(ulong oldMemoryUsage, ulong newMemoryUsage);

        /// <summary>
        /// The available memory limit has changed.
        /// </summary>
        /// <param name="oldMemoryLimit">The previous memory limit in bytes.</param>
        /// <param name="newMemoryLimit">The new memory limit in bytes.</param>
        void OnMemoryLimitChanged(ulong oldMemoryLimit, ulong newMemoryLimit);

        /// <summary>
        /// The peak memory used has changed.
        /// </summary>
        /// <param name="oldMemoryPeak">The old memory peak in bytes.</param>
        /// <param name="newMemoryPeak">The new memory peak in bytes.</param>
        void OnMemoryPeakChanged(ulong oldMemoryPeak, ulong newMemoryPeak);
    }
}