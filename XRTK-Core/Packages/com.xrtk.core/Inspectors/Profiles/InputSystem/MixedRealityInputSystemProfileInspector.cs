// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.InputSystem;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty focusProviderType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty gesturesProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty controllerVisualizationProfile;
        private SerializedProperty inputDataProvidersProfile;
        private SerializedProperty controllerMappingProfiles;
        private SerializedProperty handTrackingProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty(nameof(focusProviderType));
            inputActionsProfile = serializedObject.FindProperty(nameof(inputActionsProfile));
            pointerProfile = serializedObject.FindProperty(nameof(pointerProfile));
            gesturesProfile = serializedObject.FindProperty(nameof(gesturesProfile));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));
            controllerVisualizationProfile = serializedObject.FindProperty(nameof(controllerVisualizationProfile));
            inputDataProvidersProfile = serializedObject.FindProperty(nameof(inputDataProvidersProfile));
            controllerMappingProfiles = serializedObject.FindProperty(nameof(controllerMappingProfiles));
            handTrackingProfile = serializedObject.FindProperty(nameof(handTrackingProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(focusProviderType);
            EditorGUILayout.PropertyField(inputActionsProfile);
            EditorGUILayout.PropertyField(pointerProfile);
            EditorGUILayout.PropertyField(gesturesProfile);
            EditorGUILayout.PropertyField(speechCommandsProfile);
            EditorGUILayout.PropertyField(controllerVisualizationProfile);
            EditorGUILayout.PropertyField(inputDataProvidersProfile);
            EditorGUILayout.PropertyField(controllerMappingProfiles);
            EditorGUILayout.PropertyField(handTrackingProfile);

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}