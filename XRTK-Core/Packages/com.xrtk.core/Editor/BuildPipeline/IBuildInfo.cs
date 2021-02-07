// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.BuildPipeline
{
    /// <summary>
    /// The Build Info defines common properties for a build.
    /// </summary>
    public interface IBuildInfo
    {
        /// <summary>
        /// Is this build being issued from the command line?
        /// </summary>
        bool IsCommandLine { get; }

        /// <summary>
        /// The directory to put the final build output.
        /// </summary>
        /// <remarks>
        /// Defaults to "<see cref="Application.dataPath"/>/Builds/Platform Target/"
        /// </remarks>
        string OutputDirectory { get; set; }

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
        /// Optional parameter to set the player's <see cref="ColorSpace"/>
        /// </summary>
        ColorSpace? ColorSpace { get; set; }

        /// <summary>
        /// Should the build auto increment the build version number?
        /// </summary>
        bool AutoIncrement { get; set; }

        /// <summary>
        /// The symbols associated with this build.
        /// </summary>
        string BuildSymbols { get; set; }

        /// <summary>
        /// The build configuration (i.e. debug, release, or master)
        /// </summary>
        string Configuration { get; set; }

        /// <summary>
        /// The build platform (i.e. x86, x64)
        /// </summary>
        string BuildPlatform { get; set; }
    }
}