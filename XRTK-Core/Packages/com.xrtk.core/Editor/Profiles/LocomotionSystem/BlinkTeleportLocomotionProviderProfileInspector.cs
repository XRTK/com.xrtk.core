// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(BlinkTeleportLocomotionProviderProfile))]
    public class BlinkTeleportLocomotionProviderProfileInspector : BaseTeleportLocomotionProviderProfileInspector
    {
        private SerializedProperty fadeDuration;
        private SerializedProperty fadeMaterial;
        private SerializedProperty fadeInColor;
        private SerializedProperty fadeOutColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            fadeDuration = serializedObject.FindProperty(nameof(fadeDuration));
            fadeMaterial = serializedObject.FindProperty(nameof(fadeMaterial));
            fadeInColor = serializedObject.FindProperty(nameof(fadeInColor));
            fadeOutColor = serializedObject.FindProperty(nameof(fadeOutColor));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(fadeDuration);
            EditorGUILayout.PropertyField(fadeMaterial);
            EditorGUILayout.PropertyField(fadeInColor);
            EditorGUILayout.PropertyField(fadeOutColor);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
