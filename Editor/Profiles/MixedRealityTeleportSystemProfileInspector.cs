// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.TeleportSystem;
using XRTK.Services;
using XRTK.Services.Teleportation;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityTeleportSystemProfile))]
    public class MixedRealityTeleportSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportMode;
        private SerializedProperty teleportProvider;

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportMode = serializedObject.FindProperty(nameof(teleportMode));
            teleportProvider = serializedObject.FindProperty(nameof(teleportProvider));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The teleport system profile defines behaviour for the teleport system.");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(teleportMode);

            var activeTeleportMode = (TeleportMode)teleportMode.intValue;
            switch (activeTeleportMode)
            {
                case TeleportMode.Provider:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(teleportProvider);
                    EditorGUI.indentLevel--;
                    break;
            }

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