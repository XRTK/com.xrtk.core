// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Platforms;
using XRTK.Editor.Utilities;

namespace XRTK.Editor.BuildPipeline
{
    /// <summary>
    /// Build and Deploy Specific Editor Preferences for the Build and Deploy Window.
    /// </summary>
    public static class BuildDeployPreferences
    {
        private static string appDataPath = null;

        public static string ApplicationDataPath => appDataPath ?? (appDataPath = Application.dataPath);

        // Constants
        private const string EDITOR_PREF_BUILD_DIR = "BuildDeployWindow_BuildDir";

        /// <summary>
        /// The Build Directory that the Mixed Reality Toolkit will build to.
        /// </summary>
        /// <remarks>
        /// This is a root build folder path. Each platform build will be put into a child directory with the name of the current active build target.
        /// </remarks>
        public static string BuildDirectory
        {
            get
            {
                if (MixedRealityPreferences.CurrentPlatformTarget != null &&
                    MixedRealityPreferences.CurrentPlatformTarget.GetType() != typeof(AllPlatforms))
                {
                    return $"{EditorPreferences.Get(EDITOR_PREF_BUILD_DIR, "Builds")}/{MixedRealityPreferences.CurrentPlatformTarget.Name}";
                }
                return $"{EditorPreferences.Get(EDITOR_PREF_BUILD_DIR, "Builds")}/{EditorUserBuildSettings.activeBuildTarget}";
            }
            set
            {
                if (MixedRealityPreferences.CurrentPlatformTarget != null &&
                    MixedRealityPreferences.CurrentPlatformTarget.GetType() != typeof(AllPlatforms))
                {
                    EditorPreferences.Set(EDITOR_PREF_BUILD_DIR, value.Replace($"/{MixedRealityPreferences.CurrentPlatformTarget.Name}", string.Empty));
                }
                else
                {
                    EditorPreferences.Set(EDITOR_PREF_BUILD_DIR, value.Replace($"/{EditorUserBuildSettings.activeBuildTarget}", string.Empty));
                }
            }
        }

        /// <summary>
        /// The absolute path to <see cref="BuildDirectory"/>
        /// </summary>
        public static string AbsoluteBuildDirectory
        {
            get
            {
                string rootBuildDirectory = BuildDirectory;
                int dirCharIndex = rootBuildDirectory.IndexOf("/", StringComparison.Ordinal);

                if (dirCharIndex != -1)
                {
                    rootBuildDirectory = rootBuildDirectory.Substring(0, dirCharIndex);
                }

                return Path.GetFullPath(Path.Combine(Path.Combine(ApplicationDataPath, ".."), rootBuildDirectory));
            }
        }
    }
}
