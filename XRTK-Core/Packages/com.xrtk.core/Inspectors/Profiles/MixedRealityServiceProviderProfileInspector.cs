// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(BaseMixedRealityServiceProfile<>))]
    public class MixedRealityServiceProfileInspector : BaseMixedRealityProfileInspector
    {
        private readonly GUIContent ProfileContent = new GUIContent("Profile", "The configuration profile for this service.");
        private ReorderableList configurationList;
        private int currentlySelectedConfigurationOption;

        private SerializedProperty configurations;

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
            ServiceConstraint = genericTypeArgs?[0];
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
                EditorGUILayout.HelpBox($"Register a new Configuration", MessageType.Warning);
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
            var prevLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = prevLabelWidth - 18f;
            EditorGUIUtility.wideMode = true;

            var isScrollBarActive = (int)(EditorGUIUtility.currentViewWidth - (rect.width + 25f)) == 36;

            var halfFieldWidth = rect.width * 0.5f;
            var halfFieldHeight = EditorGUIUtility.singleLineHeight * 0.25f;

            var nameRect = new Rect(rect.x, rect.y + halfFieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(rect.x, rect.y + halfFieldHeight * 6, rect.width, EditorGUIUtility.singleLineHeight);
            var runtimeRect = new Rect(rect.x, rect.y + halfFieldHeight * 11, rect.width, EditorGUIUtility.singleLineHeight);
            var profileRect = new Rect(rect.x, rect.y + halfFieldHeight * 16, rect.width, EditorGUIUtility.singleLineHeight);

            var profileHeight = rect.y + halfFieldHeight * 16;
            var profilePosition = rect.x + EditorGUIUtility.labelWidth;
            var profileLabelRect = new Rect(rect.x, profileHeight, halfFieldWidth, EditorGUIUtility.singleLineHeight);

            var configurationProperty = configurations.GetArrayElementAtIndex(index);

            var nameProperty = configurationProperty.FindPropertyRelative("name");
            var priorityProperty = configurationProperty.FindPropertyRelative("priority");
            var instanceTypeProperty = configurationProperty.FindPropertyRelative("instancedType");
            var runtimePlatformProperty = configurationProperty.FindPropertyRelative("runtimePlatform");
            var configurationProfileProperty = configurationProperty.FindPropertyRelative("configurationProfile");

            var configurationProfile = configurationProfileProperty.objectReferenceValue as BaseMixedRealityProfile;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(nameRect, nameProperty);
            TypeReferencePropertyDrawer.FilterConstraintOverride = IsConstraintSatisfied;
            EditorGUI.PropertyField(typeRect, instanceTypeProperty);
            var systemTypeReference = new SystemType(instanceTypeProperty.FindPropertyRelative("reference").stringValue);

            Type profileType = null;

            if (systemTypeReference.Type != null)
            {
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

            priorityProperty.intValue = index;
            EditorGUI.PropertyField(runtimeRect, runtimePlatformProperty);

            var update = false;

            if (profileType != null)
            {
                EditorGUI.LabelField(profileLabelRect, ProfileContent);
                var isNullProfile = configurationProfileProperty.objectReferenceValue == null;

                var buttonWidth = isNullProfile ? 20f : 42f;
                var profileObjectWidth = EditorGUIUtility.currentViewWidth - profilePosition - buttonWidth - 12f;
                var scrollOffset = isScrollBarActive ? 15f : 0f;
                var profileObjectRect = new Rect(profilePosition, profileHeight, profileObjectWidth - scrollOffset, EditorGUIUtility.singleLineHeight);
                var buttonRect = new Rect(profilePosition + profileObjectWidth - scrollOffset, profileHeight, buttonWidth, EditorGUIUtility.singleLineHeight);

                var newProfileObjectReference = EditorGUI.ObjectField(profileObjectRect, configurationProfileProperty.objectReferenceValue, profileType, false);

                if (newProfileObjectReference is BaseMixedRealityProfile newProfile)
                {
                    var newProfileType = newProfile.GetType();
                    if (newProfileType == profileType ||
                        newProfileType.IsSubclassOf(profileType))
                    {
                        configurationProfileProperty.objectReferenceValue = newProfileObjectReference;
                    }
                    else
                    {
                        Debug.LogError($"{newProfileObjectReference.name} does not derive from {profileType.Name}!");
                    }
                }
                else if (newProfileObjectReference is null)
                {
                    configurationProfileProperty.objectReferenceValue = null;
                }

                update = GUI.Button(buttonRect, isNullProfile ? NewProfileContent : CloneProfileContent);

                if (update)
                {
                    if (isNullProfile)
                    {
                        CreateNewProfileInstance(ThisProfile, configurationProfileProperty, profileType);
                    }
                    else
                    {
                        CloneProfileInstance(ThisProfile, configurationProfileProperty, configurationProfile);
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(profileRect, "No Configuration Profile needed");
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

            if (update ||
                EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (MixedRealityToolkit.IsInitialized &&
                    !string.IsNullOrEmpty(instanceTypeProperty.FindPropertyRelative("reference").stringValue))
                {
                    MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
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
