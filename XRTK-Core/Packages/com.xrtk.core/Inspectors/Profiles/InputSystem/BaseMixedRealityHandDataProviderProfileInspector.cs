// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.Controllers;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(BaseMixedRealityHandDataProviderProfile))]
    public class BaseMixedRealityHandDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty handMeshingEnabled;
        private SerializedProperty handRayType;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;

        protected override void OnEnable()
        {
            base.OnEnable();

            handMeshingEnabled = serializedObject.FindProperty(nameof(handMeshingEnabled));
            handRayType = serializedObject.FindProperty(nameof(handRayType));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(handMeshingEnabled);
            EditorGUILayout.PropertyField(handRayType);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(handPhysicsEnabled);
            EditorGUILayout.PropertyField(useTriggers);
            EditorGUILayout.PropertyField(boundsMode);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}