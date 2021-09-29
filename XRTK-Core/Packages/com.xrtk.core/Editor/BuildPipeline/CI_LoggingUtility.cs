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

    public abstract class AbstractCILogger : ICILogger
    {
        private readonly ILogHandler defaultLogger;

        protected AbstractCILogger()
        {
            defaultLogger = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;
        }

        /// <inheritdoc />
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (CILoggingUtility.LoggingEnabled && !CILoggingUtility.IgnoredLogs.Any(args.Contains))
            {
                switch (logType)
                {
                    case LogType.Log:
                        break;
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        format = $"{Error}{format}";
                        break;
                    case LogType.Warning:
                        format = $"{Warning}{format}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                }
            }

            defaultLogger.LogFormat(logType, context, format, args);
        }

        /// <inheritdoc />
        public void LogException(Exception exception, Object context)
        {
            defaultLogger.LogException(exception, context);
        }

        public virtual string Error => string.Empty;

        public virtual string Warning => string.Empty;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands
    /// </summary>
    public class AzurePipelinesLogger : AbstractCILogger
    {
        public override string Error => "##vso[task.logissue type=error;]";

        public override string Warning => "##vso[task.logissue type=warning;]";
    }

    /// <summary>
    /// https://docs.github.com/en/actions/learn-github-actions/workflow-commands-for-github-actions#about-workflow-commands
    /// </summary>
    public class GitHubActionsLogger : AbstractCILogger
    {
        public override string Error => "::error::";

        public override string Warning => "::warning::";
    }

    [InitializeOnLoad]
    public static class CILoggingUtility
    {
        public static ICILogger Logger { get; }

        public static bool LoggingEnabled { get; set; } = Application.isBatchMode;

        public readonly static List<string> IgnoredLogs = new List<string>
        {
            @".android\repositories.cfg could not be loaded",
            @"Using symlinks in Unity projects may cause your project to become corrupted",
            @"Skipping WindowsDictationDataProvider registration",
            @"Skipping WindowsSpeechDataProvider registration",
            @"Cancelling DisplayDialog: Built in VR Detected XR Plug-in Management has detected that this project is using built in VR.",
            @"Reference rewriter: Error: method `System.Numerics.Vector3[] Windows.Perception.Spatial.SpatialStageFrameOfReference::TryGetMovementBounds(Windows.Perception.Spatial.SpatialCoordinateSystem)` doesn't exist in target framework. It is referenced from XRTK.WindowsMixedReality.dll at System.Boolean XRTK.WindowsMixedReality.Providers.BoundarySystem.WindowsMixedRealityBoundaryDataProvider::TryGetBoundaryGeometry"
        };

        static CILoggingUtility()
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AGENT_NAME")))
            {
                Logger = new AzurePipelinesLogger();
            }

            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_WORKFLOW")))
            {
                Logger = new GitHubActionsLogger();
            }
        }
    }
}
