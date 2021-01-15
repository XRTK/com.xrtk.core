// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Services;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityLocomotionSystemProfile))]
    public class MixedRealityLocomotionSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        // Teleporting
        private SerializedProperty hotSpotsOnly;
        private SerializedProperty teleportProvider;
        private SerializedProperty teleportAction;
        private SerializedProperty cancelTeleportAction;
        private const string noTeleportProviderAssignedMessage = @"When no Teleport Provider is assigned, the system will revert to default behvaiour and perform an instant teleport to the target location. ";

        // Movement
        private SerializedProperty movementProvider;
        private SerializedProperty moveAction;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Teleporting
            hotSpotsOnly = serializedObject.FindProperty(nameof(hotSpotsOnly));
            teleportProvider = serializedObject.FindProperty(nameof(teleportProvider));
            teleportAction = serializedObject.FindProperty(nameof(teleportAction));
            cancelTeleportAction = serializedObject.FindProperty(nameof(cancelTeleportAction));

            // Movement
            movementProvider = serializedObject.FindProperty(nameof(movementProvider));
            moveAction = serializedObject.FindProperty(nameof(moveAction));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The locomotion system profile defines behaviour for the locomotion system and how your users will be able to move around in the application.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Teleporting
            EditorGUILayout.LabelField("Teleportation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(hotSpotsOnly);
            EditorGUILayout.PropertyField(teleportProvider);
            var referenceProperty = teleportProvider.FindPropertyRelative("reference");
            if (!XRTK.Extensions.TypeExtensions.TryResolveType(referenceProperty.stringValue, out var referenceType))
            {
                EditorGUILayout.HelpBox(noTeleportProviderAssignedMessage, MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(teleportAction);
            EditorGUILayout.PropertyField(cancelTeleportAction);
            EditorGUI.indentLevel--;

            // Movement
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(movementProvider);
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