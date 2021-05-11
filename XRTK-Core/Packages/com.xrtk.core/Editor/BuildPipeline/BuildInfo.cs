// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
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
    [CustomEditor(typeof(BuildInfo), true)]
    public class BuildInfoInspector : UnityEditor.Editor
    {
        private SerializedProperty autoIncrement;
        private SerializedProperty bundleIdentifier;
        private SerializedProperty install;

        private BuildInfo buildInfo;

        protected void OnEnable()
        {
            autoIncrement = serializedObject.FindProperty(nameof(autoIncrement));
            bundleIdentifier = serializedObject.FindProperty(nameof(bundleIdentifier));
            install = serializedObject.FindProperty(nameof(install));

            buildInfo = (BuildInfo)target;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(bundleIdentifier);

            if (EditorGUI.EndChangeCheck())
            {
                var buildTargetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(buildInfo.BuildTarget);
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, bundleIdentifier.stringValue);
            }

            EditorGUILayout.PropertyField(autoIncrement);
            EditorGUILayout.PropertyField(install);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [RuntimePlatform(typeof(WebGlPlatform))]
    [RuntimePlatform(typeof(WindowsStandalonePlatform))]
    public class BuildInfo : ScriptableObject, IBuildInfo
    {
        [SerializeField]
        private bool autoIncrement;

        /// <inheritdoc />
        public bool AutoIncrement
        {
            get => autoIncrement;
            set => autoIncrement = value;
        }

        [SerializeField]
        [Tooltip("The bundle or application identifier\n(i.e. 'com.xrtk.core')")]
        private string bundleIdentifier;

        /// <inheritdoc />
        public string BundleIdentifier
        {
            get
            {
                if (string.IsNullOrWhiteSpace(bundleIdentifier))
                {
                    bundleIdentifier = PlayerSettings.applicationIdentifier;
                }

                return bundleIdentifier;
            }
            set
            {
                bundleIdentifier = value;
                PlayerSettings.applicationIdentifier = bundleIdentifier;
            }
        }

        /// <inheritdoc />
        public virtual Version Version { get; set; }

        /// <inheritdoc />
        public int? VersionCode { get; set; }

        /// <inheritdoc />
        public virtual BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;

        /// <inheritdoc />
        public virtual IMixedRealityPlatform BuildPlatform => MixedRealityPreferences.CurrentPlatformTarget;

        /// <inheritdoc />
        public bool IsCommandLine => Application.isBatchMode;

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

        private List<EditorBuildSettingsScene> scenes;

        /// <inheritdoc />
        public IEnumerable<EditorBuildSettingsScene> Scenes
        {
            get
            {
                if (scenes == null || !scenes.Any())
                {
                    scenes = EditorBuildSettings.scenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)).Where(scene => scene.enabled).ToList();
                }

                return scenes;
            }
            set => scenes = value.ToList();
        }

        /// <inheritdoc />
        public BuildOptions BuildOptions { get; set; }

        /// <inheritdoc />
        public ColorSpace? ColorSpace { get; set; }

        /// <inheritdoc />
        public string BuildSymbols { get; set; } = string.Empty;

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
                    case "-versionName":
                        if (Version.TryParse(arguments[++i], out var version))
                        {
                            Version = version;
                        }
                        else
                        {
                            Debug.LogError($"Failed to parse -versionName \"{arguments[i]}\"");
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
                    case "-buildOutputDirectory":
                        OutputDirectory = arguments[++i];
                        break;
                    case "-colorSpace":
                        ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), arguments[++i]);
                        break;
                    case "-buildArchitecture":
                        var architecture = arguments[++i].Substring(1);

                        switch (architecture)
                        {
                            case "x86":
                            case "x64":
                            case "ARM":
                            case "ARM64":
                                Architecture = architecture;
                                break;
                            default:
                                Debug.LogError($"Failed to parse -buildArchitecture: {architecture}");
                                break;
                        }

                        break;
                    case "-buildConfiguration":
                        var configuration = arguments[++i].Substring(1).ToLower();

                        switch (configuration)
                        {
                            case "debug":
                            case "master":
                            case "release":
                                Configuration = configuration;
                                break;
                            default:
                                Debug.LogError($"Failed to parse -buildConfiguration: {configuration}");
                                break;
                        }

                        break;
                }
            }
        }

        [SerializeField]
        [Tooltip("Should the executable be installed OnPostProcessBuild?")]
        private bool install;

        /// <inheritdoc />
        public virtual bool Install
        {
            get => install;
            set => install = value;
        }

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
                // Do a thing.
            }
        }

        /// <inheritdoc />
        public virtual void OnPostProcessBuild(BuildReport report)
        {
            if (MixedRealityToolkit.ActivePlatforms.Contains(BuildPlatform))
            {
                // Do a thing.
            }
        }
    }
}
