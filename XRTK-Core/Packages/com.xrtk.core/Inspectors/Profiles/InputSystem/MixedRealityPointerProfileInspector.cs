// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Utilities;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent ControllerTypeContent = new GUIContent("Controller Type", "The type of Controller this pointer will attach itself to at runtime.");

        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty gazeProviderType;
        private SerializedProperty pointerOptions;
        private ReorderableList pointerOptionList;

        private int currentlySelectedPointerOption = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointingRaycastLayerMasks = serializedObject.FindProperty(nameof(pointingRaycastLayerMasks));
            debugDrawPointingRays = serializedObject.FindProperty(nameof(debugDrawPointingRays));
            debugDrawPointingRayColors = serializedObject.FindProperty(nameof(debugDrawPointingRayColors));
            gazeCursorPrefab = serializedObject.FindProperty(nameof(gazeCursorPrefab));
            gazeProviderType = serializedObject.FindProperty(nameof(gazeProviderType));
            pointerOptions = serializedObject.FindProperty(nameof(pointerOptions));

            pointerOptionList = new ReorderableList(serializedObject, pointerOptions, false, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 4
            };

            pointerOptionList.drawElementCallback += DrawPointerOptionElement;
            pointerOptionList.onAddCallback += OnPointerOptionAdded;
            pointerOptionList.onRemoveCallback += OnPointerOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Pointer Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Pointers attach themselves onto controllers as they are initialized.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            currentlySelectedPointerOption = -1;

            EditorGUILayout.PropertyField(pointingExtent);
            EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
            EditorGUILayout.PropertyField(debugDrawPointingRays);
            EditorGUILayout.PropertyField(debugDrawPointingRayColors, true);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("The gaze provider uses the default settings above, but further customization of the gaze can be done on the Gaze Provider.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(gazeCursorPrefab);
            EditorGUILayout.PropertyField(gazeProviderType);

            EditorGUILayout.Space();

            if (GUILayout.Button("Customize Gaze Provider Settings"))
            {
                Selection.activeObject = CameraCache.Main.gameObject;
            }

            EditorGUILayout.Space();
            pointerOptionList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPointerOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedPointerOption = index;
            }

            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var halfFieldHeight = EditorGUIUtility.singleLineHeight * 0.25f;
            var controllerTypeRect = new Rect(rect.x, rect.y + halfFieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var handednessControlRect = new Rect(rect.x, rect.y + halfFieldHeight * 6, rect.width, EditorGUIUtility.singleLineHeight);
            var pointerPrefabRect = new Rect(rect.x, rect.y + halfFieldHeight * 11, rect.width, EditorGUIUtility.singleLineHeight);

            var pointerOption = pointerOptions.GetArrayElementAtIndex(index);
            var controllerType = pointerOption.FindPropertyRelative("controllerType");
            var handedness = pointerOption.FindPropertyRelative("handedness");
            var prefab = pointerOption.FindPropertyRelative("pointerPrefab");

            EditorGUI.PropertyField(controllerTypeRect, controllerType, ControllerTypeContent);
            EditorGUI.PropertyField(handednessControlRect, handedness);
            EditorGUI.PropertyField(pointerPrefabRect, prefab);

            EditorGUIUtility.wideMode = lastMode;
        }

        private void OnPointerOptionAdded(ReorderableList list)
        {
            pointerOptions.arraySize += 1;
            var pointerOption = pointerOptions.GetArrayElementAtIndex(pointerOptions.arraySize - 1);
            var controllerType = pointerOption.FindPropertyRelative("controllerType");
            var referenceType = controllerType.FindPropertyRelative("reference");
            referenceType.stringValue = string.Empty;
            var handedness = pointerOption.FindPropertyRelative("handedness");
            handedness.enumValueIndex = 0;
            var prefab = pointerOption.FindPropertyRelative("pointerPrefab");
            prefab.objectReferenceValue = null;
        }

        private void OnPointerOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedPointerOption >= 0)
            {
                pointerOptions.DeleteArrayElementAtIndex(currentlySelectedPointerOption);
            }
        }
    }
}