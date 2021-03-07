// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces;
using XRTK.Services;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildDeployWindow : EditorWindow
    {
        private int platformIndex = -1;
        private readonly List<Tuple<IMixedRealityPlatform, string>> platforms = new List<Tuple<IMixedRealityPlatform, string>>();
        private bool isBuilding;

        private List<Tuple<IMixedRealityPlatform, string>> Platforms
        {
            get
            {
                if (platforms.Count == 0)
                {
                    foreach (var availablePlatform in MixedRealityToolkit.AvailablePlatforms)
                    {
                        if (availablePlatform is AllPlatforms ||
                            availablePlatform is EditorPlatform ||
                            availablePlatform is CurrentBuildTargetPlatform)
                        {
                            continue;
                        }

                        platforms.Add(new Tuple<IMixedRealityPlatform, string>(availablePlatform, availablePlatform.GetType().Name.Replace("Platform", string.Empty)));
                    }

                    for (var i = 0; i < platforms.Count; i++)
                    {
                        var (platform, platformName) = platforms[i];

                        if (MixedRealityPreferences.CurrentPlatformTarget == platform)
                        {
                            platformIndex = i;
                            break;
                        }
                    }
                }

                return platforms;
            }
        }

        [MenuItem("Mixed Reality Toolkit/Build Window", false, 99)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<BuildDeployWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Build Window");
            window.Show();
        }

        #region EditorWindow Events

        private void OnEnable()
        {
            titleContent = new GUIContent("Build Window");
            minSize = new Vector2(512, 256);
        }

        private void OnFocus()
        {
            platforms.Clear();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            for (var i = 0; i < Platforms.Count; i++)
            {
                var (platform, platformName) = Platforms[i];

                if (MixedRealityPreferences.CurrentPlatformTarget == platform)
                {
                    platformIndex = i;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();
            var prevPlatformIndex = platformIndex;
            platformIndex = EditorGUILayout.Popup("Platform Target", platformIndex, Platforms.Select(p => p.Item2).ToArray(), GUILayout.Width(256));

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Platforms.Count; i++)
                {
                    if (i == platformIndex)
                    {
                        var (platform, platformName) = Platforms[i];

                        MixedRealityPreferences.CurrentPlatformTarget = platform;

                        var buildTarget = platform.ValidBuildTargets[0]; // For now just get the highest priority one.

                        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(UnityEditor.BuildPipeline.GetBuildTargetGroup(buildTarget), buildTarget))
                        {
                            platformIndex = prevPlatformIndex;
                            Debug.LogWarning($"Failed to switch {platformName} active build target to {buildTarget}");
                        }
                    }
                }
            }

            if (GUILayout.Button("Open Player Settings"))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(PlayerSettings));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            var curBuildDirectory = BuildDeployPreferences.BuildDirectory;
            EditorGUILayout.LabelField("Build Directory", GUILayout.Width(96));
            var newBuildDirectory = EditorGUILayout.TextField(curBuildDirectory, GUILayout.Width(64), GUILayout.ExpandWidth(true));

            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPreferences.BuildDirectory = newBuildDirectory;
            }

            GUI.enabled = Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);

            if (GUILayout.Button("Open Build Directory"))
            {
                EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Unity Build Window"))
            {
                GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            GUI.enabled = !isBuilding && !Application.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (GUILayout.Button(new GUIContent("Build Unity Project", $"{(GUI.enabled ? "Build the unity project" : "Building disabled while project is updating...")}")))
            {
                EditorApplication.delayCall += BuildUnityProject;
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion EditorWindow Events

        private void BuildUnityProject()
        {
            if (UnityPlayerBuildTools.CheckBuildScenes() == false)
            {
                return;
            }

            Debug.Assert(!isBuilding);
            isBuilding = true;

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.WSAPlayer:
                    UnityPlayerBuildTools.BuildUnityPlayer(new UwpBuildInfo());
                    break;
                case BuildTarget.Lumin:
                    UnityPlayerBuildTools.BuildUnityPlayer(new LuminBuildInfo());
                    break;
                default:
                    UnityPlayerBuildTools.BuildUnityPlayer(new BuildInfo());
                    break;
            }

            isBuilding = false;
        }
    }
}
