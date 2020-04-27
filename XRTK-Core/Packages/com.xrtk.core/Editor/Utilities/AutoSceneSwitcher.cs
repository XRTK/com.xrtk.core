// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using XRTK.Editor.Utilities.SymbolicLinks;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// Ensures that the <see cref="MixedRealityPreferences.StartSceneAsset"/> is always loaded
    /// so that the <see cref="XRTK.Services.MixedRealityToolkit"/> runs correctly in the editor.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoSceneSwitcher
    {
        static AutoSceneSwitcher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (MixedRealityPreferences.StartSceneAsset == null &&
                EditorBuildSettings.scenes.Length > 0 &&
                !EditorBuildSettings.scenes[0].path.Contains("SampleScene"))
            {
                MixedRealityPreferences.StartSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (SymbolicLinker.IsSyncing ||
                EditorApplication.isPlaying ||
                !EditorApplication.isPlayingOrWillChangePlaymode ||
                PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                return;
            }

            var startSceneAsset = MixedRealityPreferences.StartSceneAsset;
            var startSceneLoaded = false;

            if (startSceneAsset != null)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);

                    if (scene.name == startSceneAsset.name &&
                        SceneManager.GetActiveScene().name != startSceneAsset.name)
                    {
                        SceneManager.SetActiveScene(scene);
                        startSceneLoaded = true;
                        break;
                    }
                }

                if (!startSceneLoaded && startSceneAsset != null)
                {
                    var startScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(startSceneAsset), OpenSceneMode.Additive);
                    SceneManager.SetActiveScene(startScene);
                }
            }

            if (startSceneAsset != null && SceneManager.GetActiveScene().name == startSceneAsset.name) { return; }

            var dialogResult = EditorUtility.DisplayDialogComplex(
                    title: "Missing the MixedRealityToolkit",
                    message: "Would you like to configure the Mixed Reality Toolkit now?",
                    "No, play anyway",
                    "Configure, then play",
                    "Cancel");

            switch (dialogResult)
            {
                case 1:
                    MixedRealityToolkitInspector.CreateMixedRealityToolkitGameObject();
                    EditorApplication.isPlaying = true;
                    break;
                case 2:
                    EditorApplication.isPlaying = false;
                    break;
            }
        }
    }
}