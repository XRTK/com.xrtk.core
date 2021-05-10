// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using XRTK.Definitions.Platforms;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Services;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildDeployWindow : EditorWindow
    {
        private bool isBuilding;
        private int platformIndex = -1;

        private readonly List<IMixedRealityPlatform> platforms = new List<IMixedRealityPlatform>();

        private List<IMixedRealityPlatform> Platforms
        {
            get
            {
                if (platforms.Count == 0)
                {
                    for (int i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                    {
                        var availablePlatform = MixedRealityToolkit.AvailablePlatforms[i];

                        if (availablePlatform is AllPlatforms ||
                            availablePlatform is EditorPlatform ||
                            availablePlatform is CurrentBuildTargetPlatform)
                        {
                            continue;
                        }

                        platforms.Add(availablePlatform);
                    }

                    for (var i = 0; i < platforms.Count; i++)
                    {
                        if (MixedRealityPreferences.CurrentPlatformTarget == platforms[i])
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
            Assert.IsNotNull(UnityPlayerBuildTools.BuildInfo);
        }

        private void OnFocus()
        {
            platforms.Clear();
        }

        private void OnGUI()
        {
            for (var i = 0; i < Platforms.Count; i++)
            {
                if (MixedRealityPreferences.CurrentPlatformTarget == Platforms[i])
                {
                    platformIndex = i;
                    break;
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("The Mixed Reality Toolkit", MixedRealityInspectorUtility.BoldCenteredHeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"{MixedRealityPreferences.CurrentPlatformTarget.Name} Build Window", MixedRealityInspectorUtility.BoldCenteredHeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var prevPlatformIndex = platformIndex;
            var prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 96;
            platformIndex = EditorGUILayout.Popup("Platform Target", platformIndex, Platforms.Select(p => p.Name).ToArray(), GUILayout.Width(192));
            EditorGUIUtility.labelWidth = prevWidth;

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Platforms.Count; i++)
                {
                    if (i == platformIndex)
                    {
                        var platform = Platforms[i];

                        MixedRealityPreferences.CurrentPlatformTarget = platform;

                        var buildTarget = platform.ValidBuildTargets[0]; // For now just get the highest priority one.

                        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(UnityEditor.BuildPipeline.GetBuildTargetGroup(buildTarget), buildTarget))
                        {
                            platformIndex = prevPlatformIndex;
                            Debug.LogWarning($"Failed to switch {platform.Name} active build target to {buildTarget}");
                        }
                    }
                }
            }

            if (GUILayout.Button("Open Player Settings", GUILayout.Width(128), GUILayout.ExpandWidth(true)))
            {
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(PlayerSettings));
            }

            if (GUILayout.Button("Open Unity Build Window", GUILayout.Width(176), GUILayout.ExpandWidth(true)))
            {
                GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            var curBuildDirectory = BuildDeployPreferences.BuildDirectory;
            EditorGUILayout.LabelField("Build Directory", GUILayout.Width(96));
            var newBuildDirectory = EditorGUILayout.TextField(curBuildDirectory, GUILayout.Width(224), GUILayout.ExpandWidth(true));

            if (newBuildDirectory != curBuildDirectory)
            {
                BuildDeployPreferences.BuildDirectory = newBuildDirectory;
            }

            GUI.enabled = Directory.Exists(BuildDeployPreferences.AbsoluteBuildDirectory);

            if (GUILayout.Button("Open Build Directory", GUILayout.Width(176), GUILayout.ExpandWidth(true)))
            {
                EditorApplication.delayCall += () => Process.Start(BuildDeployPreferences.AbsoluteBuildDirectory);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (UnityPlayerBuildTools.BuildInfo is ScriptableObject buildObject)
            {
                var editor = UnityEditor.Editor.CreateEditor(buildObject);

                if (!editor.IsNull())
                {
                    editor.OnInspectorGUI();
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = !isBuilding && !Application.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (GUILayout.Button(new GUIContent("Build Unity Project", $"{(GUI.enabled ? "Build the unity project" : "Building disabled while project is updating...")}")))
            {
                EditorApplication.delayCall += BuildUnityProject;
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        #endregion EditorWindow Events

        private void BuildUnityProject()
        {
            if (UnityPlayerBuildTools.CheckBuildScenes() == false)
            {
                return;
            }

            Debug.Assert(!isBuilding, "Build already in progress!");
            isBuilding = true;

            UnityPlayerBuildTools.BuildUnityPlayer();

            isBuilding = false;
        }
    }
}
