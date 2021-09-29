// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline.Logging
{
    [InitializeOnLoad]
    public static class CILoggingUtility
    {
        public static ICILogger Logger { get; set; }

        public static bool LoggingEnabled { get; set; } = Application.isBatchMode;

        public static readonly List<string> IgnoredLogs = new List<string>
        {
            @".android\repositories.cfg could not be loaded",
            @"Using symlinks in Unity projects may cause your project to become corrupted",
            @"Skipping WindowsDictationDataProvider registration",
            @"Skipping WindowsSpeechDataProvider registration",
            @"Cancelling DisplayDialog: Built in VR Detected XR Plug-in Management has detected that this project is using built in VR.",
            @"Reference Rewriter found some errors while running with command",
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
