// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.InputSystem;

namespace XRTK.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty pointerPrefab;
        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private SerializedProperty drawDebugPointingRays;
        private SerializedProperty debugPointingRayColors;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointerPrefab = serializedObject.FindProperty(nameof(pointerPrefab));
            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointingRaycastLayerMasks = serializedObject.FindProperty(nameof(pointingRaycastLayerMasks));
            drawDebugPointingRays = serializedObject.FindProperty(nameof(drawDebugPointingRays));
            debugPointingRayColors = serializedObject.FindProperty(nameof(debugPointingRayColors));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Pointers are the raycasters that are attached to controllers and other tracked objects, that understand what they're 'looking' at.\n\nPointers are managed and updated by the Focus Provider.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(pointerPrefab);
            EditorGUILayout.PropertyField(pointingExtent);
            EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
            EditorGUILayout.PropertyField(drawDebugPointingRays);
            EditorGUILayout.PropertyField(debugPointingRayColors, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}