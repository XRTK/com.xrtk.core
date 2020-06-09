// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    [InitializeOnLoad]
    public static class LoggingUtility
    {
        private static readonly string[] IgnoredLogs =
        {
            "Using symlinks in Unity projects may cause your project to become corrupted",
        };

        static LoggingUtility()
        {
            if (Application.isBatchMode)
            {
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }

        private static void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (IgnoredLogs.Any(condition.Contains))
            {
                return;
            }

            switch (type)
            {
                case LogType.Error:
                    Debug.Log($"##vso[task.logissue type=error;]{condition}\n{stacktrace}");
                    break;
                case LogType.Assert:
                    Debug.Log($"##vso[task.logissue type=error;]{condition}\n{stacktrace}");
                    break;
                case LogType.Warning:
                    Debug.Log($"##vso[task.logissue type=warning;]{condition}\n{stacktrace}");
                    break;
                case LogType.Exception:
                    Debug.Log($"##vso[task.logissue type=error;]{condition}\n{stacktrace}");
                    break;
            }
        }
    }
}
