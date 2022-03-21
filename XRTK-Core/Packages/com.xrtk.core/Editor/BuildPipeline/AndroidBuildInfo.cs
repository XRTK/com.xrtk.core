// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Editor.BuildPipeline
{
    [RuntimePlatform(typeof(AndroidPlatform))]
    public class AndroidBuildInfo : BuildInfo
    {
        private bool prevApkExpansionSettings;

        /// <inheritdoc />
        public override BuildTarget BuildTarget => BuildTarget.Android;

        /// <inheritdoc />
        public override IMixedRealityPlatform BuildPlatform => new AndroidPlatform();

        /// <inheritdoc />
        public override string ExecutableFileExtension => ".apk";

        public bool UseExpansionFiles { get; set; }

        public override void ParseCommandLineArgs()
        {
            base.ParseCommandLineArgs();

            var arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-splitApk":
                        UseExpansionFiles = true;
                        break;
                }
            }
        }

        /// <inheritdoc />
        public override void OnPreProcessBuild(BuildReport report)
        {
            if (!MixedRealityToolkit.ActivePlatforms.Contains(BuildPlatform) ||
                EditorUserBuildSettings.activeBuildTarget != BuildTarget)
            {
                return;
            }

            prevApkExpansionSettings = PlayerSettings.Android.useAPKExpansionFiles;

            PlayerSettings.Android.useAPKExpansionFiles = UseExpansionFiles;

            if (VersionCode.HasValue)
            {
                PlayerSettings.Android.bundleVersionCode = VersionCode.Value;
            }
            else
            {
                // Usually version codes are unique and not tied to the usual semver versions
                // see https://developer.android.com/studio/publish/versioning#appversioning
                // versionCode - A positive integer used as an internal version number.
                // This number is used only to determine whether one version is more recent than another,
                // with higher numbers indicating more recent versions. The Android system uses the
                // versionCode value to protect against downgrades by preventing users from installing
                // an APK with a lower versionCode than the version currently installed on their device.
                PlayerSettings.Android.bundleVersionCode++;
            }

            if (BuildPlatform.GetType() == typeof(AndroidPlatform))
            {
                // TODO generate manifest
            }
        }

        public override void OnPostProcessBuild(BuildReport report)
        {
            PlayerSettings.Android.useAPKExpansionFiles = prevApkExpansionSettings;
        }
    }
}
