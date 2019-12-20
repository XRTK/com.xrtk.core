// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.Diagnostics
{
    public interface IMixedRealityDiagnosticsDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Enable / disable the profiler diagnostics visualization.
        /// </summary>
        bool ShowProfiler { get; set; }

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        float FrameSampleRate { get; }

        /// <summary>
        /// Enable / disable console diagnostics visualization.
        /// </summary>
        bool ShowConsole { get; set; }
    }
}