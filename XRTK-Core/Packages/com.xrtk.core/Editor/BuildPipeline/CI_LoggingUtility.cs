// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Editor.BuildPipeline
{
    public interface ICILogger : ILogHandler
    {
        string Error { get; }

        string Warning { get; }
    }

    public class AzurePipelinesLogger : ICILogger
    {
        public string Error => "##vso[task.logissue type=error;]";

        public string Warning => "##vso[task.logissue type=warning;]";

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception, Object context)
        {
            throw new NotImplementedException();
        }
    }

    public class GitHubActionsLogger : ICILogger
    {
        public string Error => "::error::";
        public string Warning => "::warning::";

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception, Object context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands
    /// </summary>
    [InitializeOnLoad]
    public static class CILoggingUtility
    {
        public static ICILogger Logger { get; }

        public static bool LoggingEnabled { get; set; } = Application.isBatchMode;

        public readonly static List<string> IgnoredLogs = new List<string>
        {
            @".android\repositories.cfg could not be loaded",
            @"Using symlinks in Unity projects may cause your project to become corrupted",
            @"Assembly 'Newtonsoft.Json' has non matching file name:",
            @"Skipping WindowsDictationDataProvider registration",
            @"Skipping WindowsSpeechDataProvider registration",
            @"Cancelling DisplayDialog: Built in VR Detected XR Plug-in Management has detected that this project is using built in VR.",
            @"Reference rewriter: Error: method `System.Numerics.Vector3[] Windows.Perception.Spatial.SpatialStageFrameOfReference::TryGetMovementBounds(Windows.Perception.Spatial.SpatialCoordinateSystem)` doesn't exist in target framework. It is referenced from XRTK.WindowsMixedReality.dll at System.Boolean XRTK.WindowsMixedReality.Providers.BoundarySystem.WindowsMixedRealityBoundaryDataProvider::TryGetBoundaryGeometry"
        };

        static CILoggingUtility()
        {
            if (LoggingEnabled)
            {
                var ciVar = Environment.GetEnvironmentVariable("CI");
                Debug.Log($"CI: {ciVar}");

                var agentVar = Environment.GetEnvironmentVariable("AGENT_NAME");
                Debug.Log($"AGENT: {agentVar}");

                if (!string.IsNullOrWhiteSpace(agentVar))
                {
                    Logger = new AzurePipelinesLogger();
                }

                var githubVar = Environment.GetEnvironmentVariable("GITHUB_WORKFLOW");
                Debug.Log($"GitHub: {githubVar}");

                if (!string.IsNullOrWhiteSpace(githubVar))
                {
                    Logger = new GitHubActionsLogger();
                }

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
                    Debug.Log($"{Logger.Error}{condition}\n{stacktrace}");
                    break;
                case LogType.Warning:
                    Debug.Log($"{Logger.Warning}{condition}\n{stacktrace}");
                    break;
            }
        }
    }
}
