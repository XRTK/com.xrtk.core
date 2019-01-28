// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace XRTK.Inspectors.Utilities
{
    [InitializeOnLoad]
    public static class AutoSceneSwitcher
    {
        static AutoSceneSwitcher()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            OnPlayModeStateChanged();
        }

        private static void OnPlayModeStateChanged()
        {
            if (EditorApplication.isPlaying || !EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            var startScene = MixedRealityPreferences.StartSceneAsset;

            if (startScene == null || SceneManager.GetActiveScene().name == startScene.name) { return; }

            int dialogResult = EditorUtility.DisplayDialogComplex(
                    "Wrong scene!",
                    $"Would you like to open {startScene.name} scene before playing?\n" +
                    $"You can update the starting scene in:\n" +
                    $"Editor/Preferences... Tools",
                    "No, play anyway",
                    "Change scene, then play",
                    "Cancel");

            switch (dialogResult)
            {
                case 0:
                    //do nothing
                    break;
                case 1:
                    EditorApplication.isPlaying = false;
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(startScene));
                        EditorApplication.isPlaying = true;
                    }
                    break;
                case 2:
                    EditorApplication.isPlaying = false;
                    break;
            }
        }
    }
}