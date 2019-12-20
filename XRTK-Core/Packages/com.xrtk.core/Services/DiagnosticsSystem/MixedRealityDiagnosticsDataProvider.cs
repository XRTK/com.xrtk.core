// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;
using System.Collections.Generic;

#if WINDOWS_UWP
using Windows.System;
#else
using UnityEngine.Profiling;
#endif

namespace XRTK.Services.DiagnosticsSystem
{
    public class MixedRealityDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        private const int maxFrameTimings = 128;
        private const float fallbackRefreshRate = 60.0f;

        private MixedRealityDiagnosticsDataProviderProfile profile;
        private readonly List<IMixedRealityDiagnosticsHandler> handlers = new List<IMixedRealityDiagnosticsHandler>();

        private int frameCount;
        private readonly FrameTiming[] frameTimings = new FrameTiming[maxFrameTimings];
        private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private static float AppFrameRate
        {
            get
            {
                // If the current XR SDK does not report refresh rate information, assume fallback value.
                float refreshRate = UnityEngine.XR.XRDevice.refreshRate;
                return ((int)refreshRate == 0) ? fallbackRefreshRate : refreshRate;
            }
        }

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

        private static ulong AppMemoryUsageLimit
        {
            get
            {
#if WINDOWS_UWP
                return MemoryManager.AppMemoryUsageLimit;
#else
                return ConvertMegabytesToBytes(SystemInfo.systemMemorySize);
#endif
            }
        }

        /// <inheritdoc />
        public bool ShowProfiler { get; set; }

        private float frameSampleRate = 0.1f;
        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool ShowConsole { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the data provider as assigned in configuration.</param>
        /// <param name="priority">The priority of the data provider.</param>
        public MixedRealityDiagnosticsDataProvider(string name, uint priority, MixedRealityDiagnosticsDataProviderProfile profile)
            : base(name, priority)
        {
            this.profile = profile;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            // Apply profile settings
            ShowProfiler = profile.ShowProfiler;
            FrameSampleRate = profile.FrameSampleRate;

            TryCreateVisualizer();
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            //ComputeFrameRate();
            //ComputeMemoryData();

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].UpdateDiagnostics(new DiagnosticsData
                {
                    ShowConsole = ShowConsole,
                    ShowProfiler = ShowProfiler,
                    FrameSampleRate = FrameSampleRate
                });
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <inheritdoc />
        public void Register(IMixedRealityDiagnosticsHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <inheritdoc />
        public void Unregister(IMixedRealityDiagnosticsHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }

        //private void ComputeMemoryData()
        //{
        //    // Update memory statistics.
        //    ulong limit = AppMemoryUsageLimit;

        //    if (limit != limitMemoryUsage)
        //    {
        //        if (window.activeSelf && WillDisplayedMemoryUsageDiffer(limitMemoryUsage, limit, profile.DisplayedDecimalDigits))
        //        {
        //            MemoryUsageToString(stringBuffer, profile.DisplayedDecimalDigits, limitMemoryText, LimitMemoryString, limit);
        //        }

        //        limitMemoryUsage = limit;
        //    }

        //    ulong usage = AppMemoryUsage;

        //    if (usage != memoryUsage)
        //    {
        //        var scale = usedAnchor.localScale;
        //        scale.x = (float)usage / limitMemoryUsage;
        //        usedAnchor.localScale = scale;

        //        if (window.activeSelf && WillDisplayedMemoryUsageDiffer(memoryUsage, usage, profile.DisplayedDecimalDigits))
        //        {
        //            MemoryUsageToString(stringBuffer, profile.DisplayedDecimalDigits, usedMemoryText, UsedMemoryString, usage);
        //        }

        //        memoryUsage = usage;
        //    }

        //    if (memoryUsage > peakMemoryUsage)
        //    {
        //        var scale = peakAnchor.localScale;
        //        scale.x = (float)memoryUsage / limitMemoryUsage;
        //        peakAnchor.localScale = scale;

        //        if (window.activeSelf && WillDisplayedMemoryUsageDiffer(peakMemoryUsage, memoryUsage, profile.DisplayedDecimalDigits))
        //        {
        //            MemoryUsageToString(stringBuffer, profile.DisplayedDecimalDigits, peakMemoryText, PeakMemoryString, memoryUsage);
        //        }

        //        peakMemoryUsage = memoryUsage;
        //    }
        //}

        //private void ComputeFrameRate()
        //{
        //    // Capture frame timings every frame and read from it depending on the frameSampleRate.
        //    FrameTimingManager.CaptureFrameTimings();

        //    ++frameCount;
        //    var elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;

        //    if (elapsedSeconds >= frameSampleRate)
        //    {
        //        int cpuFrameRate = (int)(1.0f / (elapsedSeconds / frameCount));
        //        int gpuFrameRate = 0;

        //        // Many platforms do not yet support the FrameTimingManager. When timing data is returned from the FrameTimingManager we will use
        //        // its timing data, else we will depend on the stopwatch.
        //        uint frameTimingsCount = FrameTimingManager.GetLatestTimings((uint)Mathf.Min(frameCount, maxFrameTimings), frameTimings);

        //        if (frameTimingsCount != 0)
        //        {
        //            AverageFrameTiming(frameTimings, frameTimingsCount, out var cpuFrameTime, out var gpuFrameTime);
        //            cpuFrameRate = (int)(1.0f / (cpuFrameTime / frameCount));
        //            gpuFrameRate = (int)(1.0f / (gpuFrameTime / frameCount));
        //        }

        //        // Update frame rate text.
        //        cpuFrameRateText.text = cpuFrameRateStrings[Mathf.Clamp(cpuFrameRate, 0, MaxTargetFrameRate)];

        //        if (gpuFrameRate != 0)
        //        {
        //            gpuFrameRateText.gameObject.SetActive(true);
        //            gpuFrameRateText.text = gpuFrameRateStrings[Mathf.Clamp(gpuFrameRate, 0, MaxTargetFrameRate)];
        //        }

        //        // Update frame colors.
        //        for (int i = FrameRange - 1; i > 0; --i)
        //        {
        //            frameInfoColors[i] = frameInfoColors[i - 1];
        //        }

        //        // Ideally we would query a device specific API (like the HolographicFramePresentationReport) to detect missed frames.
        //        // But, many of these APIs are inaccessible in Unity. Currently missed frames are assumed when the average cpuFrameRate 
        //        // is under the target frame rate.
        //        frameInfoColors[0] = (cpuFrameRate < ((int)(AppFrameRate) - 1)) ? profile.MissedFrameRateColor : profile.TargetFrameRateColor;
        //        frameInfoPropertyBlock.SetVectorArray(colorId, frameInfoColors);

        //        // Reset timers.
        //        frameCount = 0;
        //        stopwatch.Reset();
        //        stopwatch.Start();
        //    }
        //}

        private static void AverageFrameTiming(FrameTiming[] frameTimings, uint frameTimingsCount, out float cpuFrameTime, out float gpuFrameTime)
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

        private static int MemoryItoA(int value, char[] stringBuffer, int bufferIndex)
        {
            int startIndex = bufferIndex;

            for (; value != 0; value /= 10)
            {
                stringBuffer[bufferIndex++] = (char)((char)(value % 10) + '0');
            }

            for (int endIndex = bufferIndex - 1; startIndex < endIndex; ++startIndex, --endIndex)
            {
                var temp = stringBuffer[startIndex];
                stringBuffer[startIndex] = stringBuffer[endIndex];
                stringBuffer[endIndex] = temp;
            }

            return bufferIndex;
        }

        private static void MemoryUsageToString(char[] stringBuffer, int displayedDecimalDigits, TextMesh textMesh, string prefixString, ulong memoryUsage)
        {
            // Using a custom number to string method to avoid the overhead, and allocations, of built in string.Format/StringBuilder methods.
            // We can also make some assumptions since the domain of the input number (memoryUsage) is known.
            var memoryUsageMb = ConvertBytesToMegabytes(memoryUsage);
            int memoryUsageIntegerDigits = (int)memoryUsageMb;
            int memoryUsageFractionalDigits = (int)((memoryUsageMb - memoryUsageIntegerDigits) * Mathf.Pow(10.0f, displayedDecimalDigits));
            int bufferIndex = 0;

            for (int i = 0; i < prefixString.Length; ++i)
            {
                stringBuffer[bufferIndex++] = prefixString[i];
            }

            bufferIndex = MemoryItoA(memoryUsageIntegerDigits, stringBuffer, bufferIndex);
            stringBuffer[bufferIndex++] = '.';

            if (memoryUsageFractionalDigits != 0)
            {
                bufferIndex = MemoryItoA(memoryUsageFractionalDigits, stringBuffer, bufferIndex);
            }
            else
            {
                for (int i = 0; i < displayedDecimalDigits; ++i)
                {
                    stringBuffer[bufferIndex++] = '0';
                }
            }

            stringBuffer[bufferIndex++] = 'M';
            stringBuffer[bufferIndex++] = 'B';
            textMesh.text = new string(stringBuffer, 0, bufferIndex);
        }

        private static bool WillDisplayedMemoryUsageDiffer(ulong oldUsage, ulong newUsage, int displayedDecimalDigits)
        {
            var oldUsageMBs = ConvertBytesToMegabytes(oldUsage);
            var newUsageMBs = ConvertBytesToMegabytes(newUsage);
            var decimalPower = Mathf.Pow(10.0f, displayedDecimalDigits);

            return (int)(oldUsageMBs * decimalPower) != (int)(newUsageMBs * decimalPower);
        }

        private static ulong ConvertMegabytesToBytes(int megabytes)
        {
            return ((ulong)megabytes * 1024UL) * 1024UL;
        }

        private static float ConvertBytesToMegabytes(ulong bytes)
        {
            return (bytes / 1024.0f) / 1024.0f;
        }

        private void TryCreateVisualizer()
        {
            if (profile.VisualizationPrefab == null)
            {
                Debug.LogError($"Failed to create a diagnostics visuailzer for {GetType().Name}. Check if a visualizer prefab is assigned in the configuration profile.");
                return;
            }

            Object.Instantiate(profile.VisualizationPrefab, MixedRealityToolkit.DiagnosticsSystem.DiagnosticsTransform);
        }
    }
}