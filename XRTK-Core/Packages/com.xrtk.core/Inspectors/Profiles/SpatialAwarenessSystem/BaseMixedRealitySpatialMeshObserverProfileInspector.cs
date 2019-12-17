// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialMeshObserverProfile))]
    public abstract class BaseMixedRealitySpatialMeshObserverProfileInspector : BaseMixedRealitySpatialObserverProfileInspector
    {
        private SerializedProperty meshPhysicsLayerOverride;
        private SerializedProperty meshLevelOfDetail;
        private SerializedProperty meshTrianglesPerCubicMeter;
        private SerializedProperty meshRecalculateNormals;
        private SerializedProperty meshVisibleMaterial;
        private SerializedProperty meshOcclusionMaterial;
        private SerializedProperty additionalComponents;
        private SerializedProperty meshObjectPrefab;

        private static bool foldout = true;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            meshPhysicsLayerOverride = serializedObject.FindProperty("meshPhysicsLayerOverride");
            meshLevelOfDetail = serializedObject.FindProperty("meshLevelOfDetail");
            meshTrianglesPerCubicMeter = serializedObject.FindProperty("meshTrianglesPerCubicMeter");
            meshRecalculateNormals = serializedObject.FindProperty("meshRecalculateNormals");
            meshVisibleMaterial = serializedObject.FindProperty("meshVisibleMaterial");
            meshOcclusionMaterial = serializedObject.FindProperty("meshOcclusionMaterial");
            additionalComponents = serializedObject.FindProperty("additionalComponents");
            meshObjectPrefab = serializedObject.FindProperty("meshObjectPrefab");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            foldout = EditorGUILayout.Foldout(foldout, "Spatial Mesh Settings", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshPhysicsLayerOverride);
                EditorGUILayout.PropertyField(meshLevelOfDetail);
                EditorGUILayout.PropertyField(meshTrianglesPerCubicMeter);
                EditorGUILayout.PropertyField(meshRecalculateNormals);
                EditorGUILayout.PropertyField(meshVisibleMaterial);
                EditorGUILayout.PropertyField(meshOcclusionMaterial);
                EditorGUILayout.PropertyField(additionalComponents, true);
                EditorGUILayout.HelpBox("Note: MeshFilter, MeshRenderer, and MeshCollider are already added automatically as they're required components for a mesh object.", MessageType.Info);
                EditorGUILayout.PropertyField(meshObjectPrefab);
                EditorGUILayout.HelpBox("The mesh object is procedurally generated, but you can also use an empty prefab object as well with predefined components and data.", MessageType.Info);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}