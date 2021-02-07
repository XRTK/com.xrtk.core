// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildInfo : IBuildInfo, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public BuildInfo(bool isCommandLine = false)
        {
            IsCommandLine = isCommandLine;
            BuildSymbols = string.Empty;
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            Scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled);
        }

        /// <inheritdoc />
        public virtual BuildTarget BuildTarget { get; }

        /// <inheritdoc />
        public bool IsCommandLine { get; }

        private string outputDirectory;

        /// <inheritdoc />
        public string OutputDirectory
        {
            get => string.IsNullOrEmpty(outputDirectory) ? outputDirectory = BuildDeployPreferences.BuildDirectory : outputDirectory;
            set => outputDirectory = value;
        }

        /// <inheritdoc />
        public IEnumerable<EditorBuildSettingsScene> Scenes { get; set; }

        /// <inheritdoc />
        public BuildOptions BuildOptions { get; set; }

        /// <inheritdoc />
        public ColorSpace? ColorSpace { get; set; }

        /// <inheritdoc />
        public bool AutoIncrement { get; set; } = false;

        /// <inheritdoc />
        public string BuildSymbols { get; set; }

        /// <inheritdoc />
        public string BuildPlatform { get; set; } = "x86";

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