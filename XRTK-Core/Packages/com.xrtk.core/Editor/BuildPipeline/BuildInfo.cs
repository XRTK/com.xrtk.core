// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildInfo : ScriptableObject, IBuildInfo
    {
        protected virtual void Awake()
        {
            BundleIdentifier = Application.identifier;
            IsCommandLine = Application.isBatchMode;
            BuildSymbols = string.Empty;
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            Scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled);
        }

        /// <inheritdoc />
        public bool AutoIncrement { get; set; }

        /// <inheritdoc />
        public string BundleIdentifier { get; set; }

        /// <inheritdoc />
        public virtual Version Version { get; set; }

        /// <inheritdoc />
        public int? VersionCode { get; set; }

        /// <inheritdoc />
        public virtual BuildTarget BuildTarget { get; private set; }

        /// <inheritdoc />
        public virtual IMixedRealityPlatform BuildPlatform { get; internal set; }

        /// <inheritdoc />
        public bool IsCommandLine { get; private set; }

        private string outputDirectory;

        /// <inheritdoc />
        public virtual string OutputDirectory
        {
            get => string.IsNullOrEmpty(outputDirectory)
                ? outputDirectory = BuildDeployPreferences.BuildDirectory
                : outputDirectory;
            set => outputDirectory = value;
        }

        /// <inheritdoc />
        public virtual string AbsoluteOutputDirectory
        {
            get
            {
                var rootBuildDirectory = OutputDirectory;
                var dirCharIndex = rootBuildDirectory.IndexOf("/", StringComparison.Ordinal);

                if (dirCharIndex != -1)
                {
                    rootBuildDirectory = rootBuildDirectory.Substring(0, dirCharIndex);
                }

                return Path.GetFullPath(Path.Combine(Path.Combine(BuildDeployPreferences.ApplicationDataPath, ".."), rootBuildDirectory));
            }
        }

        /// <inheritdoc />
        public string FullOutputPath => $"{OutputDirectory}/{BundleIdentifier}{ExecutableFileExtension}";

        /// <inheritdoc />
        public virtual string ExecutableFileExtension
        {
            get
            {
                switch (BuildTarget)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        return ".exe";
                    default:
                        return "/";
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<EditorBuildSettingsScene> Scenes { get; set; }

        /// <inheritdoc />
        public BuildOptions BuildOptions { get; set; }

        /// <inheritdoc />
        public ColorSpace? ColorSpace { get; set; }

        /// <inheritdoc />
        public string BuildSymbols { get; set; }

        /// <inheritdoc />
        public string Architecture { get; set; }

        /// <inheritdoc />
        public virtual void ParseCommandLineArgs()
        {
            var arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-autoIncrement":
                        AutoIncrement = true;
                        break;
                    case "-version":
                        if (Version.TryParse(arguments[++i], out var version))
                        {
                            Version = version;
                        }
                        else
                        {
                            Debug.LogError($"Failed to parse -version \"{arguments[i]}\"");
                        }
                        break;
                    case "-versionCode":
                        if (int.TryParse(arguments[++i], out var versionCode))
                        {
                            VersionCode = versionCode;
                        }
                        else
                        {
                            Debug.LogError($"Failed to parse -versionCode \"{arguments[i]}\"");
                        }
                        break;
                    case "-bundleIdentifier":
                        BundleIdentifier = arguments[++i];
                        break;
                    case "-sceneList":
                        Scenes = Scenes.Union(UnityPlayerBuildTools.SplitSceneList(arguments[++i]));
                        break;
                    case "-sceneListFile":
                        Scenes = Scenes.Union(UnityPlayerBuildTools.SplitSceneList(File.ReadAllText(arguments[++i])));
                        break;
                    case "-buildOutput":
                        OutputDirectory = arguments[++i];
                        break;
                    case "-colorSpace":
                        ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), arguments[++i]);
                        break;
                    case "-x86":
                    case "-x64":
                    case "-ARM":
                    case "-ARM64":
                        Architecture = arguments[i].Substring(1);
                        break;
                    case "-debug":
                    case "-master":
                    case "-release":
                        Configuration = arguments[i].Substring(1).ToLower();
                        break;
                }
            }
        }

        /// <inheritdoc />
        public virtual bool Install { get; set; }

        /// <inheritdoc />
        public string Configuration
        {
            get
            {
                if (!this.HasConfigurationSymbol())
                {
                    return UnityPlayerBuildTools.BuildSymbolMaster;
                }

                return this.HasAnySymbols(UnityPlayerBuildTools.BuildSymbolDebug)
                    ? UnityPlayerBuildTools.BuildSymbolDebug
                    : this.HasAnySymbols(UnityPlayerBuildTools.BuildSymbolRelease)
                        ? UnityPlayerBuildTools.BuildSymbolRelease
                        : UnityPlayerBuildTools.BuildSymbolMaster;
            }
            set
            {
                if (this.HasConfigurationSymbol())
                {
                    this.RemoveSymbols(new[]
                    {
                        UnityPlayerBuildTools.BuildSymbolDebug,
                        UnityPlayerBuildTools.BuildSymbolRelease,
                        UnityPlayerBuildTools.BuildSymbolMaster
                    });
                }

                this.AppendSymbols(value);
            }
        }

        /// <inheritdoc />
        public virtual void OnPreProcessBuild(BuildReport report)
        {
            if (MixedRealityToolkit.ActivePlatforms.Contains(BuildPlatform))
            {
                Debug.Log($"{nameof(BuildInfo)}.{nameof(OnPreProcessBuild)}");
            }
        }

        /// <inheritdoc />
        public virtual void OnPostProcessBuild(BuildReport report)
        {
            if (MixedRealityToolkit.ActivePlatforms.Contains(BuildPlatform))
            {
                Debug.Log($"{nameof(BuildInfo)}.{nameof(OnPostProcessBuild)}");
            }
        }
    }
}
