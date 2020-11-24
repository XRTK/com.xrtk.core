// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Utilities.Build
{
    /// <summary>
    /// Class containing various utility methods to build a lumin mpk installer from a Unity project.
    /// </summary>
    public static class LuminPlayerBuildTools
    {
        /// <summary>
        /// Build the Lumin Player.
        /// </summary>
        /// <param name="buildInfo"></param>
        public static BuildReport BuildPlayer(BuildInfo buildInfo)
        {
            if (!Application.isBatchMode)
            {
                buildInfo.PostBuildAction += PostBuildAction;
            }

            return UnityPlayerBuildTools.BuildUnityPlayer(buildInfo);
        }

        private static async void PostBuildAction(IBuildInfo buildInfo, BuildReport buildReport)
        {
            // TODO Check if installation flag is set in build window

            Debug.Log("Starting installation...");

            var canInstall = false;

            try
            {
                var deviceResult = await new Process().RunAsync("mldb devices", false);

                if (deviceResult.ExitCode == 0)
                {
                    foreach (var deviceId in deviceResult.Output)
                    {
                        if (!string.IsNullOrWhiteSpace(deviceId))
                        {
                            Debug.Log(deviceId);

                            if (!deviceId.Contains("List"))
                            {
                                canInstall = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (!canInstall)
            {
                Debug.Log("No devices found, skipping installation");
                return;
            }

            try
            {
                await new Process().RunAsync($"mldb install -u \"{buildInfo.OutputDirectory}\"", true);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}