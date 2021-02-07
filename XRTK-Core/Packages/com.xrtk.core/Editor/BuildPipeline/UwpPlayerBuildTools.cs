// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace XRTK.Editor.BuildPipeline
{
    /// <summary>
    /// Class containing various utility methods to build a WSA solution from a Unity project.
    /// </summary>
    public static class UwpPlayerBuildTools
    {
        private static void ParseBuildCommandLine(ref UwpBuildInfo buildInfo)
        {
            IBuildInfo iBuildInfo = buildInfo;
            UnityPlayerBuildTools.ParseBuildCommandLine(ref iBuildInfo);

            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-buildAppx":
                        buildInfo.BuildAppx = true;
                        break;
                    case "-rebuildAppx":
                        buildInfo.RebuildAppx = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Do a build configured for UWP Applications to the specified path, returns the error from <see cref="BuildPlayer(UwpBuildInfo, CancellationToken)"/>
        /// </summary>
        /// <param name="buildDirectory"></param>
        /// <param name="showDialog">Should the user be prompted to build the appx as well?</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True, if build was successful.</returns>
        public static async Task<bool> BuildPlayer(string buildDirectory, CancellationToken cancellationToken, bool showDialog = true)
        {
            if (UnityPlayerBuildTools.CheckBuildScenes() == false)
            {
                return false;
            }

            var buildInfo = new UwpBuildInfo
            {
                OutputDirectory = buildDirectory,
                Scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled),
                CancellationToken = cancellationToken,
                BuildAppx = !showDialog
            };

            return await BuildPlayer(buildInfo, cancellationToken);
        }

        /// <summary>
        /// Build the Uwp Player.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<bool> BuildPlayer(UwpBuildInfo buildInfo, CancellationToken cancellationToken = default)
        {
            if (buildInfo.IsCommandLine)
            {
                ParseBuildCommandLine(ref buildInfo);
            }

            var buildReport = UnityPlayerBuildTools.BuildUnityPlayer(buildInfo);
            var success = buildReport != null && buildReport.summary.result == BuildResult.Succeeded;

            if (success && buildInfo.BuildAppx)
            {
                success &= await UwpAppxBuildTools.BuildAppxAsync(buildInfo, cancellationToken);
            }

            return success;
        }
    }
}