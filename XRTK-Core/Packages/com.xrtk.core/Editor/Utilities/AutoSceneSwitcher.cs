// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using XRTK.Editor.Utilities.SymbolicLinks;
using XRTK.Services;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// Ensures that the <see cref="MixedRealityPreferences.StartSceneAsset"/> is always loaded
    /// so that the <see cref="MixedRealityToolkit"/> runs correctly in the editor.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoSceneSwitcher
    {
        static AutoSceneSwitcher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
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
                EditorPreferences.Set($"{nameof(AutoSceneSwitcher)}", true);
                return;
            }

            // We couldn't auto setup the XRTK start scene for the user. Thus, the currently
            // loaded scene(s) may not contain a proper XRTK service locator game object configuration.
            // Least we can do now is offer the user to auto configure the current scene, if none of the
            // loaded scenes already contains a XRTK configuration.
            if (!MixedRealityToolkit.IsInitialized && EditorPreferences.Get($"{nameof(AutoSceneSwitcher)}", true))
            {
                var dialogResult = EditorUtility.DisplayDialogComplex(
                        title: "Missing the MixedRealityToolkit",
                        message: "Would you like to configure the Mixed Reality Toolkit now?",
                        "No, play anyway",
                        "Configure, then play",
                        "Cancel");

                switch (dialogResult)
                {
                    case 0:
                        EditorPreferences.Set($"{nameof(AutoSceneSwitcher)}", false);
                        break;
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
