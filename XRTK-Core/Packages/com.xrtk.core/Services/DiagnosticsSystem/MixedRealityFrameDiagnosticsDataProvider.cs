// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Diagnostics data provider for frame diagnostics. It provides frame rate information and missed frames
    /// information to identify performance issues.
    /// </summary>
    public class MixedRealityFrameDiagnosticsDataProvider : BaseMixedRealityDiagnosticsDataProvider<IMixedRealityFrameDiagnosticsHandler>
    {
        private const int maxFrameTimings = 128;
        private const float fallbackRefreshRate = 60.0f;
        private const int missedFramesRange = 30;
        private readonly FrameTiming[] frameTimings = new FrameTiming[maxFrameTimings];
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly bool[] missedFrames = new bool[missedFramesRange];
        private int framesPassedSinceLastCalculation;

        private float frameSampleRate = 0.1f;
        /// <summary>
        /// The rate at which frames are sampled for FPS calculation.
        /// </summary>
        public float FrameSampleRate
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
        private float DeviceTargetRefreshRate
        {
            get
            {
                // If the current XR SDK does not report refresh rate information, assume fallback value.
                float refreshRate = UnityEngine.XR.XRDevice.refreshRate;
                return ((int)refreshRate == 0) ? fallbackRefreshRate : refreshRate;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        /// <param name="profile">The provider configuration profile assigned.</param>
        public MixedRealityFrameDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile)
            : base(name, priority, profile) { }

        public override void Enable()
        {
            base.Enable();

            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            // Capture frame timings every frame and read from it depending on the frameSampleRate.
            FrameTimingManager.CaptureFrameTimings();

            // Increase internal frame count since last FPS calculation.
            ++framesPassedSinceLastCalculation;

            // Do we have enough sample data for a new FPS calculation?
            float elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedSeconds >= frameSampleRate)
            {
                int newCPUFrameRate = (int)(1.0f / (elapsedSeconds / framesPassedSinceLastCalculation));
                int newGPUFrameRate = 0;

                // Many platforms do not yet support the FrameTimingManager. When timing data is returned from the FrameTimingManager we will use
                // its timing data, else we will depend on the stopwatch.
                uint frameTimingsCount = FrameTimingManager.GetLatestTimings((uint)Mathf.Min(framesPassedSinceLastCalculation, maxFrameTimings), frameTimings);
                if (frameTimingsCount != 0)
                {
                    AverageFrameTiming(frameTimings, frameTimingsCount, out var cpuFrameTime, out var gpuFrameTime);
                    newCPUFrameRate = (int)(1.0f / (cpuFrameTime / framesPassedSinceLastCalculation));
                    newGPUFrameRate = (int)(1.0f / (gpuFrameTime / framesPassedSinceLastCalculation));
                }

                if (CPUFrameRate != newCPUFrameRate)
                {
                    for (int i = 0; i < Handlers.Count; i++)
                    {
                        Handlers[i].OnFrameRateChanged(CPUFrameRate, newCPUFrameRate, false);
                    }

                    CPUFrameRate = newCPUFrameRate;
                }

                if (newGPUFrameRate != 0 && GPUFrameRate != newGPUFrameRate)
                {
                    for (int i = 0; i < Handlers.Count; i++)
                    {
                        Handlers[i].OnFrameRateChanged(GPUFrameRate, newGPUFrameRate, true);
                    }

                    GPUFrameRate = newGPUFrameRate;
                }

                // Update missed frames.
                for (int i = missedFramesRange - 1; i > 0; --i)
                {
                    missedFrames[i] = missedFrames[i - 1];
                }

                // Ideally we would query a device specific API (like the HolographicFramePresentationReport) to detect missed frames.
                // But, many of these APIs are inaccessible in Unity. Currently missed frames are assumed when the average cpuFrameRate 
                // is under the target frame rate.
                missedFrames[0] = newCPUFrameRate < ((int)(DeviceTargetRefreshRate) - 1);
                for (int i = 0; i < Handlers.Count; i++)
                {
                    Handlers[i].OnMissedFramesChanged(missedFrames);
                }

                // Reset timers.
                framesPassedSinceLastCalculation = 0;
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        private void AverageFrameTiming(FrameTiming[] frameTimings, uint frameTimingsCount, out float cpuFrameTime, out float gpuFrameTime)
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
