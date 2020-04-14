// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Inspectors.Extensions;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseHandControllerDataProviderProfile), true, isFallback = true)]
    public class BaseMixedRealityHandControllerDataProviderProfileInspector : BaseMixedRealityControllerDataProviderProfileInspector
    {
        private SerializedProperty handMeshingEnabled;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;

        private bool showHandTrackingSettings = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            handMeshingEnabled = serializedObject.FindProperty(nameof(handMeshingEnabled));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            showHandTrackingSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showHandTrackingSettings, new GUIContent("Hand Tracking Settings"), true);
            if (showHandTrackingSettings)
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
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}