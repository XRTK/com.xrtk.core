// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Services;
using XRTK.Definitions.LocomotionSystem;
using UnityEngine;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityLocomotionSystemProfile))]
    public class MixedRealityLocomotionSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportAction;
        private SerializedProperty movementCancelsTeleport;

        private static readonly GUIContent movementCancelsTeleportLabel = new GUIContent("Cancels Teleport");

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportAction = serializedObject.FindProperty(nameof(teleportAction));
            movementCancelsTeleport = serializedObject.FindProperty(nameof(movementCancelsTeleport));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The locomotion system profile defines behaviour for the locomotion system and how your users will be able to move around in your application.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Teleporting
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Teleportation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(teleportAction);
            EditorGUI.indentLevel--;

            // Movement
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(movementCancelsTeleport, movementCancelsTeleportLabel);
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
