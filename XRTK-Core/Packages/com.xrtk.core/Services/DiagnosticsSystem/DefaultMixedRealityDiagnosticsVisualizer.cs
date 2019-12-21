// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;
using TMPro;
using System.Globalization;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default profiler diagnostics visualizer implementation.
    /// </summary>
    public class DefaultMixedRealityDiagnosticsVisualizer : BaseMixedRealityDiagnosticsHandler
    {
        private static readonly int maxStringLength = 32;
        private static readonly string usedMemoryPrefix = "Used: ";
        private static readonly string peakMemoryPrefix = "Peak: ";
        private static readonly string limitMemoryPrefix = "Limit: ";
        private readonly char[] stringBuffer = new char[maxStringLength];

        [Range(0, 3)]
        [SerializeField]
        [Tooltip("How many decimal places to display on numeric strings.")]
        private int displayedDecimalDigits = 1;

        [SerializeField]
        [Tooltip("The text component used to display the memory usage info.")]
        private TextMeshProUGUI memoryUsedText;

        [SerializeField]
        [Tooltip("The text component used to display the memory peak info.")]
        private TextMeshProUGUI memoryPeakText;

        [SerializeField]
        [Tooltip("The text component used to display the memory limit info.")]
        private TextMeshProUGUI memoryLimitText;

        [SerializeField]
        [Tooltip("The text component used to display the application build version and identifier info.")]
        private TextMeshProUGUI applicationBuildText;

        [SerializeField]
        [Tooltip("The text component used to display the CPU FPS information.")]
        private TextMeshProUGUI cpuFrameRateText;

        [SerializeField]
        [Tooltip("The text component used to display the GPU FPS information.")]
        private TextMeshProUGUI gpuFrameRateText;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            applicationBuildText.text = MixedRealityToolkit.DiagnosticsSystem.ApplicationSignature;
        }

        /// <inheritdoc />
        public override void OnMissedFramesChanged(bool[] missedFrames)
        {

        }

        /// <inheritdoc />
        public override void OnFrameRateChanged(int oldFPS, int newFPS, bool isGPU)
        {
            string displayedDecimalFormat = $"{{0:F{displayedDecimalDigits}}}";

            StringBuilder stringBuilder = new StringBuilder(32);
            StringBuilder millisecondStringBuilder = new StringBuilder(16);

            float milliseconds = (newFPS == 0) ? 0.0f : (1.0f / newFPS) * 1000.0f;
            millisecondStringBuilder.AppendFormat(displayedDecimalFormat, milliseconds.ToString(CultureInfo.InvariantCulture));

            // GPU
            if (isGPU)
            {
                stringBuilder.AppendFormat("GPU: {0} fps ({1} ms)", newFPS.ToString(), millisecondStringBuilder);
                gpuFrameRateText.text = stringBuilder.ToString();
                millisecondStringBuilder.Length = 0;
                stringBuilder.Length = 0;
            }
            else
            {
                // CPU
                stringBuilder.AppendFormat("CPU: {0} fps ({1} ms)", newFPS.ToString(), millisecondStringBuilder);
                cpuFrameRateText.text = stringBuilder.ToString();
                stringBuilder.Length = 0;
            }
        }

        /// <inheritdoc />
        public override void OnMemoryUsageChanged(ulong oldMemoryUsage, ulong newMemoryUsage)
        {
            if (WillDisplayedMemoryDiffer(oldMemoryUsage, newMemoryUsage, displayedDecimalDigits))
            {
                memoryUsedText.text = MemoryToString(stringBuffer, displayedDecimalDigits, usedMemoryPrefix, newMemoryUsage);
            }
        }

        /// <inheritdoc />
        public override void OnMemoryLimitChanged(ulong oldMemoryLimit, ulong newMemoryLimit)
        {
            if (WillDisplayedMemoryDiffer(oldMemoryLimit, newMemoryLimit, displayedDecimalDigits))
            {
                memoryLimitText.text = MemoryToString(stringBuffer, displayedDecimalDigits, limitMemoryPrefix, newMemoryLimit);
            }
        }

        /// <inheritdoc />
        public override void OnMemoryPeakChanged(ulong oldMemoryPeak, ulong newMemoryPeak)
        {
            if (WillDisplayedMemoryDiffer(oldMemoryPeak, newMemoryPeak, displayedDecimalDigits))
            {
                memoryPeakText.text = MemoryToString(stringBuffer, displayedDecimalDigits, peakMemoryPrefix, newMemoryPeak);
            }
        }

        /// <inheritdoc />
        public override void OnLogReceived(string message, LogType type)
        {

        }

        private string MemoryToString(char[] stringBuffer, int displayedDecimalDigits, string prefixString, ulong memory)
        {
            // Using a custom number to string method to avoid the overhead, and allocations, of built in string.Format/StringBuilder methods.
            // We can also make some assumptions since the domain of the input number (memoryUsage) is known.
            var memoryUsageMb = DiagnosticsUtils.ConvertBytesToMegabytes(memory);
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

            return new string(stringBuffer, 0, bufferIndex);
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

        private static bool WillDisplayedMemoryDiffer(ulong oldUsage, ulong newUsage, int displayedDecimalDigits)
        {
            var oldUsageMBs = DiagnosticsUtils.ConvertBytesToMegabytes(oldUsage);
            var newUsageMBs = DiagnosticsUtils.ConvertBytesToMegabytes(newUsage);
            var decimalPower = Mathf.Pow(10.0f, displayedDecimalDigits);

            return (int)(oldUsageMBs * decimalPower) != (int)(newUsageMBs * decimalPower);
        }
    }
}