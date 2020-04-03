// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.BoundarySystem;

namespace XRTK.Inspectors.Profiles
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
            RenderHeader();

            EditorGUILayout.LabelField("Boundary Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary visualizations can help users stay oriented and comfortable in the experience.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(boundaryHeight);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Floor Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showFloor, showContent);
            EditorGUILayout.PropertyField(floorMaterial, materialContent);
            var prevWideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUILayout.PropertyField(floorScale, scaleContent, GUILayout.ExpandWidth(true));
            EditorGUIUtility.wideMode = prevWideMode;
            EditorGUILayout.PropertyField(floorPhysicsLayer);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Play Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showPlayArea, showContent);
            EditorGUILayout.PropertyField(playAreaMaterial, materialContent);
            EditorGUILayout.PropertyField(playAreaPhysicsLayer);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Tracked Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showTrackedArea, showContent);
            EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);
            EditorGUILayout.PropertyField(trackedAreaPhysicsLayer);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary Wall Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showBoundaryWalls, showContent);
            EditorGUILayout.PropertyField(boundaryWallMaterial, materialContent);
            EditorGUILayout.PropertyField(boundaryWallsPhysicsLayer);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary Ceiling Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showBoundaryCeiling, showContent);
            EditorGUILayout.PropertyField(boundaryCeilingMaterial, materialContent);
            EditorGUILayout.PropertyField(ceilingPhysicsLayer);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
