// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Editor.PropertyDrawers;

namespace XRTK.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityControllerProfile))]
    public class BaseMixedRealityControllerMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty controllerType;
        private SerializedProperty handedness;
        private SerializedProperty visualizationProfile;
        private SerializedProperty pointerProfiles;
        private ReorderableList profileList;
        private int selectedPointerIndex;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerType = serializedObject.FindProperty(nameof(controllerType));
            handedness = serializedObject.FindProperty(nameof(handedness));
            visualizationProfile = serializedObject.FindProperty(nameof(visualizationProfile));
            pointerProfiles = serializedObject.FindProperty(nameof(pointerProfiles));

            profileList = new ReorderableList(serializedObject, pointerProfiles, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            profileList.drawElementCallback += DrawConfigurationOptionElement;
            profileList.onAddCallback += OnConfigurationOptionAdded;
            profileList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines the type of controller that is valid for this data provider, which hand it belongs to, and how to visualize this controller in the scene, and binds each interactions on every physical control mechanism or sensor on the device.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(controllerType);
            EditorGUILayout.PropertyField(handedness);
            EditorGUILayout.PropertyField(visualizationProfile);
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Pointers", EditorStyles.boldLabel);
            profileList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                selectedPointerIndex = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var mappingProfileProperty = pointerProfiles.GetArrayElementAtIndex(index);
            MixedRealityProfilePropertyDrawer.ProfileTypeOverride = typeof(MixedRealityPointerProfile);
            EditorGUI.PropertyField(rect, mappingProfileProperty, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            pointerProfiles.arraySize += 1;
            var index = pointerProfiles.arraySize - 1;

            var mappingProfileProperty = pointerProfiles.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (selectedPointerIndex >= 0)
            {
                pointerProfiles.DeleteArrayElementAtIndex(selectedPointerIndex);
            }
        }
    }
}
