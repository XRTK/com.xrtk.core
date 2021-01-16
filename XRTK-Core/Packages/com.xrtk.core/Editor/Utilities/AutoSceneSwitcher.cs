// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using XRTK.Services;
using XRTK.Extensions;
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

            // If a start scene hasn't been configured yet for XRTK but the project
            // has build scenes configured, we assume the scene at index 0 of the build scenes
            // is our start scene and save it to preferences, unless it's the Unity default sample scene.
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

            // If we have a start scene configured in the XRTK preferences
            // make sure it's loaded.
            if (startSceneAsset != null)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var loadedScene = SceneManager.GetSceneAt(i);
                    if (string.Equals(loadedScene.name, startSceneAsset.name))
                    {
                        startSceneLoaded = true;
                        break;
                    }
                }

                // A start scene was configured in preferences, but it's not loaded
                // into hierarchy yet, so we force load it for the user now.
                if (!startSceneLoaded && startSceneAsset != null)
                {
                    var startScene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(startSceneAsset), OpenSceneMode.Additive);

                    // Move the start scene to be first in hierarchy, if multiples scenes are loaded.
                    if (SceneManager.sceneCount > 1)
                    {
                        EditorSceneManager.MoveSceneBefore(startScene, SceneManager.GetSceneAt(1));
                    }

                    startSceneLoaded = true;
                }
            }

            // If the start scene was successfully loaded, we're good and can continue playing.
            if (startSceneLoaded)
            {
                return;
            }

            // We couldn't auto setup the XRTK start scene for the user. Thus, the currently
            // loaded scene(s) may not contain a proper XRTK service locator game object configuration.
            // Least we can do now is offer the user to auto configure the current scene, if none of the
            // loaded scenes already contains a XRTK configuration.
            if (MixedRealityToolkit.Instance.IsNull())
            {
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
                        break;
                    case 2:
                        EditorApplication.isPlaying = false;
                        break;
                }
            }
        }
    }
}
