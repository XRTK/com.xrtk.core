// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Editor.PropertyDrawers;
using XRTK.Extensions;

namespace XRTK.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityInteractionMappingProfile))]
    public class MixedRealityInteractionMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        private SerializedProperty interactionMapping;
        private SerializedProperty pointerProfiles;
        private SerializedProperty description;
        private SerializedProperty stateChangeType;
        private SerializedProperty axisType;
        private SerializedProperty inputType;
        private SerializedProperty inputAction;
        private SerializedProperty keyCode;
        private SerializedProperty inputName;
        private SerializedProperty axisCodeX;
        private SerializedProperty axisCodeY;
        private SerializedProperty inputProcessors;

        private ReorderableList profileList;
        private int selectedPointerIndex;

        protected override void OnEnable()
        {
            base.OnEnable();

            interactionMapping = serializedObject.FindProperty(nameof(interactionMapping));

            description = interactionMapping.FindPropertyRelative(nameof(description));
            stateChangeType = interactionMapping.FindPropertyRelative(nameof(stateChangeType));
            axisType = interactionMapping.FindPropertyRelative(nameof(axisType));
            inputType = interactionMapping.FindPropertyRelative(nameof(inputType));
            inputAction = interactionMapping.FindPropertyRelative(nameof(inputAction));
            keyCode = interactionMapping.FindPropertyRelative(nameof(keyCode));
            inputName = interactionMapping.FindPropertyRelative(nameof(inputName));
            axisCodeX = interactionMapping.FindPropertyRelative(nameof(axisCodeX));
            axisCodeY = interactionMapping.FindPropertyRelative(nameof(axisCodeY));
            inputProcessors = interactionMapping.FindPropertyRelative(nameof(inputProcessors));

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
            RenderHeader("A distinct input pattern that can be recognized on a physical control mechanism. An Interaction only triggers an InputAction when the device's raw input matches the criteria defined by the Processor.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(inputName);
            // EditorGUILayout.PropertyField(stateChangeType); TODO Implement
            EditorGUILayout.PropertyField(axisType);
            EditorGUILayout.PropertyField(inputType);
            var currentAxisType = (AxisType)axisType.intValue;
            inputActionDropdown.OnGui(new GUIContent(inputAction.displayName, inputAction.tooltip), inputAction, currentAxisType);

            switch (currentAxisType)
            {
                case AxisType.Digital:
                    EditorGUILayout.PropertyField(keyCode);
                    break;
                case AxisType.SingleAxis:
                    EditorGUILayout.PropertyField(axisCodeX);
                    break;
                case AxisType.DualAxis:
                    EditorGUILayout.PropertyField(axisCodeX);
                    EditorGUILayout.PropertyField(axisCodeY);
                    break;
            }

            if (currentAxisType != AxisType.Raw &&
                currentAxisType != AxisType.None)
            {
                RenderInputProcessors(inputProcessors, currentAxisType);
            }

            if (currentAxisType == AxisType.SixDof || currentAxisType == AxisType.ThreeDofPosition)
            {
                EditorGUILayout.LabelField("Registered Pointers", EditorStyles.boldLabel);
                profileList.DoLayoutList();
            }
            else
            {
                if (pointerProfiles.arraySize > 0)
                {
                    pointerProfiles.ClearArray();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderInputProcessors(SerializedProperty inputProcessorList, AxisType currentAxisType)
        {
            Type inputProcessorType;

            switch (currentAxisType)
            {
                case AxisType.Digital:
                    inputProcessorType = typeof(InputProcessor<bool>);
                    break;
                case AxisType.SingleAxis:
                    inputProcessorType = typeof(InputProcessor<float>);
                    break;
                case AxisType.DualAxis:
                    inputProcessorType = typeof(InputProcessor<Vector2>);
                    break;
                case AxisType.ThreeDofPosition:
                    inputProcessorType = typeof(InputProcessor<Vector3>);
                    break;
                case AxisType.ThreeDofRotation:
                    inputProcessorType = typeof(InputProcessor<Quaternion>);
                    break;
                case AxisType.SixDof:
                    inputProcessorType = typeof(InputProcessor<MixedRealityPose>);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentAxisType), currentAxisType, null);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"{currentAxisType.ToString().ToProperCase()} Input Processors", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+ Add Input Processor"))
            {
                inputProcessorList.arraySize++;
                var newProcessor = inputProcessorList.GetArrayElementAtIndex(inputProcessorList.arraySize - 1);
                newProcessor.objectReferenceValue = null;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            for (int i = 0; i < inputProcessorList.arraySize; i++)
            {
                EditorGUILayout.Space();
                var processorProperty = inputProcessorList.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                var processorObject = processorProperty.objectReferenceValue;

                EditorGUI.BeginChangeCheck();
                processorObject = EditorGUILayout.ObjectField(GUIContent.none, processorObject, inputProcessorType, false);

                if (EditorGUI.EndChangeCheck())
                {
                    processorProperty.objectReferenceValue = processorObject;
                }

                if (GUILayout.Button("-", GUILayout.Width(24f)))
                {
                    inputProcessorList.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (processorProperty.objectReferenceValue == null) { continue; }

                EditorGUI.indentLevel++;
                var processorEditor = CreateEditor(processorProperty.objectReferenceValue);
                var iterator = processorEditor.serializedObject.GetIterator();
                iterator.NextVisible(true);

                while (iterator.NextVisible(false))
                {
                    processorEditor.serializedObject.Update();
                    EditorGUILayout.PropertyField(iterator, true);
                    processorEditor.serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
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