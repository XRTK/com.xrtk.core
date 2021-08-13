// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(BaseTeleportLocomotionProviderProfile))]
    public class BaseTeleportLocomotionProviderProfileInspector : LocomotionProviderProfileInspector
    {
        private SerializedProperty inputThreshold;
        private SerializedProperty teleportActivationAngle;
        private SerializedProperty angleOffset;
        private SerializedProperty rotateActivationAngle;
        private SerializedProperty rotationAmount;

        private static readonly GUIContent singleAndDualAxisConfigHeader = new GUIContent("Single / Dual Axis Input Action Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            inputThreshold = serializedObject.FindProperty(nameof(inputThreshold));
            teleportActivationAngle = serializedObject.FindProperty(nameof(teleportActivationAngle));
            angleOffset = serializedObject.FindProperty(nameof(angleOffset));
            rotateActivationAngle = serializedObject.FindProperty(nameof(rotateActivationAngle));
            rotationAmount = serializedObject.FindProperty(nameof(rotationAmount));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (inputThreshold.FoldoutWithBoldLabelPropertyField(singleAndDualAxisConfigHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(teleportActivationAngle);
                EditorGUILayout.PropertyField(angleOffset);
                EditorGUILayout.PropertyField(rotateActivationAngle);
                EditorGUILayout.PropertyField(rotationAmount);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
