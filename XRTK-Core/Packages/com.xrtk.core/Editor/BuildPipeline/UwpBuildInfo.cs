// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading;
using UnityEditor;
using UnityEditor.Build.Reporting;
using XRTK.Editor.Utilities;

namespace XRTK.Editor.BuildPipeline
{
    public class UwpBuildInfo : BuildInfo
    {
        public UwpBuildInfo(bool isCommandLine = false) : base(isCommandLine)
        {
        }

        internal CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <inheritdoc />
        public override BuildTarget BuildTarget => BuildTarget.WSAPlayer;

        /// <summary>
        /// Build the appx bundle after building Unity Player?
        /// </summary>
        public bool BuildAppx { get; set; } = false;

        /// <summary>
        /// Force rebuilding the appx bundle?
        /// </summary>
        public bool RebuildAppx { get; set; } = false;

        #region Overrides of BuildInfo

        /// <inheritdoc />
        public override async void OnPostprocessBuild(BuildReport buildReport)
        {
            if (buildReport.summary.result != BuildResult.Succeeded)
            {
                EditorUtility.DisplayDialog($"{PlayerSettings.productName} WindowsStoreApp Build {buildReport.summary.result}!", "See console for details", "OK");
            }
            else
            {
                if (!EditorUtility.DisplayDialog(PlayerSettings.productName, "Build Complete", "OK", "Build AppX"))
                {
                    EditorAssemblyReloadManager.LockReloadAssemblies = true;
                    await UwpAppxBuildTools.BuildAppxAsync(this, CancellationToken);
                    EditorAssemblyReloadManager.LockReloadAssemblies = false;
                }
            }
        }

        #endregion
    }
}