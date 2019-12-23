// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.EventDatum.DiagnosticsSystem
{
    /// <summary>
    /// The event data associated with memory events.
    /// </summary>
    public class MemoryEventData : BaseDiagnosticsEventData
    {
        public MemoryPeak MemoryPeak { get; private set; } = default;

        public MemoryLimit CurrentMemoryLimit { get; private set; } = default;

        public MemoryUsage CurrentMemoryUsage { get; private set; } = default;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MemoryEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(MemoryLimit currentMemoryLimit)
        {
            BaseInitialize();
            CurrentMemoryLimit = currentMemoryLimit;
        }

        public void Initialize(MemoryUsage currentMemoryUsage)
        {
            BaseInitialize();
            CurrentMemoryUsage = currentMemoryUsage;
        }

        public void Initialize(MemoryPeak memoryPeak)
        {
            BaseInitialize();
            MemoryPeak = memoryPeak;
        }
    }
}