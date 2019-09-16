// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XRTK.Definitions;
using XRTK.Extensions.EditorClassExtensions;
using XRTK.Inspectors.Profiles;
using XRTK.Services;

namespace XRTK.Inspectors
{
    [CustomEditor(typeof(MixedRealityToolkit))]
    public class MixedRealityToolkitInspector : BaseMixedRealityToolkitInspector
    {
        private SerializedProperty activeProfile;
        private int currentPickerWindow = -1;
        private bool checkChange;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            currentPickerWindow = -1;
            checkChange = activeProfile.objectReferenceValue == null;
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);
            var changed = EditorGUI.EndChangeCheck();
            var commandName = Event.current.commandName;
            var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>();

            if (activeProfile.objectReferenceValue == null)
            {
                if (currentPickerWindow == -1 && checkChange)
                {
                    if (allConfigProfiles.Length > 1)
                    {
                        EditorUtility.DisplayDialog("Attention!", "You must choose a profile for the Mixed Reality Toolkit.", "OK");
                        currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);
                        EditorGUIUtility.ShowObjectPicker<MixedRealityToolkitConfigurationProfile>(GetDefaultProfile(allConfigProfiles), false, string.Empty, currentPickerWindow);
                    }
                    else if (allConfigProfiles.Length == 1)
                    {
                        activeProfile.objectReferenceValue = allConfigProfiles[0];
                        changed = true;
                        Selection.activeObject = allConfigProfiles[0];
                        EditorGUIUtility.PingObject(allConfigProfiles[0]);
                    }

                    checkChange = false;
                }

                if (GUILayout.Button("Create new configuration"))
                {
                    var profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                    profile.CreateAsset("Assets/XRTK.Generated/CustomProfiles");
                    activeProfile.objectReferenceValue = profile;
                    Selection.activeObject = profile;
                    EditorGUIUtility.PingObject(profile);
                }
            }

            if (EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
            {
                switch (commandName)
                {
                    case "ObjectSelectorUpdated":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        changed = true;
                        break;
                    case "ObjectSelectorClosed":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        currentPickerWindow = -1;
                        changed = true;
                        Selection.activeObject = activeProfile.objectReferenceValue;
                        EditorGUIUtility.PingObject(activeProfile.objectReferenceValue);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                MixedRealityToolkit.Instance.ResetConfiguration((MixedRealityToolkitConfigurationProfile)activeProfile.objectReferenceValue);
            }
        }

        [MenuItem("Mixed Reality Toolkit/Configure...", true, 0)]
        private static bool CreateMixedRealityToolkitGameObjectValidation()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("Mixed Reality Toolkit/Configure...", false, 0)]
        public static void CreateMixedRealityToolkitGameObject()
        {
            var startScene = MixedRealityPreferences.StartSceneAsset;

            if (startScene != null)
            {
                if (!EditorUtility.DisplayDialog(
                    title: "Attention!",
                    message: $"It seems you've already set the projects Start Scene to {startScene.name}\n" +
                             "You only need to configure the toolkit once in your Start Scene.\n" +
                             "Would you like to replace your Start Scene with the current scene you're configuring?",
                    ok: "Yes",
                    cancel: "No"))
                {
                    return;
                }
            }

            Selection.activeObject = MixedRealityToolkit.Instance;
            Debug.Assert(MixedRealityToolkit.IsInitialized);
            var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
            Debug.Assert(playspace != null);

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning("You must save this scene and assign it to the Start Scene in the XRTK preferences for the Mixed Reality Toolkit to function correctly.");
            }

            EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Please ensure the open scene has been saved for the Mixed Reality Toolkit to function correctly.");

            Debug.Assert(!string.IsNullOrEmpty(SceneManager.GetActiveScene().path),
                "Configured Scene must be saved in order to set it as the Start Scene!\n" +
                "Please save your scene and set it as the Start Scene in the XRTK preferences.");
            MixedRealityPreferences.StartSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);
        }

        private static MixedRealityToolkitConfigurationProfile GetDefaultProfile(IEnumerable<MixedRealityToolkitConfigurationProfile> allProfiles)
        {
            return allProfiles.FirstOrDefault(profile => profile.name == "DefaultMixedRealityToolkitConfigurationProfile");
        }
    }
}
