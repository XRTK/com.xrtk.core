// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedHandControllerDataProviderProfile))]
    public class SimulatedHandControllerDataProviderProfileInspector : SimulatedControllerDataProviderProfileInspector
    {
        private static readonly GUIContent SimulatedHandSettingsFoldoutHeader = new GUIContent("Simulated Hand Tracking Settings");
        private static readonly GUIContent handPoseAnimationSpeedLabel = new GUIContent("Hand Pose Animation Speed");

        private SerializedProperty handMeshingEnabled;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;
        private SerializedProperty trackedPoses;
        private SerializedProperty handPoseAnimationSpeed;

        private bool showSimulatedHandTrackingSettings = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            handMeshingEnabled = serializedObject.FindProperty(nameof(handMeshingEnabled));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));

            trackedPoses = serializedObject.FindProperty(nameof(trackedPoses));

            handPoseAnimationSpeed = serializedObject.FindProperty(nameof(handPoseAnimationSpeed));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();

            showSimulatedHandTrackingSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showSimulatedHandTrackingSettings, SimulatedHandSettingsFoldoutHeader, true);
            if (showSimulatedHandTrackingSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Hand Rendering Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(handMeshingEnabled);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Hand Physics Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(handPhysicsEnabled);
                EditorGUILayout.PropertyField(useTriggers);
                EditorGUILayout.PropertyField(boundsMode);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Tracked Hand Poses");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trackedPoses, true);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Simulated Poses");
                EditorGUI.indentLevel++;
                handPoseAnimationSpeed.floatValue = EditorGUILayout.Slider(handPoseAnimationSpeedLabel, handPoseAnimationSpeed.floatValue, 1, 10);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}