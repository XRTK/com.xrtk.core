// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityRegisteredServiceProvidersProfile))]
    public class MixedRealityRegisteredServiceProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty configurations;
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurations = serializedObject.FindProperty("configurations");

            configurationList = new ReorderableList(serializedObject, configurations, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 5.5f
            };

            configurationList.drawElementCallback += DrawConfigurationOptionElement;
            configurationList.onAddCallback += OnConfigurationOptionAdded;
            configurationList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Service Providers Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This profile defines any additional Services like systems, features, and managers to register with the Mixed Reality Toolkit.\n\n" +
                                    "Note: The order of the list determines the order these services get created.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();
            EditorGUILayout.Space();
            configurationList.DoLayoutList();
            EditorGUILayout.Space();

            if (configurations == null || configurations.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Register a new Service Provider.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedConfigurationOption = index;
            }

            var lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var halfFieldHeight = EditorGUIUtility.singleLineHeight * 0.25f;
            var nameRect = new Rect(rect.x, rect.y + halfFieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(rect.x, rect.y + halfFieldHeight * 6, rect.width, EditorGUIUtility.singleLineHeight);
            var runtimeRect = new Rect(rect.x, rect.y + halfFieldHeight * 11, rect.width, EditorGUIUtility.singleLineHeight);
            var profileRect = new Rect(rect.x, rect.y + halfFieldHeight * 16, rect.width, EditorGUIUtility.singleLineHeight);

            var managerConfig = configurations.GetArrayElementAtIndex(index);
            var componentName = managerConfig.FindPropertyRelative("componentName");
            var componentType = managerConfig.FindPropertyRelative("componentType");
            var priority = managerConfig.FindPropertyRelative("priority");
            var runtimePlatform = managerConfig.FindPropertyRelative("runtimePlatform");
            var configurationProfile = managerConfig.FindPropertyRelative("configurationProfile");

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(nameRect, componentName);
            EditorGUI.PropertyField(typeRect, componentType);
            priority.intValue = index;
            EditorGUI.PropertyField(runtimeRect, runtimePlatform);
            EditorGUI.PropertyField(profileRect, configurationProfile);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (MixedRealityToolkit.IsInitialized &&
                    !string.IsNullOrEmpty(componentType.FindPropertyRelative("reference").stringValue))
                {
                    MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }

            EditorGUIUtility.wideMode = lastMode;
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            configurations.arraySize += 1;
            var index = configurations.arraySize - 1;
            var managerConfig = configurations.GetArrayElementAtIndex(index);
            var componentName = managerConfig.FindPropertyRelative("componentName");
            componentName.stringValue = $"New Configuration {index}";
            var priority = managerConfig.FindPropertyRelative("priority");
            priority.intValue = index;
            var runtimePlatform = managerConfig.FindPropertyRelative("runtimePlatform");
            runtimePlatform.intValue = 0;
            var configurationProfile = managerConfig.FindPropertyRelative("configurationProfile");
            configurationProfile.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
            var componentType = ((MixedRealityRegisteredServiceProvidersProfile)serializedObject.targetObject).Configurations[index].ComponentType;
            componentType.Type = null;
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedConfigurationOption >= 0)
            {
                configurations.DeleteArrayElementAtIndex(currentlySelectedConfigurationOption);
            }

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}