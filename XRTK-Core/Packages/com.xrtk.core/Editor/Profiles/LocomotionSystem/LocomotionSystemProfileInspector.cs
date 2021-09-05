// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Services;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(LocomotionSystemProfile))]
    public class LocomotionSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportCooldown;

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportCooldown = serializedObject.FindProperty(nameof(teleportCooldown));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The locomotion system profile defines behaviour for the locomotion system and how your users will be able to move around in your application.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Movement
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(teleportCooldown);
            EditorGUI.indentLevel--;

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
