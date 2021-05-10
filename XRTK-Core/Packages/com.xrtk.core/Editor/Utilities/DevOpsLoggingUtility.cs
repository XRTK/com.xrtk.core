// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    [InitializeOnLoad]
    public static class DevOpsLoggingUtility
    {
        public static bool LoggingEnabled { get; set; } = Application.isBatchMode;

        public static List<string> IgnoredLogs = new List<string>()
        {
            @".android\repositories.cfg could not be loaded",
            @"Using symlinks in Unity projects may cause your project to become corrupted",
            @"Assembly 'Newtonsoft.Json' has non matching file name:",
            @"Skipping WindowsDictationDataProvider registration",
            @"Skipping WindowsSpeechDataProvider registration",
            @"Cancelling DisplayDialog: Built in VR Detected XR Plug-in Management has detected that this project is using built in VR.",
            @"Reference rewriter: Error: method `System.Numerics.Vector3[] Windows.Perception.Spatial.SpatialStageFrameOfReference::TryGetMovementBounds(Windows.Perception.Spatial.SpatialCoordinateSystem)` doesn't exist in target framework. It is referenced from XRTK.WindowsMixedReality.dll at System.Boolean XRTK.WindowsMixedReality.Providers.BoundarySystem.WindowsMixedRealityBoundaryDataProvider::TryGetBoundaryGeometry"
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
            if (!LoggingEnabled || IgnoredLogs.Any(condition.Contains))
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
