// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;

namespace XRTK.Providers.Controllers.Simulation
{
    public sealed class SimulationTimeStampStopWatch
    {
        private readonly Stopwatch stopwatch;
        private DateTime stopwatchStartTime;

        /// <summary>
        /// Gets the current stopwatch datetime.
        /// </summary>
        public DateTime Current => stopwatchStartTime.Add(stopwatch.Elapsed);

        /// <summary>
        /// Gets the current stopwatch timestamp.
        /// </summary>
        public long TimeStamp => Current.Ticks;

        /// <summary>
        /// Creates a new timestamp stopwatch.
        /// </summary>
        public SimulationTimeStampStopWatch()
        {
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Resets the internal stopwatch and thus timestamps provided.
        /// </summary>
        public void Reset()
        {
            stopwatchStartTime = DateTime.UtcNow;
            stopwatch.Restart();
        }
    }
}