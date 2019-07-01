// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem.Simulation
{
    [CustomEditor(typeof(MixedRealitySimulationDataProvidersProfile))]
    public class MixedRealitySimulationDataProvidersProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent AddInputSimulationDataProviderContent = new GUIContent("+ Add a New Input Simulation Data Provider");
        private static readonly GUIContent RemoveInputSimulationDataProviderContent = new GUIContent("-", "Remove Input Simulation Data Provider");
        private static readonly GUIContent ProfileContent = new GUIContent("Profile");

        private SerializedProperty inputSimulationDataProviders;
        private bool[] foldouts = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputSimulationDataProviders = serializedObject.FindProperty("registeredInputSimulationDataProviders");
            foldouts = new bool[inputSimulationDataProviders.arraySize];
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input Simulation Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the input sources to be simulated.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();

            bool changed = false;

            if (GUILayout.Button(AddInputSimulationDataProviderContent, EditorStyles.miniButton))
            {
                inputSimulationDataProviders.arraySize += 1;
                var newConfiguration = inputSimulationDataProviders.GetArrayElementAtIndex(inputSimulationDataProviders.arraySize - 1);
                var dataProviderType = newConfiguration.FindPropertyRelative("dataProviderType");
                var dataProviderName = newConfiguration.FindPropertyRelative("dataProviderName");
                var priority = newConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = newConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = newConfiguration.FindPropertyRelative("profile");

                serializedObject.ApplyModifiedProperties();
                dataProviderType.FindPropertyRelative("reference").stringValue = string.Empty;
                dataProviderName.stringValue = "New Input Simulation Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                profile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[inputSimulationDataProviders.arraySize];
                changed = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < inputSimulationDataProviders.arraySize; i++)
            {
                var dataProviderConfiguration = inputSimulationDataProviders.GetArrayElementAtIndex(i);
                var dataProviderName = dataProviderConfiguration.FindPropertyRelative("dataProviderName");
                var dataProviderType = dataProviderConfiguration.FindPropertyRelative("dataProviderType");
                var priority = dataProviderConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = dataProviderConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = dataProviderConfiguration.FindPropertyRelative("profile");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], dataProviderName.stringValue, true);

                if (GUILayout.Button(RemoveInputSimulationDataProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    inputSimulationDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[inputSimulationDataProviders.arraySize];
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(dataProviderType);
                    EditorGUILayout.PropertyField(dataProviderName);
                    EditorGUILayout.PropertyField(priority);
                    EditorGUILayout.PropertyField(runtimePlatform);
                    RenderProfile(thisProfile, profile, ProfileContent, false);

                    if (EditorGUI.EndChangeCheck())
                    {
                        changed = true;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}