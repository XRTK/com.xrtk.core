// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Extensions;
using XRTK.Utilities.Editor;
using Debug = UnityEngine.Debug;

namespace XRTK.Utilities.Build
{
    /// <summary>
    /// Cross platform player build tools
    /// </summary>
    public static class UnityPlayerBuildTools
    {
        // Build configurations. Exactly one of these should be defined for any given build.
        public const string BuildSymbolDebug = "debug";
        public const string BuildSymbolRelease = "release";
        public const string BuildSymbolMaster = "master";

        /// <summary>
        /// Starts the build process
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <returns>The <see cref="BuildReport"/> from Unity's <see cref="BuildPipeline"/></returns>
        public static BuildReport BuildUnityPlayer(IBuildInfo buildInfo)
        {
            EditorUtility.DisplayProgressBar("Build Pipeline", "Gathering Build Data...", 0.25f);

            // Call the pre-build action, if any
            buildInfo.PreBuildAction?.Invoke(buildInfo);

            var buildTargetGroup = buildInfo.BuildTarget.GetGroup();
            var playerBuildSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!string.IsNullOrEmpty(playerBuildSymbols))
            {
                if (buildInfo.HasConfigurationSymbol())
                {
                    buildInfo.AppendWithoutConfigurationSymbols(playerBuildSymbols);
                }
                else
                {
                    buildInfo.AppendSymbols(playerBuildSymbols.Split(';'));
                }
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildInfo.BuildSymbols);
            }

            if ((buildInfo.BuildOptions & BuildOptions.Development) == BuildOptions.Development &&
                !buildInfo.HasConfigurationSymbol())
            {
                buildInfo.AppendSymbols(BuildSymbolDebug);
            }

            if (buildInfo.HasAnySymbols(BuildSymbolDebug))
            {
                buildInfo.BuildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
            }

            if (buildInfo.HasAnySymbols(BuildSymbolRelease))
            {
                // Unity automatically adds the DEBUG symbol if the BuildOptions.Development flag is
                // specified. In order to have debug symbols and the RELEASE symbols we have to
                // inject the symbol Unity relies on to enable the /debug+ flag of csc.exe which is "DEVELOPMENT_BUILD"
                buildInfo.AppendSymbols("DEVELOPMENT_BUILD");
            }

            var oldColorSpace = PlayerSettings.colorSpace;

            if (buildInfo.ColorSpace.HasValue)
            {
                PlayerSettings.colorSpace = buildInfo.ColorSpace.Value;
            }

            var oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var oldBuildTargetGroup = oldBuildTarget.GetGroup();

            if (EditorUserBuildSettings.activeBuildTarget != buildInfo.BuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildInfo.BuildTarget);
            }

            buildInfo.OutputDirectory = $"{buildInfo.OutputDirectory}/{PlayerSettings.productName}";

            switch (buildInfo.BuildTarget)
            {
                case BuildTarget.Lumin:
                    buildInfo.OutputDirectory += ".mpk";
                    break;
                case BuildTarget.Android:
                    buildInfo.OutputDirectory += ".apk";
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildInfo.OutputDirectory += ".exe";
                    break;
            }

            BuildReport buildReport = default;

            if (Application.isBatchMode)
            {
                foreach (var scene in buildInfo.Scenes)
                {
                    Debug.Log($"BuildScene->{scene.path}");
                }
            }

            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                    buildInfo.Scenes.ToArray(),
                    buildInfo.OutputDirectory,
                    buildInfo.BuildTarget,
                    buildInfo.BuildOptions);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }

            PlayerSettings.colorSpace = oldColorSpace;

            if (EditorUserBuildSettings.activeBuildTarget != oldBuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
            }

            // Call the post-build action, if any
            buildInfo.PostBuildAction?.Invoke(buildInfo, buildReport);

            return buildReport;
        }

        /// <summary>
        /// Force Unity To Write Project Files
        /// </summary>
        public static void SyncSolution()
        {
            var syncVs = Type.GetType("UnityEditor.SyncVS,UnityEditor");
            Debug.Assert(syncVs != null);
            var syncSolution = syncVs.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            Debug.Assert(syncSolution != null);
            syncSolution.Invoke(null, null);
        }

        /// <summary>
        /// Start a build using Unity's command line. Valid arguments:<para/>
        /// -autoIncrement : Increments the build revision number.<para/>
        /// -sceneList : A csv of a list of scenes to include in the build.<para/>
        /// -sceneListFile : A json file with a list of scenes to include in the build.<para/>
        /// -buildOutput : The target directory you'd like the build to go.<para/>
        /// -colorSpace : The <see cref="ColorSpace"/> settings for the build.<para/>
        /// -x86 / -x64 : The target build platform. (Default is x86)<para/>
        /// -debug / -release / -master : The target build configuration. (Default is master)<para/>
        ///
        /// UWP Platform Specific arguments:<para/>
        /// -buildAppx : Builds the appx bundle after the Unity Build step.<para/>
        /// -rebuildAppx : Rebuild the appx bundle.<para/>
        /// </summary>
        [UsedImplicitly]
        public static async void StartCommandLineBuild()
        {
            // We don't need stack traces on all our logs. Makes things a lot easier to read.
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.Log($"Starting command line build for {EditorUserBuildSettings.activeBuildTarget}...");
            EditorAssemblyReloadManager.LockReloadAssemblies = true;

            bool success;
            try
            {
                SyncSolution();

                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.WSAPlayer:
                        success = await UwpPlayerBuildTools.BuildPlayer(new UwpBuildInfo(true));
                        break;
                    default:
                        var buildInfo = new BuildInfo(true) as IBuildInfo;
                        ParseBuildCommandLine(ref buildInfo);
                        var buildResult = BuildUnityPlayer(buildInfo);
                        success = buildResult.summary.result == BuildResult.Succeeded;
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Build Failed!\n{e.Message}\n{e.StackTrace}");
                success = false;
            }

            Debug.Log($"Exiting command line build... Build success? {success}");
            EditorApplication.Exit(success ? 0 : 1);
        }

        internal static bool CheckBuildScenes()
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                return EditorUtility.DisplayDialog(
                    "Attention!",
                    "No scenes are present in the build settings.\n" +
                    "The current scene will be the one built.\n\n" +
                    "Do you want to cancel and add one?",
                    "Continue Anyway",
                    "Cancel Build");
            }

            return true;
        }

        /// <summary>
        /// Get the Unity Project Root Path.
        /// </summary>
        /// <returns>The full path to the project's root.</returns>
        public static string GetProjectPath()
        {
            return Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
        }

        /// <summary>
        /// Parses the command like arguments.
        /// </summary>
        /// <param name="buildInfo"></param>
        public static void ParseBuildCommandLine(ref IBuildInfo buildInfo)
        {
            var arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-autoIncrement":
                        buildInfo.AutoIncrement = true;
                        break;
                    case "-sceneList":
                        buildInfo.Scenes = buildInfo.Scenes.Union(SplitSceneList(arguments[++i]));
                        break;
                    case "-sceneListFile":
                        buildInfo.Scenes = buildInfo.Scenes.Union(SplitSceneList(File.ReadAllText(arguments[++i])));
                        break;
                    case "-buildOutput":
                        buildInfo.OutputDirectory = arguments[++i];
                        break;
                    case "-colorSpace":
                        buildInfo.ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), arguments[++i]);
                        break;
                    case "-x86":
                    case "-x64":
                        buildInfo.BuildPlatform = arguments[i].Substring(1);
                        break;
                    case "-debug":
                    case "-master":
                    case "-release":
                        buildInfo.Configuration = arguments[i].Substring(1).ToLower();
                        break;
                }
            }
        }

        private static IEnumerable<EditorBuildSettingsScene> SplitSceneList(string sceneList)
        {
            var sceneListArray = sceneList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return sceneListArray
                .Where(scenePath => !string.IsNullOrWhiteSpace(scenePath))
                .Select(scene => new EditorBuildSettingsScene(scene.Trim(), true));
        }

        /// <summary>
        /// Restores any nuget packages at the path specified.
        /// </summary>
        /// <param name="nugetPath"></param>
        /// <param name="storePath"></param>
        /// <returns>True, if the nuget packages were successfully restored.</returns>
        public static async Task<bool> RestoreNugetPackagesAsync(string nugetPath, string storePath)
        {
            Debug.Assert(File.Exists(nugetPath));
            Debug.Assert(Directory.Exists(storePath));

            await new Process().RunAsync($"restore \"{storePath}/project.json\"", nugetPath);

            return File.Exists($"{storePath}\\project.lock.json");
        }
    }
}