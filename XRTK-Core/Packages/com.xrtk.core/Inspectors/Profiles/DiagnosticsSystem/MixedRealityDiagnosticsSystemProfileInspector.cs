// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.DiagnosticsSystem
{
    [CustomEditor(typeof(MixedRealityDiagnosticsSystemProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent addDataProviderContent = new GUIContent("+ Add a new diagnostics data provider");
        private static readonly GUIContent removeDataProviderContent = new GUIContent("-", "Remove diagnostics data provider");
        private static readonly GUIContent profileContent = new GUIContent("Profile");

        private SerializedProperty diagnosticsWindowPrefab;
        private SerializedProperty showDiagnosticsWindowOnStart;
        private SerializedProperty registeredDiagnosticsDataProviders;
        private bool[] foldouts = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            diagnosticsWindowPrefab = serializedObject.FindProperty(nameof(diagnosticsWindowPrefab));
            showDiagnosticsWindowOnStart = serializedObject.FindProperty(nameof(showDiagnosticsWindowOnStart));
            registeredDiagnosticsDataProviders = serializedObject.FindProperty(nameof(registeredDiagnosticsDataProviders));
            foldouts = new bool[registeredDiagnosticsDataProviders.arraySize];
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diagnostic System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Diagnostic can help monitor system resources and performance inside an application during development.", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(diagnosticsWindowPrefab);
            EditorGUILayout.PropertyField(showDiagnosticsWindowOnStart);
            EditorGUILayout.Space();

            bool changed = false;
            if (GUILayout.Button(addDataProviderContent, EditorStyles.miniButton))
            {
                registeredDiagnosticsDataProviders.arraySize += 1;
                var newConfiguration = registeredDiagnosticsDataProviders.GetArrayElementAtIndex(registeredDiagnosticsDataProviders.arraySize - 1);
                var dataProviderType = newConfiguration.FindPropertyRelative("dataProviderType");
                var dataProviderName = newConfiguration.FindPropertyRelative("dataProviderName");
                var priority = newConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = newConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = newConfiguration.FindPropertyRelative("profile");

                serializedObject.ApplyModifiedProperties();
                dataProviderType.FindPropertyRelative("reference").stringValue = string.Empty;
                dataProviderName.stringValue = "New Diagnostics Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                profile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[registeredDiagnosticsDataProviders.arraySize];
                changed = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < registeredDiagnosticsDataProviders.arraySize; i++)
            {
                var controllerConfiguration = registeredDiagnosticsDataProviders.GetArrayElementAtIndex(i);
                var dataProviderName = controllerConfiguration.FindPropertyRelative("dataProviderName");
                var dataProviderType = controllerConfiguration.FindPropertyRelative("dataProviderType");
                var priority = controllerConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = controllerConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = controllerConfiguration.FindPropertyRelative("profile");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], dataProviderName.stringValue, true);

                if (GUILayout.Button(removeDataProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    registeredDiagnosticsDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[registeredDiagnosticsDataProviders.arraySize];
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
                    RenderProfile(thisProfile, profile, profileContent, false);

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
