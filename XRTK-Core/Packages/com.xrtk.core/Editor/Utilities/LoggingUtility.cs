using System;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    [InitializeOnLoad]
    public static class LoggingUtility
    {
        static LoggingUtility()
        {
            if (Application.isBatchMode)
            {
                Application.logMessageReceived += OnLogMessageReceived;

                Debug.LogError("Test Error");
                Debug.LogWarning("Test Warning");
                Debug.LogException(new Exception("Test Exception"));
                Debug.Assert(false, "Test Assert");
            }
        }

        private static void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            switch (type)
            {
                // sourcepath=consoleapp/main.cs;linenumber=1;columnnumber=1;code=100;
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
