// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline
{
    /// <summary>
    /// The Build Info defines common properties for a build.
    /// </summary>
    public interface IBuildInfo : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>
        /// The Name of the executable. Same as <see cref="Application.productName"/> but cached for threading safety.
        /// </summary>
        string BuildName { get; }

        /// <summary>
        /// The <see cref="Version"/> of the executable.
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// Is this build being issued from the command line?
        /// </summary>
        bool IsCommandLine { get; }

        /// <summary>
        /// The directory to put the final build output relative to the project root.
        /// </summary>
        /// <remarks>
        /// Defaults to "<see cref="Application.dataPath"/>/Builds/Platform Target/"
        /// </remarks>
        string OutputDirectory { get; set; }

        /// <summary>
        /// The directory containing the full path to the <see cref="OutputDirectory"/>.
        /// </summary>
        string AbsoluteOutputDirectory { get; }

        /// <summary>
        /// The executable file extension for this build.
        /// </summary>
        string ExecutableFileExtension { get; }

        /// <summary>
        /// The list of scenes to include in the build.
        /// </summary>
        IEnumerable<EditorBuildSettingsScene> Scenes { get; set; }

        /// <summary>
        /// Build options to include in the Unity player build pipeline.
        /// </summary>
        BuildOptions BuildOptions { get; set; }

        /// <summary>
        /// The build target.
        /// </summary>
        BuildTarget BuildTarget { get; }

        /// <summary>
        /// The <see cref="XRTK.Interfaces.IMixedRealityPlatform"/> to build to.
        /// </summary>
        XRTK.Interfaces.IMixedRealityPlatform BuildPlatform { get; }

        /// <summary>
        /// Optional parameter to set the player's <see cref="ColorSpace"/>
        /// </summary>
        ColorSpace? ColorSpace { get; set; }

        /// <summary>
        /// The symbols associated with this build.
        /// </summary>
        string BuildSymbols { get; set; }

        /// <summary>
        /// The build configuration (i.e. debug, release, or master)
        /// </summary>
        string Configuration { get; set; }

        /// <summary>
        /// The build platform architecture (i.e. x86, x64, ARM, ARM64)
        /// </summary>
        string Architecture { get; set; }

        void ParseCommandLineArgs();

        bool Install { get; set; }
    }
}
