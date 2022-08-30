// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Extensions;
using XRTK.Editor.PropertyDrawers;
using XRTK.Providers.Controllers;

namespace XRTK.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseMixedRealityControllerDataProviderProfile), editorForChildClasses: true, isFallback = true)]
    public class BaseMixedRealityControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly List<int> nullElementIndexes = new List<int>();

        private SerializedProperty hasSetupDefaults;
        private SerializedProperty controllerProfiles;

        private BaseMixedRealityControllerDataProviderProfile dataProviderProfile;

        private ReorderableList mappingProfileList;

        private int currentlySelectedElement;

        private GUIStyle controllerButtonStyle;

        private GUIStyle ControllerButtonStyle => controllerButtonStyle ?? (controllerButtonStyle = new GUIStyle("button")
        {
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold,
            stretchHeight = true,
            stretchWidth = true,
            wordWrap = true,
            fontSize = 10,
        });

        protected override void OnEnable()
        {
            base.OnEnable();

            serializedObject.Update();

            hasSetupDefaults = serializedObject.FindProperty(nameof(hasSetupDefaults));
            controllerProfiles = serializedObject.FindProperty(nameof(controllerProfiles));

            dataProviderProfile = (BaseMixedRealityControllerDataProviderProfile)serializedObject.targetObject;

            if (!hasSetupDefaults.boolValue)
            {
                var defaultControllerOptions = dataProviderProfile.GetDefaultControllerOptions();

                Debug.Assert(defaultControllerOptions != null, $"Missing default controller definitions for {dataProviderProfile.name}");

                var profileRootPath = AssetDatabase.GetAssetPath(dataProviderProfile);
                profileRootPath = $"{Directory.GetParent(Directory.GetParent(profileRootPath).FullName).FullName}/DataProviders";

                controllerProfiles.ClearArray();

                for (int i = 0; i < defaultControllerOptions.Length; i++)
                {
                    var defaultControllerOption = defaultControllerOptions[i];

                    var controllerMappingAsset = CreateInstance(nameof(MixedRealityControllerProfile)).CreateAsset($"{profileRootPath}/", $"{defaultControllerOption.Description}Profile", false) as MixedRealityControllerProfile;

                    Debug.Assert(controllerMappingAsset != null);

                    var mappingProfileSerializedObject = new SerializedObject(controllerMappingAsset);
                    mappingProfileSerializedObject.Update();

                    var controllerTypeProperty = mappingProfileSerializedObject.FindProperty("controllerType").FindPropertyRelative("reference");
                    var handednessProperty = mappingProfileSerializedObject.FindProperty("handedness");
                    var interactionMappingProfilesProperty = mappingProfileSerializedObject.FindProperty("interactionMappingProfiles");

                    if (defaultControllerOption.ControllerType.Type.GUID != Guid.Empty)
                    {
                        controllerTypeProperty.stringValue = defaultControllerOption.ControllerType.Type.GUID.ToString();
                    }
                    else
                    {
                        Debug.LogError($"{defaultControllerOption.ControllerType} requires a {nameof(GuidAttribute)}");
                    }

                    handednessProperty.intValue = (int)defaultControllerOption.Handedness;

                    if (Activator.CreateInstance(new SystemType(controllerTypeProperty.stringValue)) is BaseController)
                    {
                        interactionMappingProfilesProperty.ClearArray();
                    }

                    mappingProfileSerializedObject.ApplyModifiedProperties();

                    controllerProfiles.InsertArrayElementAtIndex(i);
                    var mappingProfile = controllerProfiles.GetArrayElementAtIndex(i);
                    mappingProfile.objectReferenceValue = controllerMappingAsset;
                    mappingProfile.serializedObject.ApplyModifiedProperties();
                }

                hasSetupDefaults.boolValue = true;

                serializedObject.ApplyModifiedProperties();
            }

            mappingProfileList = new ReorderableList(serializedObject, controllerProfiles, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            mappingProfileList.drawElementCallback += DrawConfigurationOptionElement;
            mappingProfileList.onAddCallback += OnConfigurationOptionAdded;
            mappingProfileList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines all of the controllers and their mappings to use for a specific class of device. Additional platform settings can also be available as well.");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Profiles", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            DrawAdvancedControllerMappingProfilesView();
        }

        private void DrawAdvancedControllerMappingProfilesView()
        {
            serializedObject.Update();
            mappingProfileList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedElement = index;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += 3;
            var mappingProfileProperty = controllerProfiles.GetArrayElementAtIndex(index);
            MixedRealityProfilePropertyDrawer.ProfileTypeOverride = typeof(MixedRealityControllerProfile);
            EditorGUI.PropertyField(position, mappingProfileProperty, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            controllerProfiles.arraySize += 1;
            var index = controllerProfiles.arraySize - 1;

            var mappingProfileProperty = controllerProfiles.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedElement >= 0)
            {
                controllerProfiles.DeleteArrayElementAtIndex(currentlySelectedElement);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
