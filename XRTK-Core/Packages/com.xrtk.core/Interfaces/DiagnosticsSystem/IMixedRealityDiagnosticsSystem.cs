// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Interfaces.DiagnosticsSystem
{
    /// <summary>
    /// The interface contract that defines the Diagnostics system in the Mixed Reality Toolkit
    /// </summary>
    public interface IMixedRealityDiagnosticsSystem : IMixedRealityService
    {
        /// <summary>
        /// Gets the root transform where any visualized diagnostics <see cref="GameObject"/>s should live.
        /// </summary>
        Transform DiagnosticsRoot { get; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> reference to the diagnostics window.
        /// </summary>
        GameObject DiagnosticsWindow { get; }

        /// <summary>
        /// Gets the application product name and build version.
        /// Used to identify the current application name and build.
        /// </summary>
        string ApplicationSignature { get; }

        /// <summary>
        /// Should the diagnostics window be displayed?
        /// </summary>
        bool IsWindowEnabled { get; set; }

        #region Console Events

        /// <summary>
        /// Raise the event that a log was received by the <see cref="IMixedRealityDiagnosticsSystem"/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        void RaiseLogReceived(string condition, string message, LogType type);

        #endregion Console Events

        #region Frame Events

        /// <summary>
        /// Raise the event that the frame rate has changed.
        /// </summary>
        /// <param name="frameRate"></param>
        /// <param name="isGPU"></param>
        void RaiseFrameRateChanged(int frameRate, bool isGPU);

        /// <summary>
        /// Raise the event that the missed frames have changed.
        /// </summary>
        /// <param name="missedFrames"></param>
        void RaiseMissedFramesChanged(bool[] missedFrames);

        #endregion Frame Events

        #region Memory Events

        /// <summary>
        /// Raise the event that the memory limit has changed.
        /// </summary>
        /// <param name="currentMemoryLimit"></param>
        void RaiseMemoryLimitChanged(MemoryLimit currentMemoryLimit);

        /// <summary>
        /// Raise the event that the memory usage has changed.
        /// </summary>
        /// <param name="currentMemoryUsage"></param>
        void RaiseMemoryUsageChanged(MemoryUsage currentMemoryUsage);

        /// <summary>
        /// Raise the event that the peak memory has changed.
        /// </summary>
        /// <param name="peakMemoryUsage"></param>
        void RaiseMemoryPeakChanged(MemoryPeak peakMemoryUsage);

        #endregion Memory Events
    }
}