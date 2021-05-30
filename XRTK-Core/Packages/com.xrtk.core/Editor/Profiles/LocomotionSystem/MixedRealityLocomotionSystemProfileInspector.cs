// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Services;
using XRTK.Definitions.LocomotionSystem;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityLocomotionSystemProfile))]
    public class MixedRealityLocomotionSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty teleportStartupBehaviour;
        private SerializedProperty teleportAction;
        private SerializedProperty movementStartupBehaviour;
        private SerializedProperty movementCancelsTeleport;
        private SerializedProperty moveAction;

        private static readonly GUIContent movementCancelsTeleportLabel = new GUIContent("Cancels Teleport");
        private static readonly GUIContent teleportStartupBehaviourLabel = new GUIContent("Startup Behaviour");
        private static readonly GUIContent movementStartupBehaviourLabel = new GUIContent("Startup Behaviour");

        protected override void OnEnable()
        {
            base.OnEnable();

            teleportStartupBehaviour = serializedObject.FindProperty(nameof(teleportStartupBehaviour));
            teleportAction = serializedObject.FindProperty(nameof(teleportAction));
            movementStartupBehaviour = serializedObject.FindProperty(nameof(movementStartupBehaviour));
            movementCancelsTeleport = serializedObject.FindProperty(nameof(movementCancelsTeleport));
            moveAction = serializedObject.FindProperty(nameof(moveAction));
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

            if (((AutoStartBehavior)movementStartupBehaviour.intValue) == AutoStartBehavior.ManualStart)
            {
                EditorGUILayout.HelpBox("Movement is currently disabled. Teleportation will only work once movement has been enabled programmatically at runtime.", MessageType.Info);
            }

            EditorGUILayout.PropertyField(teleportStartupBehaviour, teleportStartupBehaviourLabel);
            EditorGUILayout.PropertyField(teleportAction);
            EditorGUI.indentLevel--;

            // Movement
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(movementStartupBehaviour, movementStartupBehaviourLabel);
            EditorGUILayout.PropertyField(movementCancelsTeleport, movementCancelsTeleportLabel);
            EditorGUILayout.PropertyField(moveAction);
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
