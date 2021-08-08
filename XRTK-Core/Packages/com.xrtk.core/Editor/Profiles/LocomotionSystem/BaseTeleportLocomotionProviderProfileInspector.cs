// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    public abstract class BaseTeleportLocomotionProviderProfileInspector : LocomotionProviderProfileInspector
    {
        private SerializedProperty inputThreshold;
        private SerializedProperty teleportActivationAngle;
        private SerializedProperty angleOffset;
        private SerializedProperty rotateActivationAngle;
        private SerializedProperty rotationAmount;
        private SerializedProperty backStrafeActivationAngle;
        private SerializedProperty strafeAmount;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputThreshold = serializedObject.FindProperty(nameof(inputThreshold));
            teleportActivationAngle = serializedObject.FindProperty(nameof(teleportActivationAngle));
            angleOffset = serializedObject.FindProperty(nameof(angleOffset));
            rotateActivationAngle = serializedObject.FindProperty(nameof(rotateActivationAngle));
            rotationAmount = serializedObject.FindProperty(nameof(rotationAmount));
            backStrafeActivationAngle = serializedObject.FindProperty(nameof(backStrafeActivationAngle));
            strafeAmount = serializedObject.FindProperty(nameof(strafeAmount));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.LabelField("Dual Axis Input Action Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(inputThreshold);
            EditorGUILayout.PropertyField(teleportActivationAngle);
            EditorGUILayout.PropertyField(angleOffset);
            EditorGUILayout.PropertyField(rotateActivationAngle);
            EditorGUILayout.PropertyField(rotationAmount);
            EditorGUILayout.PropertyField(backStrafeActivationAngle);
            EditorGUILayout.PropertyField(strafeAmount);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
