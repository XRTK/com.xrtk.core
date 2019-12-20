// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.DiagnosticsSystem
{
    public class DiagnosticsData
    {
        /// <summary>
        /// Enable / disable the profiler diagnostics visualization.
        /// </summary>
        public bool ShowProfiler { get; set; }

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        public float FrameSampleRate { get; set; }

        /// <summary>
        /// Enable / disable console diagnostics visualization.
        /// </summary>
        public bool ShowConsole { get; set; }
    }
}