﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Inspectors.Extensions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(BaseMixedRealityServiceProfile<>))]
    public class MixedRealityServiceProfileInspector : BaseMixedRealityProfileInspector
    {
        private readonly GUIContent profileContent = new GUIContent("Profile", "The settings profile for this service.");
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        private SerializedProperty configurations;

        private bool showConfigurationFoldout = true;

        /// <summary>
        /// Gets the service constraint used to filter options listed in the
        /// <see cref="configurations"/> instance type dropdown. Set after
        /// <see cref="OnEnable"/> was called to override.
        /// </summary>
        protected Type ServiceConstraint { get; set; } = null;

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
            EditorGUILayout.Space();
            showConfigurationFoldout = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showConfigurationFoldout, new GUIContent($"{ServiceConstraint.Name} Configuration Options"), true);

            if (showConfigurationFoldout)
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
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedConfigurationOption = index;
            }

            var lastMode = EditorGUIUtility.wideMode;
            var prevLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = prevLabelWidth - 18f;
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
            var platformEntriesProperty = configurationProperty.FindPropertyRelative("platformEntries");
            var configurationProfileProperty = configurationProperty.FindPropertyRelative("profile");

            priorityProperty.intValue = index;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(nameRect, nameProperty);
            TypeReferencePropertyDrawer.FilterConstraintOverride = IsConstraintSatisfied;
            EditorGUI.PropertyField(typeRect, instanceTypeProperty);
            var systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);

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
                        break;
                    }
                }
            }

            EditorGUI.PropertyField(runtimeRect, platformEntriesProperty);

            if (profileType != null)
            {
                MixedRealityProfilePropertyDrawer.ProfileTypeOverride = profileType;
                EditorGUI.PropertyField(profileRect, configurationProfileProperty, profileContent);
            }

            if (configurationProfileProperty.objectReferenceValue != null)
            {
                var renderedProfile = configurationProfileProperty.objectReferenceValue as BaseMixedRealityProfile;
                Debug.Assert(renderedProfile != null);

                if (renderedProfile.ParentProfile == null ||
                    renderedProfile.ParentProfile != ThisProfile)
                {
                    renderedProfile.ParentProfile = ThisProfile;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (MixedRealityToolkit.IsInitialized &&
                    !string.IsNullOrEmpty(instanceTypeProperty.FindPropertyRelative("reference").stringValue))
                {
                    MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }

            EditorGUIUtility.wideMode = lastMode;
            EditorGUIUtility.labelWidth = prevLabelWidth;
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
