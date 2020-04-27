// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.BoundarySystem;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityBoundaryVisualizationProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty boundaryHeight;
        private SerializedProperty showFloor;
        private SerializedProperty floorMaterial;
        private SerializedProperty floorScale;
        private SerializedProperty floorPhysicsLayer;

        private SerializedProperty showPlayArea;
        private SerializedProperty playAreaMaterial;
        private SerializedProperty playAreaPhysicsLayer;

        private SerializedProperty showTrackedArea;
        private SerializedProperty trackedAreaMaterial;
        private SerializedProperty trackedAreaPhysicsLayer;

        private SerializedProperty showBoundaryWalls;
        private SerializedProperty boundaryWallMaterial;
        private SerializedProperty boundaryWallsPhysicsLayer;

        private SerializedProperty showBoundaryCeiling;
        private SerializedProperty boundaryCeilingMaterial;
        private SerializedProperty ceilingPhysicsLayer;

        private readonly GUIContent showContent = new GUIContent("Show");
        private readonly GUIContent scaleContent = new GUIContent("Scale");
        private readonly GUIContent materialContent = new GUIContent("Material");
        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");
        private readonly GUIContent floorSettingsFoldoutHeader = new GUIContent("Floor Settings");
        private readonly GUIContent playAreaSettingsFoldoutHeader = new GUIContent("Play Area Settings");
        private readonly GUIContent trackedAreaSettingsFoldoutHeader = new GUIContent("Tracked Area Settings");
        private readonly GUIContent boundaryWallSettingsFoldoutHeader = new GUIContent("Boundary Wall Settings");
        private readonly GUIContent boundaryCeilingSettingsFoldoutHeader = new GUIContent("Boundary Ceiling Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            boundaryHeight = serializedObject.FindProperty(nameof(boundaryHeight));

            showFloor = serializedObject.FindProperty(nameof(showFloor));
            floorMaterial = serializedObject.FindProperty(nameof(floorMaterial));
            floorScale = serializedObject.FindProperty(nameof(floorScale));
            floorPhysicsLayer = serializedObject.FindProperty(nameof(floorPhysicsLayer));

            showPlayArea = serializedObject.FindProperty(nameof(showPlayArea));
            playAreaMaterial = serializedObject.FindProperty(nameof(playAreaMaterial));
            playAreaPhysicsLayer = serializedObject.FindProperty(nameof(playAreaPhysicsLayer));

            showTrackedArea = serializedObject.FindProperty(nameof(showTrackedArea));
            trackedAreaMaterial = serializedObject.FindProperty(nameof(trackedAreaMaterial));
            trackedAreaPhysicsLayer = serializedObject.FindProperty(nameof(trackedAreaPhysicsLayer));

            showBoundaryWalls = serializedObject.FindProperty(nameof(showBoundaryWalls));
            boundaryWallMaterial = serializedObject.FindProperty(nameof(boundaryWallMaterial));
            boundaryWallsPhysicsLayer = serializedObject.FindProperty(nameof(boundaryWallsPhysicsLayer));

            showBoundaryCeiling = serializedObject.FindProperty(nameof(showBoundaryCeiling));
            boundaryCeilingMaterial = serializedObject.FindProperty(nameof(boundaryCeilingMaterial));
            ceilingPhysicsLayer = serializedObject.FindProperty(nameof(ceilingPhysicsLayer));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Boundary visualizations can help users stay oriented and comfortable in the experience.");

            serializedObject.Update();

            boundaryHeight.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader);

            EditorGUILayout.Space();

            if (showFloor.FoldoutWithBoldLabelPropertyField(floorSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(floorMaterial, materialContent);
                var prevWideMode = EditorGUIUtility.wideMode;
                EditorGUIUtility.wideMode = true;
                EditorGUILayout.PropertyField(floorScale, scaleContent, GUILayout.ExpandWidth(true));
                EditorGUIUtility.wideMode = prevWideMode;
                EditorGUILayout.PropertyField(floorPhysicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            if (showPlayArea.FoldoutWithBoldLabelPropertyField(playAreaSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(playAreaMaterial, materialContent);
                EditorGUILayout.PropertyField(playAreaPhysicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            if (showTrackedArea.FoldoutWithBoldLabelPropertyField(trackedAreaSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);
                EditorGUILayout.PropertyField(trackedAreaPhysicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            if (showBoundaryWalls.FoldoutWithBoldLabelPropertyField(boundaryWallSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(boundaryWallMaterial, materialContent);
                EditorGUILayout.PropertyField(boundaryWallsPhysicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            if (showBoundaryCeiling.FoldoutWithBoldLabelPropertyField(boundaryCeilingSettingsFoldoutHeader, showContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(boundaryCeilingMaterial, materialContent);
                EditorGUILayout.PropertyField(ceilingPhysicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
