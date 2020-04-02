// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.Controllers.Simulation.Hands;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedHandControllerDataProviderProfile))]
    public class SimulatedHandControllerDataProviderProfileInspector : SimulatedControllerDataProviderProfileInspector
    {
        // Global hand settings overrides
        private SerializedProperty handMeshingEnabled;
        private SerializedProperty handRayType;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;

        private SerializedProperty poseDefinitions;
        private SerializedProperty handPoseAnimationSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();

            handMeshingEnabled = serializedObject.FindProperty(nameof(handMeshingEnabled));
            handRayType = serializedObject.FindProperty(nameof(handRayType));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));

            poseDefinitions = serializedObject.FindProperty(nameof(poseDefinitions));
            handPoseAnimationSpeed = serializedObject.FindProperty(nameof(handPoseAnimationSpeed));
        }

        protected override void OnInspectorAdditionalGUI()
        {
            EditorGUILayout.PropertyField(handMeshingEnabled);
            EditorGUILayout.PropertyField(handRayType);
            EditorGUILayout.PropertyField(handPhysicsEnabled);
            EditorGUILayout.PropertyField(useTriggers);
            EditorGUILayout.PropertyField(boundsMode);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Hand Simulation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(poseDefinitions, true);
            EditorGUILayout.PropertyField(handPoseAnimationSpeed);
            EditorGUILayout.Space();
        }
    }
}