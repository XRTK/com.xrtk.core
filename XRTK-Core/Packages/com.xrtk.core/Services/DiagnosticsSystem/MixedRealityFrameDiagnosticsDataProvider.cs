// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;
using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Diagnostics data provider for frame diagnostics. It provides frame rate information and missed frames
    /// information to identify performance issues.
    /// </summary>
    public class MixedRealityFrameDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public MixedRealityFrameDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        private const int MAX_FRAME_TIMINGS = 128;
        private const float FALLBACK_REFRESH_RATE = 60.0f;
        private const int MISSED_FRAMES_RANGE = 30;

        private readonly FrameTiming[] frameTimings = new FrameTiming[MAX_FRAME_TIMINGS];
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly bool[] missedFrames = new bool[MISSED_FRAMES_RANGE];

        private uint frameTimingsCount;
        private int framesPassedSinceLastCalculation;

        private float frameSampleRate = 0.1f;

        /// <summary>
        /// The rate at which frames are sampled for FPS calculation.
        /// </summary>
        private float FrameSampleRate
        {
            get => frameSampleRate;
            set
            {
                if (!Mathf.Approximately(frameSampleRate, value))
                {
                    frameSampleRate = value;
                }
            }
        }

        /// <summary>
        /// The last computed GPU frame rate.
        /// </summary>
        private int GPUFrameRate { get; set; } = 0;

        /// <summary>
        /// The last computed CPU frame rate.
        /// </summary>
        private int CPUFrameRate { get; set; } = 0;

        /// <summary>
        /// Computed property returns the target refresh rate of the device,
        /// which is used to identify missed frames.
        /// </summary>
        private int DeviceTargetRefreshRate
        {
            get
            {
                // If the current XR SDK does not report refresh rate information, assume fallback value.
                var refreshRate = UnityEngine.XR.XRDevice.refreshRate;
                return (int)((int)refreshRate == 0 ? FALLBACK_REFRESH_RATE : refreshRate);
            }
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (!Application.isPlaying) { return; }

            stopwatch.Restart();
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            if (!Application.isPlaying) { return; }

            // Capture frame timings every frame and read from it depending on the frameSampleRate.
            FrameTimingManager.CaptureFrameTimings();

            // Increase internal frame count since last FPS calculation.
            framesPassedSinceLastCalculation++;

            // Do we have enough sample data for a new FPS calculation?
            var elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;

            if (elapsedSeconds >= FrameSampleRate)
            {
                var newCPUFrameRate = (int)(1.0f / (elapsedSeconds / framesPassedSinceLastCalculation));
                var newGPUFrameRate = 0;

                // Many platforms do not yet support the FrameTimingManager.
                // When timing data is returned from the FrameTimingManager we will use
                // its timing data, else we will depend on the stopwatch.
                var frameCount = (uint)Mathf.Min(framesPassedSinceLastCalculation, MAX_FRAME_TIMINGS);
                frameTimingsCount = FrameTimingManager.GetLatestTimings(frameCount, frameTimings);

                if (frameTimingsCount != 0)
                {
                    AverageFrameTiming(out var cpuFrameTime, out var gpuFrameTime);
                    newCPUFrameRate = (int)(1.0f / (cpuFrameTime / framesPassedSinceLastCalculation));
                    newGPUFrameRate = (int)(1.0f / (gpuFrameTime / framesPassedSinceLastCalculation));
                }

                if (CPUFrameRate != newCPUFrameRate)
                {
                    MixedRealityToolkit.DiagnosticsSystem.RaiseFrameRateChanged(newCPUFrameRate, false);
                    CPUFrameRate = newCPUFrameRate;
                }

                if (newGPUFrameRate != 0 && GPUFrameRate != newGPUFrameRate)
                {
                    MixedRealityToolkit.DiagnosticsSystem.RaiseFrameRateChanged(newGPUFrameRate, true);
                    GPUFrameRate = newGPUFrameRate;
                }

                // Update missed frames.
                for (int i = MISSED_FRAMES_RANGE - 1; i > 0; --i)
                {
                    missedFrames[i] = missedFrames[i - 1];
                }

                // Ideally we would query a device specific API (like the HolographicFramePresentationReport) to detect missed frames.
                // But, many of these APIs are inaccessible in Unity. Currently missed frames are assumed when the average cpuFrameRate 
                // is under the target frame rate.
                missedFrames[0] = newCPUFrameRate < DeviceTargetRefreshRate - 1;
                MixedRealityToolkit.DiagnosticsSystem.RaiseMissedFramesChanged(missedFrames);

                // Reset timers.
                framesPassedSinceLastCalculation = 0;

                stopwatch.Restart();
            }
        }

        #endregion IMixedRealityService Implementation

        private void AverageFrameTiming(out float cpuFrameTime, out float gpuFrameTime)
        {
            double cpuTime = 0.0f;
            double gpuTime = 0.0f;

            for (int i = 0; i < frameTimingsCount; ++i)
            {
                cpuTime += frameTimings[i].cpuFrameTime;
                gpuTime += frameTimings[i].gpuFrameTime;
            }

            cpuTime /= frameTimingsCount;
            gpuTime /= frameTimingsCount;

            cpuFrameTime = (float)(cpuTime * 0.001);
            gpuFrameTime = (float)(gpuTime * 0.001);
        }
    }
}