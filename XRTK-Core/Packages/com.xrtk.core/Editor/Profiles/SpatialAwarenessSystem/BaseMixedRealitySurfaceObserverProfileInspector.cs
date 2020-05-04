// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Editor.Extensions;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Editor.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySurfaceObserverProfile), true, isFallback = true)]
    public class BaseMixedRealitySurfaceObserverProfileInspector : BaseMixedRealitySpatialObserverProfileInspector
    {
        private readonly GUIContent surfaceFoldoutContent = new GUIContent("Surface Finding Settings");

        private SerializedProperty surfaceFindingMinimumArea;
        private SerializedProperty displayFloorSurfaces;
        private SerializedProperty floorSurfaceMaterial;
        private SerializedProperty displayCeilingSurfaces;
        private SerializedProperty ceilingSurfaceMaterial;
        private SerializedProperty displayWallSurfaces;
        private SerializedProperty wallSurfaceMaterial;
        private SerializedProperty displayPlatformSurfaces;
        private SerializedProperty platformSurfaceMaterial;

        private readonly GUIContent ceilingMaterialContent = new GUIContent("Ceiling Material");
        private readonly GUIContent floorMaterialContent = new GUIContent("Floor Material");
        private readonly GUIContent platformMaterialContent = new GUIContent("Platform Material");
        private readonly GUIContent wallMaterialContent = new GUIContent("Wall Material");
        private readonly GUIContent minimumAreaContent = new GUIContent("Minimum Area");

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            surfaceFindingMinimumArea = serializedObject.FindProperty(nameof(surfaceFindingMinimumArea));
            displayFloorSurfaces = serializedObject.FindProperty(nameof(displayFloorSurfaces));
            floorSurfaceMaterial = serializedObject.FindProperty(nameof(floorSurfaceMaterial));
            displayCeilingSurfaces = serializedObject.FindProperty(nameof(displayCeilingSurfaces));
            ceilingSurfaceMaterial = serializedObject.FindProperty(nameof(ceilingSurfaceMaterial));
            displayWallSurfaces = serializedObject.FindProperty(nameof(displayWallSurfaces));
            wallSurfaceMaterial = serializedObject.FindProperty(nameof(wallSurfaceMaterial));
            displayPlatformSurfaces = serializedObject.FindProperty(nameof(displayPlatformSurfaces));
            platformSurfaceMaterial = serializedObject.FindProperty(nameof(platformSurfaceMaterial));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (surfaceFindingMinimumArea.FoldoutWithBoldLabelPropertyField(surfaceFoldoutContent, minimumAreaContent))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(displayFloorSurfaces);
                EditorGUILayout.PropertyField(floorSurfaceMaterial, floorMaterialContent);
                EditorGUILayout.PropertyField(displayCeilingSurfaces);
                EditorGUILayout.PropertyField(ceilingSurfaceMaterial, ceilingMaterialContent);
                EditorGUILayout.PropertyField(displayWallSurfaces);
                EditorGUILayout.PropertyField(wallSurfaceMaterial, wallMaterialContent);
                EditorGUILayout.PropertyField(displayPlatformSurfaces);
                EditorGUILayout.PropertyField(platformSurfaceMaterial, platformMaterialContent);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}