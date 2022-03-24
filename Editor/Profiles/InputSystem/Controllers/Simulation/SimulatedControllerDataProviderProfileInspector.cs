// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedControllerDataProviderProfile))]
    public class SimulatedControllerDataProviderProfileInspector : BaseMixedRealityControllerDataProviderProfileInspector
    {
        private static readonly GUIContent SimulationSettingsFoldoutHeader = new GUIContent("Simulation Settings");

        private SerializedProperty simulatedUpdateFrequency;
        private SerializedProperty controllerHideTimeout;

        private SerializedProperty defaultDistance;
        private SerializedProperty depthMultiplier;
        private SerializedProperty jitterAmount;

        private SerializedProperty toggleLeftPersistentKey;
        private SerializedProperty leftControllerTrackedKey;
        private SerializedProperty toggleRightPersistentKey;
        private SerializedProperty rightControllerTrackedKey;

        private SerializedProperty rotationSpeed;

        private bool showSimulationSettings = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            simulatedUpdateFrequency = serializedObject.FindProperty(nameof(simulatedUpdateFrequency));
            controllerHideTimeout = serializedObject.FindProperty(nameof(controllerHideTimeout));

            defaultDistance = serializedObject.FindProperty(nameof(defaultDistance));
            depthMultiplier = serializedObject.FindProperty(nameof(depthMultiplier));
            jitterAmount = serializedObject.FindProperty(nameof(jitterAmount));

            toggleLeftPersistentKey = serializedObject.FindProperty(nameof(toggleLeftPersistentKey));
            toggleRightPersistentKey = serializedObject.FindProperty(nameof(toggleRightPersistentKey));
            leftControllerTrackedKey = serializedObject.FindProperty(nameof(leftControllerTrackedKey));
            rightControllerTrackedKey = serializedObject.FindProperty(nameof(rightControllerTrackedKey));

            rotationSpeed = serializedObject.FindProperty(nameof(rotationSpeed));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            showSimulationSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showSimulationSettings, SimulationSettingsFoldoutHeader, true);
            if (showSimulationSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("General Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(simulatedUpdateFrequency);
                EditorGUILayout.PropertyField(controllerHideTimeout);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Placement Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(defaultDistance);
                EditorGUILayout.PropertyField(depthMultiplier);
                EditorGUILayout.PropertyField(jitterAmount);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Controls Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(toggleLeftPersistentKey);
                EditorGUILayout.PropertyField(leftControllerTrackedKey);
                EditorGUILayout.PropertyField(toggleRightPersistentKey);
                EditorGUILayout.PropertyField(rightControllerTrackedKey);
                EditorGUILayout.PropertyField(rotationSpeed);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}