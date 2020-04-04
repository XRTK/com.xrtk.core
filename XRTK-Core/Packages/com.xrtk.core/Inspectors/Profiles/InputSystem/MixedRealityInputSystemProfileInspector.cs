// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty focusProviderType;
        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private SerializedProperty drawDebugPointingRays;
        private SerializedProperty debugPointingRayColors;
        private SerializedProperty gazeProviderType;
        private SerializedProperty gazeCursorPrefab;

        private SerializedProperty inputActionsProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty gesturesProfile;
        private SerializedProperty controllerDataProvidersProfile;

        private bool showGlobalPointerOptions;

        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty(nameof(focusProviderType));
            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointingRaycastLayerMasks = serializedObject.FindProperty(nameof(pointingRaycastLayerMasks));
            drawDebugPointingRays = serializedObject.FindProperty(nameof(drawDebugPointingRays));
            debugPointingRayColors = serializedObject.FindProperty(nameof(debugPointingRayColors));
            gazeProviderType = serializedObject.FindProperty(nameof(gazeProviderType));
            gazeCursorPrefab = serializedObject.FindProperty(nameof(gazeCursorPrefab));

            inputActionsProfile = serializedObject.FindProperty(nameof(inputActionsProfile));
            gesturesProfile = serializedObject.FindProperty(nameof(gesturesProfile));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));
            controllerDataProvidersProfile = serializedObject.FindProperty(nameof(controllerDataProvidersProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(focusProviderType, new GUIContent("Focus Provider"));
            EditorGUILayout.PropertyField(gazeProviderType, new GUIContent("Gaze Provider"));
            EditorGUILayout.PropertyField(gazeCursorPrefab);

            EditorGUILayout.Space();

            showGlobalPointerOptions = EditorGUILayout.Foldout(showGlobalPointerOptions, new GUIContent("Global Pointer Options"), true);

            if (showGlobalPointerOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pointingExtent);
                EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel--;
                var newValue = EditorGUILayout.ToggleLeft(new GUIContent(drawDebugPointingRays.displayName, drawDebugPointingRays.tooltip), drawDebugPointingRays.boolValue);
                EditorGUI.indentLevel++;

                if (EditorGUI.EndChangeCheck())
                {
                    drawDebugPointingRays.boolValue = newValue;
                }

                EditorGUILayout.PropertyField(debugPointingRayColors, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(inputActionsProfile);
            EditorGUILayout.PropertyField(speechCommandsProfile);
            EditorGUILayout.PropertyField(gesturesProfile);
            EditorGUILayout.PropertyField(controllerDataProvidersProfile);

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}