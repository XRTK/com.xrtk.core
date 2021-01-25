// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Platforms;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Extensions;
using XRTK.Editor.PropertyDrawers;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(BaseMixedRealityServiceProfile<>), true, isFallback = true)]
    public class MixedRealityServiceProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly Type AllPlatformsType = typeof(AllPlatforms);
        private static readonly Guid AllPlatformsGuid = AllPlatformsType.GUID;
        private readonly GUIContent profileContent = new GUIContent("Profile", "The settings profile for this service.");
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        private SerializedProperty configurations; // Cannot be auto property bc field is serialized.

        protected SerializedProperty Configurations => configurations;

        /// <summary>
        /// Gets the service constraint used to filter options listed in the
        /// <see cref="configurations"/> instance type dropdown. Set after
        /// <see cref="OnEnable"/> was called to override.
        /// </summary>
        protected Type ServiceConstraint { get; set; } = null;

        private bool IsSystemConfiguration => typeof(IMixedRealitySystem).IsAssignableFrom(ServiceConstraint);

        private List<Tuple<bool, bool>> configListHeightFlags;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurations = serializedObject.FindProperty(nameof(configurations));

            Debug.Assert(configurations != null);

            var baseType = ThisProfile.GetType().BaseType;
            var genericTypeArgs = baseType?.FindTopmostGenericTypeArguments();
            Debug.Assert(genericTypeArgs != null);
            ServiceConstraint = genericTypeArgs[0];
            Debug.Assert(ServiceConstraint != null);

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

        public override void OnInspectorGUI()
        {
            RenderHeader();
            EditorGUILayout.Space();
            RenderConfigurationOptions();
        }

        protected void RenderConfigurationOptions()
        {
            configurations.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(configurations.isExpanded, new GUIContent($"{ServiceConstraint.Name} Configuration Options"));

            if (configurations.isExpanded)
            {
                serializedObject.Update();
                EditorGUILayout.Space();
                configurationList.DoLayoutList();

                if (configurations == null || configurations.arraySize == 0)
                {
                    EditorGUILayout.HelpBox($"Register a new {ServiceConstraint.Name} Configuration", MessageType.Warning);
                }

                serializedObject.ApplyModifiedProperties();
            }

            TypeReferencePropertyDrawer.CreateNewTypeOverride = null;
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
                    ? IsSystemConfiguration
                        ? 4f
                        : 5.5f
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
            var systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);
            var platformEntriesProperty = configurationProperty.FindPropertyRelative("platformEntries");
            var runtimePlatformProperty = platformEntriesProperty.FindPropertyRelative("runtimePlatforms");
            var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");

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
                TypeReferencePropertyDrawer.FilterConstraintOverride = type =>
                {
                    var isValid = !type.IsAbstract &&
                                  type.GetInterfaces().Any(interfaceType => interfaceType == ServiceConstraint);

                    return isValid && (!IsSystemConfiguration || MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceConfigurations.All(configuration => configuration.InstancedType.Type != type));
                };
                TypeReferencePropertyDrawer.CreateNewTypeOverride = ServiceConstraint;

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(typeRect, instanceTypeProperty);
                systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);

                if (EditorGUI.EndChangeCheck())
                {
                    if (systemTypeReference.Type == null)
                    {
                        nameProperty.stringValue = string.Empty;
                        configurationProfileProperty.objectReferenceValue = null;
                    }
                    else
                    {
                        nameProperty.stringValue = systemTypeReference.Type.Name.ToProperCase();

                        if (IsSystemConfiguration)
                        {
                            configurationProfileProperty.objectReferenceValue = null;
                        }
                    }
                }

                if (!IsSystemConfiguration)
                {
                    EditorGUI.PropertyField(runtimeRect, platformEntriesProperty);
                    runtimePlatformProperty = platformEntriesProperty.FindPropertyRelative("runtimePlatforms");
                }
                else
                {
                    runtimePlatformProperty = platformEntriesProperty.FindPropertyRelative("runtimePlatforms");
                    runtimePlatformProperty.arraySize = 1;
                    runtimePlatformProperty.GetArrayElementAtIndex(0).FindPropertyRelative("reference").stringValue = AllPlatformsGuid.ToString();
                }

                if (hasProfile)
                {
                    MixedRealityProfilePropertyDrawer.ProfileTypeOverride = profileType;
                    EditorGUI.PropertyField(profileRect, configurationProfileProperty, profileContent);
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

            configListHeightFlags.RemoveAt(0);

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}
