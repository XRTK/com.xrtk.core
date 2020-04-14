// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Inspectors.Extensions;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedHandControllerDataProviderProfile))]
    public class SimulatedHandControllerDataProviderProfileInspector : SimulatedControllerDataProviderProfileInspector
    {
        private SerializedProperty handMeshingEnabled;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;

        private SerializedProperty handPoseAnimationSpeed;
        private SerializedProperty poseDefinitions;

        private ReorderableList poseList;
        private int currentlySelectedElement;

        private bool showSimulatedHandTrackingSettings = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            handMeshingEnabled = serializedObject.FindProperty(nameof(handMeshingEnabled));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));

            poseDefinitions = serializedObject.FindProperty(nameof(poseDefinitions));
            handPoseAnimationSpeed = serializedObject.FindProperty(nameof(handPoseAnimationSpeed));

            poseList = new ReorderableList(serializedObject, poseDefinitions, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            poseList.drawHeaderCallback += DrawHeaderCallback;
            poseList.drawElementCallback += DrawConfigurationOptionElement;
            poseList.onAddCallback += OnConfigurationOptionAdded;
            poseList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Pose Definitions");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();

            showSimulatedHandTrackingSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showSimulatedHandTrackingSettings, new GUIContent("Simulated Hand Tracking Settings"), true);
            if (showSimulatedHandTrackingSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Hand Rendering Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(handMeshingEnabled);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hand Physics Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(handPhysicsEnabled);
                EditorGUILayout.PropertyField(useTriggers);
                EditorGUILayout.PropertyField(boundsMode);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Simulated Poses");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(handPoseAnimationSpeed);
                poseList.DoLayoutList();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedElement = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var poseDataProperty = poseDefinitions.GetArrayElementAtIndex(index);
            var selectedPoseData = EditorGUI.ObjectField(rect, poseDataProperty.objectReferenceValue, typeof(SimulatedHandControllerPoseData), false) as SimulatedHandControllerPoseData;

            if (selectedPoseData != null)
            {
                selectedPoseData.ParentProfile = ThisProfile;
            }

            poseDataProperty.objectReferenceValue = selectedPoseData;
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            poseDefinitions.arraySize += 1;
            var index = poseDefinitions.arraySize - 1;

            var mappingProfileProperty = poseDefinitions.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedElement >= 0)
            {
                poseDefinitions.DeleteArrayElementAtIndex(currentlySelectedElement);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}