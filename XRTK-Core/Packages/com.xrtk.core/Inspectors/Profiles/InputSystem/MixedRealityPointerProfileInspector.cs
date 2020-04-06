// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.InputSystem;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugPointingRayColors;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointingRaycastLayerMasks = serializedObject.FindProperty(nameof(pointingRaycastLayerMasks));
            debugDrawPointingRays = serializedObject.FindProperty(nameof(debugDrawPointingRays));
            debugPointingRayColors = serializedObject.FindProperty(nameof(debugPointingRayColors));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Pointer Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Pointers are the raycasters for controllers and other tracked objects that need to understand what they're 'looking' at.\n\nPointers are managed and updated by the Focus Provider.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(pointingExtent);
            EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
            EditorGUILayout.PropertyField(debugDrawPointingRays);
            EditorGUILayout.PropertyField(debugPointingRayColors, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}