﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.TeleportSystem;
using XRTK.Services;

namespace XRTK.Editor.Profiles.TeleportSystem
{
    [CustomEditor(typeof(MixedRealityTeleportSystemProfile))]
    public class MixedRealityTeleportSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportProvider;
        private SerializedProperty teleportAction;

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportProvider = serializedObject.FindProperty(nameof(teleportProvider));
            teleportAction = serializedObject.FindProperty(nameof(teleportAction));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The teleport system profile defines behaviour for the teleport system.");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(teleportProvider);
            EditorGUILayout.PropertyField(teleportAction);

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }

            base.OnInspectorGUI();
        }
    }
}