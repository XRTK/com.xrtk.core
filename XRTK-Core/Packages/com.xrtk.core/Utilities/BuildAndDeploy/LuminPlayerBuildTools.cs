// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Extensions;

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
            await new Process().RunAsync($"mldb install -u \"{buildInfo.OutputDirectory}\"", true);
        }
    }
}