// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;
using XRTK.Definitions;

#if WINDOWS_UWP
using Windows.System;
#else
using UnityEngine.Profiling;
#endif

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Diagnostics data provider for memory diagnostics. E.g. provides information about used application memory.
    /// </summary>
    public class MixedRealityMemoryDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider<IMixedRealityMemoryDiagnosticsHandler>
    {
        private ulong lastMemoryUsage;
        private ulong peakMemoryUsage;
        private ulong lastMemoryLimit;

        /// <summary>
        /// Computed property reads the current memory usage of the running
        /// application.
        /// </summary>
        private static ulong AppMemoryUsage
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsage;
#else
                return (ulong)Profiler.GetTotalAllocatedMemoryLong();
#endif
            }
        }

        /// <summary>
        /// Computed property reads the current memory limit avaiable to the application.
        /// </summary>
        private static ulong AppMemoryUsageLimit
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsageLimit;
#else
                return DiagnosticsUtils.ConvertMegabytesToBytes(SystemInfo.systemMemorySize);
#endif
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public MixedRealityMemoryDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile)
            : base(name, priority, profile) { }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            ulong currentMemoryLimit = AppMemoryUsageLimit;
            if (currentMemoryLimit != lastMemoryLimit)
            {
                for (int i = 0; i < Handlers.Count; i++)
                {
                    Handlers[i].OnMemoryLimitChanged(lastMemoryLimit, currentMemoryLimit);
                }

                lastMemoryLimit = currentMemoryLimit;
            }

            ulong currentMemoryUsage = AppMemoryUsage;
            if (currentMemoryUsage != lastMemoryUsage)
            {
                for (int i = 0; i < Handlers.Count; i++)
                {
                    Handlers[i].OnMemoryUsageChanged(lastMemoryUsage, currentMemoryUsage);
                }

                lastMemoryUsage = currentMemoryUsage;
            }

            if (lastMemoryUsage > peakMemoryUsage)
            {
                for (int i = 0; i < Handlers.Count; i++)
                {
                    Handlers[i].OnMemoryPeakChanged(peakMemoryUsage, lastMemoryUsage);
                }

                peakMemoryUsage = lastMemoryUsage;
            }
        }
    }
}