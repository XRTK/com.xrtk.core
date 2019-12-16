// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using UnityEngine;
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

            focusProviderType = serializedObject.FindProperty("focusProviderType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
            controllerDataProvidersProfile = serializedObject.FindProperty("controllerDataProvidersProfile");
            controllerMappingProfiles = serializedObject.FindProperty("controllerMappingProfiles");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();
            bool changed = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(focusProviderType);

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            changed |= RenderProfile(thisProfile, inputActionsProfile);
            changed |= RenderProfile(thisProfile, inputActionRulesProfile);
            changed |= RenderProfile(thisProfile, pointerProfile);
            changed |= RenderProfile(thisProfile, gesturesProfile);
            changed |= RenderProfile(thisProfile, speechCommandsProfile);
            changed |= RenderProfile(thisProfile, controllerVisualizationProfile);
            changed |= RenderProfile(thisProfile, controllerDataProvidersProfile);
            changed |= RenderProfile(thisProfile, controllerMappingProfiles);

            serializedObject.ApplyModifiedProperties();

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}