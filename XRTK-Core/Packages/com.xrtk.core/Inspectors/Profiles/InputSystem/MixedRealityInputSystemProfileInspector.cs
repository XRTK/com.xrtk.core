// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using XRTK.Definitions.InputSystem;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty focusProviderType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty gesturesProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty controllerVisualizationProfile;
        private SerializedProperty controllerDataProvidersProfile;
        private SerializedProperty controllerMappingProfiles;

        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty(nameof(focusProviderType));
            inputActionsProfile = serializedObject.FindProperty(nameof(inputActionsProfile));
            inputActionRulesProfile = serializedObject.FindProperty(nameof(inputActionRulesProfile));
            pointerProfile = serializedObject.FindProperty(nameof(pointerProfile));
            gesturesProfile = serializedObject.FindProperty(nameof(gesturesProfile));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));
            controllerVisualizationProfile = serializedObject.FindProperty(nameof(controllerVisualizationProfile));
            controllerDataProvidersProfile = serializedObject.FindProperty(nameof(controllerDataProvidersProfile));
            controllerMappingProfiles = serializedObject.FindProperty(nameof(controllerMappingProfiles));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);

            ThisProfile.CheckProfileLock();

            serializedObject.Update();
            bool changed = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(focusProviderType);

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            changed |= RenderProfile(ThisProfile, inputActionsProfile);
            changed |= RenderProfile(ThisProfile, inputActionRulesProfile);
            changed |= RenderProfile(ThisProfile, pointerProfile);
            changed |= RenderProfile(ThisProfile, gesturesProfile);
            changed |= RenderProfile(ThisProfile, speechCommandsProfile);
            changed |= RenderProfile(ThisProfile, controllerVisualizationProfile);
            changed |= RenderProfile(ThisProfile, controllerDataProvidersProfile);
            changed |= RenderProfile(ThisProfile, controllerMappingProfiles);

            serializedObject.ApplyModifiedProperties();

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}