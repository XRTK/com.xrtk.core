// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
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

        [MenuItem("Mixed Reality Toolkit/Configure...", false, 0)]
        public static void CreateMixedRealityToolkitGameObject()
        {
            Selection.activeObject = MixedRealityToolkit.Instance;
            Debug.Assert(MixedRealityToolkit.IsInitialized);
            var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
            Debug.Assert(playspace != null);
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);
        }

        private static MixedRealityToolkitConfigurationProfile GetDefaultProfile(MixedRealityToolkitConfigurationProfile[] allProfiles)
        {
            for (var i = 0; i < allProfiles.Length; i++)
            {
                if (allProfiles[i].name == "DefaultMixedRealityToolkitConfigurationProfile")
                {
                    return allProfiles[i];
                }
            }

            return null;
        }
    }
}
