// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    [InitializeOnLoad]
    public static class DevOpsLoggingUtility
    {
        public static bool LoggingEnabled { get; set; } = Application.isBatchMode;

        private static readonly string[] IgnoredLogs =
        {
            "Using symlinks in Unity projects may cause your project to become corrupted",
            "Assembly 'Newtonsoft.Json' has non matching file name:",
            "Skipping WindowsSpeechDataProvider registration"
        };

        static DevOpsLoggingUtility()
        {
            if (LoggingEnabled)
            {
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }

        private static void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (!LoggingEnabled ||
                IgnoredLogs.Any(condition.Contains))
            {
                return;
            }

            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Debug.Log($"##vso[task.logissue type=error;]{condition}\n{stacktrace}");
                    break;
                case LogType.Warning:
                    Debug.Log($"##vso[task.logissue type=warning;]{condition}\n{stacktrace}");
                    break;
            }
        }
    }
}
