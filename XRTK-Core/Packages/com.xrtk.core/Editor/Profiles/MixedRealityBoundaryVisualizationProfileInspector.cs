// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.BoundarySystem;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityBoundaryProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty showBoundary;
        private SerializedProperty boundaryHeight;
        private SerializedProperty physicsLayer;
        private SerializedProperty boundaryMaterial;

        private SerializedProperty showFloor;
        private SerializedProperty floorMaterial;

        private SerializedProperty showWalls;
        private SerializedProperty wallMaterial;

        private SerializedProperty showCeiling;
        private SerializedProperty ceilingMaterial;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");
        private readonly GUIContent floorSettingsFoldoutHeader = new GUIContent("Floor Settings");
        private readonly GUIContent wallSettingsFoldoutHeader = new GUIContent("Wall Settings");
        private readonly GUIContent ceilingSettingsFoldoutHeader = new GUIContent("Ceiling Settings");
        private readonly GUIContent materialContent = new GUIContent("Material Override", "Use a different material than the global boundary material?");
        private readonly GUIContent showContent = new GUIContent("Force Show", "Force the boundary component to be displayed.\n\nNote: This can be toggled on/off at runtime.");

        protected override void OnEnable()
        {
            base.OnEnable();

            showBoundary = serializedObject.FindProperty(nameof(showBoundary));
            boundaryHeight = serializedObject.FindProperty(nameof(boundaryHeight));
            boundaryMaterial = serializedObject.FindProperty(nameof(boundaryMaterial));
            physicsLayer = serializedObject.FindProperty(nameof(physicsLayer));

            showFloor = serializedObject.FindProperty(nameof(showFloor));
            floorMaterial = serializedObject.FindProperty(nameof(floorMaterial));

            showWalls = serializedObject.FindProperty(nameof(showWalls));
            wallMaterial = serializedObject.FindProperty(nameof(wallMaterial));

            showCeiling = serializedObject.FindProperty(nameof(showCeiling));
            ceilingMaterial = serializedObject.FindProperty(nameof(ceilingMaterial));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Boundary visualizations can help users stay oriented and comfortable in the experience.");

            serializedObject.Update();

            if (showBoundary.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(boundaryHeight);
                EditorGUILayout.PropertyField(boundaryMaterial, materialContent);
                EditorGUILayout.PropertyField(physicsLayer);

                EditorGUILayout.Space();

                if (showFloor.FoldoutWithBoldLabelPropertyField(floorSettingsFoldoutHeader, showContent))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(floorMaterial, materialContent);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.Space();

                if (showWalls.FoldoutWithBoldLabelPropertyField(wallSettingsFoldoutHeader, showContent))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(wallMaterial, materialContent);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                if (showCeiling.FoldoutWithBoldLabelPropertyField(ceilingSettingsFoldoutHeader, showContent))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(ceilingMaterial, materialContent);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
