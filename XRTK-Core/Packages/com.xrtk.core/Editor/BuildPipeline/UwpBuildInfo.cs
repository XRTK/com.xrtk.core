// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline
{
    public class UwpBuildInfo : BuildInfo
    {
        /// <inheritdoc />
        public override BuildTarget BuildTarget => BuildTarget.WSAPlayer;

        /// <inheritdoc />
        public override Version Version { get; set; } = PlayerSettings.WSA.packageVersion;

        /// <summary>
        /// The name of the Visual Studio .sln file generated.
        /// </summary>
        public string SolutionName { get; } = $"{PlayerSettings.productName}\\{PlayerSettings.productName}.sln";

        /// <summary>
        /// Build the appx bundle after building Unity Player?
        /// </summary>
        public bool BuildAppx { get; set; } = false;

        /// <summary>
        /// Force rebuilding the appx bundle?
        /// </summary>
        public bool RebuildAppx { get; set; } = false;

        public string UwpSdk { get; } = EditorUserBuildSettings.wsaUWPSDK;

        public string MinSdk { get; } = EditorUserBuildSettings.wsaMinUWPSDK;

        public PlayerSettings.WSATargetFamily[] BuildTargetFamilies { get; } = GetFamilies();

        private static PlayerSettings.WSATargetFamily[] GetFamilies()
        {
            var values = (PlayerSettings.WSATargetFamily[])Enum.GetValues(typeof(PlayerSettings.WSATargetFamily));
            return values.Where(PlayerSettings.WSA.GetTargetDeviceFamily).ToArray();
        }

        /// <inheritdoc />
        public override void OnPostprocessBuild(BuildReport buildReport)
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget) { return; }

            if (buildReport.summary.result == BuildResult.Failed)
            {
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog($"{PlayerSettings.productName} Build {buildReport.summary.result}!", "See console for details", "OK");
                }
            }
            else
            {
                if (BuildAppx ||
                    !Application.isBatchMode &&
                    !EditorUtility.DisplayDialog(PlayerSettings.productName, "Build Complete", "OK", "Build AppX"))
                {
                    UwpAppxBuildTools.BuildAppx(this);
                }
            }
        }

        /// <inheritdoc />
        public override void ParseCommandLineArgs()
        {
            base.ParseCommandLineArgs();

            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-buildAppx":
                        BuildAppx = true;
                        break;
                    case "-rebuildAppx":
                        RebuildAppx = true;
                        break;
                }
            }
        }
    }
}
