// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.TeleportSystem;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityTeleportSystemProfile))]
    public class MixedRealityTeleportSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportDuration = serializedObject.FindProperty(nameof(teleportDuration));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The teleport system profile defines default behaviour for the teleport system.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(teleportDuration);

            EditorGUILayout.Space();
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}