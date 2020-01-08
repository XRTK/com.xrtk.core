// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Profiling;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Diagnostics data provider for memory diagnostics. E.g. provides information about used application memory.
    /// </summary>
    public class MixedRealityMemoryDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile"></param>
        public MixedRealityMemoryDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        private ulong lastMemoryUsage;
        private ulong peakMemoryUsage;
        private ulong lastMemoryLimit;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            var systemMemorySize = (ulong)Profiler.GetTotalReservedMemoryLong();

            if (lastMemoryUsage != systemMemorySize)
            {
                if (systemMemorySize > lastMemoryLimit)
                {
                    MixedRealityToolkit.DiagnosticsSystem.RaiseMemoryLimitChanged(new MemoryLimit(systemMemorySize));
                    lastMemoryLimit = systemMemorySize;
                }
            }

            var currentMemoryUsage = (ulong)Profiler.GetTotalAllocatedMemoryLong();

            if (currentMemoryUsage != lastMemoryUsage)
            {
                MixedRealityToolkit.DiagnosticsSystem.RaiseMemoryUsageChanged(new MemoryUsage(currentMemoryUsage));
                lastMemoryUsage = currentMemoryUsage;
            }

            if (lastMemoryUsage > peakMemoryUsage)
            {
                MixedRealityToolkit.DiagnosticsSystem.RaiseMemoryPeakChanged(new MemoryPeak(peakMemoryUsage));
                peakMemoryUsage = lastMemoryUsage;
            }
        }

        #endregion IMixedRealityService Implementation
    }
}