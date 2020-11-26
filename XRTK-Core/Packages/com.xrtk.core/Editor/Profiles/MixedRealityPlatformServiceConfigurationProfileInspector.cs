// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Extensions;
using XRTK.Editor.PropertyDrawers;
using XRTK.Extensions;
using XRTK.Services;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityPlatformServiceConfigurationProfile))]
    public class MixedRealityPlatformServiceConfigurationProfileInspector : BaseMixedRealityProfileInspector
    {
        private readonly GUIContent profileContent = new GUIContent("Profile", "The settings profile for this service.");
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        private SerializedProperty configurations;
        private SerializedProperty platformEntries;

        private Type[] platforms = new Type[0];
        private List<Tuple<bool, bool>> configListHeightFlags;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurations = serializedObject.FindProperty(nameof(configurations));
            platformEntries = serializedObject.FindProperty(nameof(platformEntries));
            UpdatePlatformList();
            Debug.Assert(configurations != null);

            configurationList = new ReorderableList(serializedObject, configurations, true, false, true, true);
            configListHeightFlags = new List<Tuple<bool, bool>>(configurations.arraySize);

            for (int i = 0; i < configurations.arraySize; i++)
            {
                configListHeightFlags.Add(new Tuple<bool, bool>(true, false));
            }

            configurationList.drawElementCallback += DrawConfigurationOptionElement;
            configurationList.onAddCallback += OnConfigurationOptionAdded;
            configurationList.onRemoveCallback += OnConfigurationOptionRemoved;
            configurationList.elementHeightCallback += ElementHeightCallback;
        }

        private void UpdatePlatformList()
        {
            var runtimePlatforms = platformEntries.FindPropertyRelative("runtimePlatforms");

            if (runtimePlatforms.arraySize > 0)
            {
                if (platforms.Length != runtimePlatforms.arraySize)
                {
                    platforms = new Type[runtimePlatforms.arraySize];
                }

                for (int i = 0; i < runtimePlatforms.arraySize; i++)
                {
                    platforms[i] = new SystemType(runtimePlatforms.GetArrayElementAtIndex(i).FindPropertyRelative("reference").stringValue);
                }
            }
            else
            {
                platforms = new Type[0];
            }
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Use this configuration profile to setup all of the services you would like to add to any existing profile configurations when the target platform package is installed");
            EditorGUILayout.Space();

            if (GUILayout.Button("Install Platform Service Configuration"))
            {
                EditorApplication.delayCall += () => PackageInstaller.InstallConfiguration(target as MixedRealityPlatformServiceConfigurationProfile, MixedRealityToolkit.Instance.ActiveProfile);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(platformEntries);

            if (EditorGUI.EndChangeCheck())
            {
                UpdatePlatformList();
            }

            EditorGUILayout.Space();
            configurations.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(configurations.isExpanded, new GUIContent("Configuration Options"));

            if (configurations.isExpanded)
            {
                EditorGUILayout.Space();
                configurationList.DoLayoutList();

                if (configurations == null || configurations.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("Register a new Service Configuration", MessageType.Warning);
                }

            }

            serializedObject.ApplyModifiedProperties();
        }

        private float ElementHeightCallback(int index)
        {
            if (configListHeightFlags.Count == 0)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var (isExpanded, hasProfile) = configListHeightFlags[index];
            var modifier = isExpanded
                ? hasProfile
                    ? 5.5f
                    : 4f
                : 1.5f;
            return EditorGUIUtility.singleLineHeight * modifier;
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedConfigurationOption = index;
            }

            serializedObject.Update();

            var configurationProperty = configurations.GetArrayElementAtIndex(index);

            var nameProperty = configurationProperty.FindPropertyRelative("name");
            var priorityProperty = configurationProperty.FindPropertyRelative("priority");
            var instanceTypeProperty = configurationProperty.FindPropertyRelative("instancedType");
            var platformEntriesProperty = configurationProperty.FindPropertyRelative("platformEntries");
            var globalRuntimePlatformProperty = platformEntries.FindPropertyRelative("runtimePlatforms");
            var runtimePlatformProperty = platformEntriesProperty.FindPropertyRelative("runtimePlatforms");
            var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");
            var systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);

            bool addPlatforms = false;

            if (runtimePlatformProperty.arraySize != globalRuntimePlatformProperty.arraySize)
            {
                addPlatforms = true;
                runtimePlatformProperty.ClearArray();
            }

            if (globalRuntimePlatformProperty.arraySize > 0)
            {
                for (int i = 0; i < globalRuntimePlatformProperty.arraySize; i++)
                {
                    if (addPlatforms)
                    {
                        runtimePlatformProperty.InsertArrayElementAtIndex(i);
                    }

                    var globalPlatform = globalRuntimePlatformProperty.GetArrayElementAtIndex(i).FindPropertyRelative("reference").stringValue;
                    runtimePlatformProperty.GetArrayElementAtIndex(i).FindPropertyRelative("reference").stringValue = globalPlatform;
                }
            }

            var hasProfile = false;

            Type profileType = null;

            if (systemTypeReference.Type != null)
            {
                if (nameProperty.stringValue.Contains("New Configuration"))
                {
                    nameProperty.stringValue = systemTypeReference.Type.Name.ToProperCase();
                }

                var constructors = systemTypeReference.Type.GetConstructors();

                foreach (var constructorInfo in constructors)
                {
                    var parameters = constructorInfo.GetParameters();

                    foreach (var parameterInfo in parameters)
                    {
                        if (parameterInfo.ParameterType.IsAbstract) { continue; }

                        if (parameterInfo.ParameterType.IsSubclassOf(typeof(BaseMixedRealityProfile)))
                        {
                            profileType = parameterInfo.ParameterType;
                            break;
                        }
                    }

                    if (profileType != null)
                    {
                        hasProfile = true;
                        break;
                    }
                }
            }

            priorityProperty.intValue = index;

            var lastMode = EditorGUIUtility.wideMode;
            var prevLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = prevLabelWidth - 18f;
            EditorGUIUtility.wideMode = true;

            var halfFieldHeight = EditorGUIUtility.singleLineHeight * 0.25f;

            var rectX = rect.x + 12;
            var rectWidth = rect.width - 12;
            var nameRect = new Rect(rectX, rect.y + halfFieldHeight, rectWidth, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(rectX, rect.y + halfFieldHeight * 6, rectWidth, EditorGUIUtility.singleLineHeight);
            var profileRect = new Rect(rectX, rect.y + halfFieldHeight * 11, rectWidth, EditorGUIUtility.singleLineHeight);
            var runtimeRect = new Rect(rectX, rect.y + halfFieldHeight * (hasProfile ? 16 : 11), rectWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            if (configurationProperty.isExpanded)
            {
                EditorGUI.PropertyField(nameRect, nameProperty);
                configurationProperty.isExpanded = EditorGUI.Foldout(nameRect, configurationProperty.isExpanded, GUIContent.none, true);

                if (!configurationProperty.isExpanded)
                {
                    GUI.FocusControl(null);
                }
            }
            else
            {
                configurationProperty.isExpanded = EditorGUI.Foldout(nameRect, configurationProperty.isExpanded, nameProperty.stringValue, true);
            }

            if (configurationProperty.isExpanded)
            {
                TypeReferencePropertyDrawer.FilterConstraintOverride = IsConstraintSatisfied;
                TypeReferencePropertyDrawer.GroupingOverride = TypeGrouping.NoneByNameNoNamespace;
                EditorGUI.PropertyField(typeRect, instanceTypeProperty);
                systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);

                GUI.enabled = false;

                EditorGUI.PropertyField(runtimeRect, platformEntriesProperty);
                GUI.enabled = true;

                if (hasProfile)
                {
                    MixedRealityProfilePropertyDrawer.ProfileTypeOverride = profileType;
                    EditorGUI.PropertyField(profileRect, configurationProfileProperty, profileContent);
                }
            }

            if (configurationProfileProperty.objectReferenceValue != null)
            {
                var renderedProfile = configurationProfileProperty.objectReferenceValue as BaseMixedRealityProfile;
                Debug.Assert(renderedProfile != null);

                if (renderedProfile.ParentProfile.IsNull() ||
                    renderedProfile.ParentProfile != ThisProfile)
                {
                    renderedProfile.ParentProfile = ThisProfile;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (MixedRealityToolkit.IsInitialized &&
                    runtimePlatformProperty.arraySize > 0 &&
                    systemTypeReference.Type != null)
                {
                    MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }

            EditorGUIUtility.wideMode = lastMode;
            EditorGUIUtility.labelWidth = prevLabelWidth;
            configListHeightFlags[index] = new Tuple<bool, bool>(configurationProperty.isExpanded, hasProfile);
        }

        private bool IsConstraintSatisfied(Type type)
        {
            var platformActive = false;

            foreach (var attribute in Attribute.GetCustomAttributes(type, typeof(RuntimePlatformAttribute)))
            {
                if (attribute is RuntimePlatformAttribute platformAttribute &&
                    platforms.Contains(platformAttribute.Platform))
                {
                    platformActive = true;
                }
            }

            return !type.IsAbstract && platformActive;
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            configurations.arraySize += 1;
            var index = configurations.arraySize - 1;

            var configuration = configurations.GetArrayElementAtIndex(index);
            configuration.isExpanded = true;
            var nameProperty = configuration.FindPropertyRelative("name");
            var priorityProperty = configuration.FindPropertyRelative("priority");
            var instancedTypeProperty = configuration.FindPropertyRelative("instancedType");
            var platformEntriesProperty = configuration.FindPropertyRelative("platformEntries");
            var configurationProfileProperty = configuration.FindPropertyRelative("profile");
            var runtimePlatformsProperty = platformEntriesProperty.FindPropertyRelative("runtimePlatforms");

            nameProperty.stringValue = $"New Configuration {index}";
            instancedTypeProperty.FindPropertyRelative("reference").stringValue = string.Empty;
            priorityProperty.intValue = index;
            runtimePlatformsProperty.ClearArray();
            configurationProfileProperty.objectReferenceValue = null;
            configListHeightFlags.Add(new Tuple<bool, bool>(true, false));
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
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}