// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Editor.Utilities;
using XRTK.Interfaces;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildInfo : IBuildInfo
    {
        public BuildInfo()
        {
            IsCommandLine = false;
            BuildSymbols = string.Empty;
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            Scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled);
        }

        public BuildInfo(bool isCommandLine)
        {
            IsCommandLine = isCommandLine;
            BuildSymbols = string.Empty;
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            Scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled);
        }

        /// <inheritdoc />
        public string Name { get; } = EditorPreferences.ApplicationProductName;

        /// <inheritdoc />
        public virtual Version Version { get; set; }

        /// <inheritdoc />
        public virtual BuildTarget BuildTarget { get; }

        /// <inheritdoc />
        public IMixedRealityPlatform BuildPlatform { get; } = null;

        /// <inheritdoc />
        public bool IsCommandLine { get; }

        private string outputDirectory;

        /// <inheritdoc />
        public string OutputDirectory
        {
            get => string.IsNullOrEmpty(outputDirectory)
                ? outputDirectory = BuildDeployPreferences.BuildDirectory
                : outputDirectory;
            set => outputDirectory = value;
        }

        public string AbsoluteOutputDirectory
        {
            get
            {
                string rootBuildDirectory = OutputDirectory;
                int dirCharIndex = rootBuildDirectory.IndexOf("/", StringComparison.Ordinal);

                if (dirCharIndex != -1)
                {
                    rootBuildDirectory = rootBuildDirectory.Substring(0, dirCharIndex);
                }

                return Path.GetFullPath(Path.Combine(Path.Combine(BuildDeployPreferences.ApplicationDataPath, ".."), rootBuildDirectory));
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
        public string Architecture { get; set; } = "x86";

        /// <inheritdoc />
        public virtual void ParseCommandLineArgs()
        {
            var arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
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

        #region IOrderedCallback

        /// <inheritdoc />
        public int callbackOrder => 0;

        /// <inheritdoc />
        public virtual void OnPostprocessBuild(BuildReport report)
        {
        }

        /// <inheritdoc />
        public virtual void OnPreprocessBuild(BuildReport report)
        {
        }

        #endregion IOrderedCallback
    }
}