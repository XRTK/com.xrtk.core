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
using XRTK.Editor.Utilities;
using XRTK.Editor.Utilities.SymbolicLinks;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.BuildAndDeploy
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

            // use https://semver.org/
            // major.minor.build
            Version version = new Version(
                (buildInfo.Version == null || buildInfo.AutoIncrement)
                    ? string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion)
                        ? string.IsNullOrWhiteSpace(Application.version)
                            ? "1.0.0"
                            : Application.version
                        : PlayerSettings.bundleVersion
                    : buildInfo.Version.ToString(3));

            // Only auto incitement if the version wasn't specified in the build info.
            if (buildInfo.Version == null &&
                buildInfo.AutoIncrement)
            {
                version = new Version(version.Major, version.Minor, version.Build + 1);
            }

            // Updates the Application.version and syncs Android and iOS bundle version strings
            PlayerSettings.bundleVersion = version.ToString(3);
            // Update Lumin bc the Application.version isn't synced like Android & iOS
            PlayerSettings.Lumin.versionName = PlayerSettings.bundleVersion;
            // Update WSA bc the Application.version isn't synced line Android & iOS
            PlayerSettings.WSA.packageVersion = version;

            var buildTargetGroup = buildInfo.BuildTarget.GetGroup();
            var oldBuildIdentifier = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);

            if (!string.IsNullOrWhiteSpace(buildInfo.BundleIdentifier))
            {
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, buildInfo.BundleIdentifier);
            }

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

            var cacheIl2Cpp = true;

            switch (buildInfo.BuildTarget)
            {
                case BuildTarget.Lumin:

                    if (buildInfo.VersionCode.HasValue)
                    {
                        PlayerSettings.Lumin.versionCode = buildInfo.VersionCode.Value;
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
                        PlayerSettings.Lumin.versionCode++;
                    }

                    buildInfo.OutputDirectory += ".mpk";

                    if (Directory.Exists($"{Directory.GetParent(Application.dataPath)}\\Library\\Mabu"))
                    {
                        Directory.Delete($"{Directory.GetParent(Application.dataPath)}\\Library\\Mabu", true);
                    }
                    break;
                case BuildTarget.Android:
                    if (buildInfo.VersionCode.HasValue)
                    {
                        PlayerSettings.Android.bundleVersionCode = buildInfo.VersionCode.Value;
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

                    buildInfo.OutputDirectory += ".apk";
                    cacheIl2Cpp = false;
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildInfo.OutputDirectory += ".exe";
                    break;
            }

            var prevIl2CppArgs = PlayerSettings.GetAdditionalIl2CppArgs();

            if (cacheIl2Cpp)
            {
                var il2cppCache = $"{Directory.GetParent(Application.dataPath)}\\Library\\il2cpp_cache\\{buildInfo.BuildTarget}";

                if (!Directory.Exists(il2cppCache))
                {
                    Directory.CreateDirectory(il2cppCache);
                }

                File.WriteAllText($"{il2cppCache}\\xrtk.lock", string.Empty);
                PlayerSettings.SetAdditionalIl2CppArgs($"--cachedirectory=\"{il2cppCache}\"");
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

            PlayerSettings.SetAdditionalIl2CppArgs(prevIl2CppArgs);
            PlayerSettings.colorSpace = oldColorSpace;

            if (EditorUserBuildSettings.activeBuildTarget != oldBuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
            }

            if (PlayerSettings.GetApplicationIdentifier(oldBuildTargetGroup) != oldBuildIdentifier)
            {
                PlayerSettings.SetApplicationIdentifier(oldBuildTargetGroup, oldBuildIdentifier);
            }

            // Call the post-build action, if any
            buildInfo.PostBuildAction?.Invoke(buildInfo, buildReport);

            return buildReport;
        }

        /// <summary>
        /// Validates the Unity Project assets by forcing a symbolic link sync and creates solution files.
        /// </summary>
        [UsedImplicitly]
        public static void ValidateProject()
        {
            try
            {
                SymbolicLinker.RunSync(true);
                SyncSolution();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorApplication.Exit(1);
            }

            EditorApplication.Exit(0);
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

            BuildReport buildReport = default;

            try
            {
                SyncSolution();

                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    var androidSdkPath = EditorPrefs.GetString("AndroidSdkRoot", "C:\\Program Files (x86)\\Android\\android-sdk");
                    Debug.Log($"AndroidSdkRoot: {androidSdkPath}");
                }

                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.WSAPlayer:
                        buildReport = await UwpPlayerBuildTools.BuildPlayer(new UwpBuildInfo(true));
                        break;
                    default:
                        var buildInfo = new BuildInfo(true) as IBuildInfo;
                        ParseBuildCommandLine(ref buildInfo);
                        buildReport = BuildUnityPlayer(buildInfo);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Build Failed!\n{e.Message}\n{e.StackTrace}");
            }

            if (buildReport == null)
            {
                Debug.LogError("Failed to find a valid build report!");
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log($"Exiting command line build...\nBuild success? {buildReport.summary.result}\nBuild time: {buildReport.summary.totalTime:g}");
                EditorApplication.Exit(buildReport.summary.result == BuildResult.Succeeded ? 0 : 1);
            }
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
                    case "-version":
                        if (Version.TryParse(arguments[++i], out var version))
                        {
                            buildInfo.Version = version;
                        }
                        else
                        {
                            Debug.LogError($"Failed to parse -version \"{arguments[i]}\"");
                        }
                        break;
                    case "-versionCode":
                        if (int.TryParse(arguments[++i], out var versionCode))
                        {
                            buildInfo.VersionCode = versionCode;
                        }
                        else
                        {
                            Debug.LogError($"Failed to parse -versionCode \"{arguments[i]}\"");
                        }
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
                    case "-ARM":
                    case "-ARM64":
                        buildInfo.BuildPlatform = arguments[i].Substring(1);
                        break;
                    case "-debug":
                    case "-master":
                    case "-release":
                        buildInfo.Configuration = arguments[i].Substring(1).ToLower();
                        break;
                    case "-bundleIdentifier":
                        buildInfo.BundleIdentifier = arguments[++i];
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
