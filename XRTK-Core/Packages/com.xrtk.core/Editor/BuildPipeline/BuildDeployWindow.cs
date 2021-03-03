// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Platforms;
using XRTK.Extensions;
using XRTK.Services;

namespace XRTK.Editor.BuildPipeline
{
    public class BuildDeployWindow : EditorWindow
    {
        private int platformIndex;
        private string[] platforms;

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
            platforms = MixedRealityToolkit.AvailablePlatforms.Select(platform =>
            {
                switch (platform)
                {
                    case AllPlatforms _:
                    case EditorPlatform _:
                    case CurrentBuildTargetPlatform _:
                        return null;
                    default:
                        return platform.GetType().Name.Replace("Platform", string.Empty);
                }
            }).Where(s => s != null).ToArray();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            platformIndex = EditorGUILayout.Popup("Platform Target", platformIndex, platforms, GUILayout.Width(256));


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

            if (GUILayout.Button("Build Unity Project"))
            {
                EditorApplication.delayCall += BuildUnityProject;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion EditorWindow Events

        private void BuildUnityProject()
        {
        }
    }
}
