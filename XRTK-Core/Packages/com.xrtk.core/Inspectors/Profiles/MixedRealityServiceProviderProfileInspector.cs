// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Services;
using XRTK.Extensions;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(BaseMixedRealityServiceProfile<>))]
    public class MixedRealityServiceProfileInspector : BaseMixedRealityProfileInspector
    {
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        private SerializedProperty configurations;

        protected Type ServiceConstraint { get; private set; } = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurations = serializedObject.FindProperty(nameof(configurations));

            Debug.Assert(configurations != null);
            var baseType = ThisProfile.GetType().BaseType;
            var genericTypeArgs = baseType?.FindTopmostGenericTypeArguments();

            Debug.Assert(genericTypeArgs != null);

            foreach (var interfaceType in genericTypeArgs)
            {
                ServiceConstraint = interfaceType;
                break;
            }

            Debug.Assert(ServiceConstraint != null);

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
            serializedObject.Update();
            configurationList.DoLayoutList();

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

            var configurationProperty = configurations.GetArrayElementAtIndex(index);

            var nameProperty = configurationProperty.FindPropertyRelative("name");
            var priorityProperty = configurationProperty.FindPropertyRelative("priority");
            var instanceTypeProperty = configurationProperty.FindPropertyRelative("instancedType");
            var runtimePlatformProperty = configurationProperty.FindPropertyRelative("runtimePlatform");
            var configurationProfileProperty = configurationProperty.FindPropertyRelative("configurationProfile");

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(nameRect, nameProperty);
            TypeReferencePropertyDrawer.FilterConstraintOverride = IsConstraintSatisfied;
            EditorGUI.PropertyField(typeRect, instanceTypeProperty);
            priorityProperty.intValue = index;
            EditorGUI.PropertyField(runtimeRect, runtimePlatformProperty);
            EditorGUI.PropertyField(profileRect, configurationProfileProperty);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (MixedRealityToolkit.IsInitialized &&
                    !string.IsNullOrEmpty(instanceTypeProperty.FindPropertyRelative("reference").stringValue))
                {
                    MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }

            EditorGUIUtility.wideMode = lastMode;
        }

        private bool IsConstraintSatisfied(Type type)
        {
            return !type.IsAbstract && type.GetInterfaces().Any(interfaceType => interfaceType == ServiceConstraint);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            configurations.arraySize += 1;
            var index = configurations.arraySize - 1;

            var configuration = configurations.GetArrayElementAtIndex(index);
            var nameProperty = configuration.FindPropertyRelative("name");
            var instancedTypeProperty = configuration.FindPropertyRelative("instancedType");
            var priorityProperty = configuration.FindPropertyRelative("priority");
            var runtimePlatformProperty = configuration.FindPropertyRelative("runtimePlatform");
            var configurationProfileProperty = configuration.FindPropertyRelative("configurationProfile");

            nameProperty.stringValue = $"New Configuration {index}";
            instancedTypeProperty.FindPropertyRelative("reference").stringValue = string.Empty;
            priorityProperty.intValue = index;
            runtimePlatformProperty.intValue = 0;
            configurationProfileProperty.objectReferenceValue = null;

            serializedObject.ApplyModifiedProperties();
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
