// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Inspectors.Utilities;
using XRTK.Definitions.Controllers;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityControllerDataProvidersProfile))]
    public class MixedRealityControllerDataProvidersProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent AddControllerDataProviderContent = new GUIContent("+ Add a New Controller Data Provider");
        private static readonly GUIContent RemoveControllerDataProviderContent = new GUIContent("-", "Remove Controller Data Provider");
        private static readonly GUIContent ProfileContent = new GUIContent("Profile");

        private SerializedProperty controllerDataProviders;

        private bool[] foldouts = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerDataProviders = serializedObject.FindProperty("registeredControllerDataProviders");
            foldouts = new bool[controllerDataProviders.arraySize];
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
            EditorGUILayout.LabelField("Controller Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the input sources your application can get input data from.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();

            bool changed = false;

            if (GUILayout.Button(AddControllerDataProviderContent, EditorStyles.miniButton))
            {
                controllerDataProviders.arraySize += 1;
                var newConfiguration = controllerDataProviders.GetArrayElementAtIndex(controllerDataProviders.arraySize - 1);
                var dataProviderType = newConfiguration.FindPropertyRelative("dataProviderType");
                var dataProviderName = newConfiguration.FindPropertyRelative("dataProviderName");
                var priority = newConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = newConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = newConfiguration.FindPropertyRelative("profile");

                serializedObject.ApplyModifiedProperties();
                dataProviderType.FindPropertyRelative("reference").stringValue = string.Empty;
                dataProviderName.stringValue = "New Controller Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                profile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[controllerDataProviders.arraySize];
                changed = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < controllerDataProviders.arraySize; i++)
            {
                var controllerConfiguration = controllerDataProviders.GetArrayElementAtIndex(i);
                var dataProviderName = controllerConfiguration.FindPropertyRelative("dataProviderName");
                var dataProviderType = controllerConfiguration.FindPropertyRelative("dataProviderType");
                var priority = controllerConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = controllerConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = controllerConfiguration.FindPropertyRelative("profile");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], dataProviderName.stringValue, true);

                if (GUILayout.Button(RemoveControllerDataProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[controllerDataProviders.arraySize];
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