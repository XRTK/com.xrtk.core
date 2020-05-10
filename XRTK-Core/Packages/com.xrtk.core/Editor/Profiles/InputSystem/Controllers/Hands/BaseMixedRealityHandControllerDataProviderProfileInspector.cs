// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseHandControllerDataProviderProfile), true, isFallback = true)]
    public class BaseMixedRealityHandControllerDataProviderProfileInspector : BaseMixedRealityControllerDataProviderProfileInspector
    {
        private SerializedProperty renderingMode;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;
        private SerializedProperty trackedPoses;

        private bool showHandTrackingSettings = true;
        private static readonly GUIContent handTrackingSettingsFoldoutHeader = new GUIContent("Hand Tracking Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            renderingMode = serializedObject.FindProperty(nameof(renderingMode));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
            trackedPoses = serializedObject.FindProperty(nameof(trackedPoses));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            showHandTrackingSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showHandTrackingSettings, handTrackingSettingsFoldoutHeader, true);
            if (showHandTrackingSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Hand Rendering Settings");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(renderingMode);
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

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}