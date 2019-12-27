// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.EventDatum.DiagnosticsSystem
{
    /// <summary>
    /// The event data associated with memory events.
    /// </summary>
    public class MemoryEventData : BaseDiagnosticsEventData
    {
        /// <summary>
        /// The peak memory usage as reported from the <see cref="IMixedRealityDiagnosticsDataProvider"/>
        /// </summary>
        public MemoryPeak MemoryPeak { get; private set; } = default;

        /// <summary>
        /// The current memory limit as reported from the <see cref="IMixedRealityDiagnosticsDataProvider"/>
        /// </summary>
        public MemoryLimit CurrentMemoryLimit { get; private set; } = default;

        /// <summary>
        /// The current memory usage as reported from the <see cref="IMixedRealityDiagnosticsDataProvider"/>
        /// </summary>
        public MemoryUsage CurrentMemoryUsage { get; private set; } = default;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MemoryEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="currentMemoryLimit"></param>
        public void Initialize(MemoryLimit currentMemoryLimit)
        {
            BaseInitialize();
            CurrentMemoryLimit = currentMemoryLimit;
        }

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="currentMemoryUsage"></param>
        public void Initialize(MemoryUsage currentMemoryUsage)
        {
            BaseInitialize();
            CurrentMemoryUsage = currentMemoryUsage;
        }

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="memoryPeak"></param>
        public void Initialize(MemoryPeak memoryPeak)
        {
            BaseInitialize();
            MemoryPeak = memoryPeak;
        }
    }
}