// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Editor.PropertyDrawers;
using XRTK.Editor.Extensions;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Editor.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialMeshObserverProfile), true, isFallback = true)]
    public class BaseMixedRealitySpatialMeshObserverProfileInspector : BaseMixedRealitySpatialObserverProfileInspector
    {
        private SerializedProperty meshLevelOfDetail;
        private SerializedProperty meshRecalculateNormals;
        private SerializedProperty meshVisibleMaterial;
        private SerializedProperty meshOcclusionMaterial;
        private SerializedProperty additionalComponents;
        private SerializedProperty meshObjectPrefab;

        private ReorderableList additionalComponentsList;
        private int currentlySelectedConfigurationOption;

        private readonly GUIContent spatialMeshSettingsFoldoutHeader = new GUIContent("Spatial Mesh Settings");

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            meshLevelOfDetail = serializedObject.FindProperty(nameof(meshLevelOfDetail));
            meshRecalculateNormals = serializedObject.FindProperty(nameof(meshRecalculateNormals));
            meshVisibleMaterial = serializedObject.FindProperty(nameof(meshVisibleMaterial));
            meshOcclusionMaterial = serializedObject.FindProperty(nameof(meshOcclusionMaterial));
            additionalComponents = serializedObject.FindProperty(nameof(additionalComponents));
            meshObjectPrefab = serializedObject.FindProperty(nameof(meshObjectPrefab));

            additionalComponentsList = new ReorderableList(serializedObject, additionalComponents, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.2f
            };

            additionalComponentsList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, new GUIContent(additionalComponents.displayName, additionalComponents.tooltip));
            };
            additionalComponentsList.drawElementCallback += DrawConfigurationOptionElement;
            additionalComponentsList.onAddCallback += OnConfigurationOptionAdded;
            additionalComponentsList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedConfigurationOption = index;
            }

            var componentItem = additionalComponents.GetArrayElementAtIndex(index);

            TypeReferencePropertyDrawer.FilterConstraintOverride = type => !type.IsAbstract && typeof(Component).IsAssignableFrom(type);
            EditorGUI.PropertyField(rect, componentItem, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            additionalComponents.arraySize += 1;
            var index = additionalComponents.arraySize - 1;
            additionalComponents.GetArrayElementAtIndex(index).FindPropertyRelative("reference").stringValue = string.Empty;
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedConfigurationOption >= 0)
            {
                additionalComponents.DeleteArrayElementAtIndex(currentlySelectedConfigurationOption);
            }
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (meshLevelOfDetail.FoldoutWithBoldLabelPropertyField(spatialMeshSettingsFoldoutHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshRecalculateNormals);
                EditorGUILayout.PropertyField(meshVisibleMaterial);
                EditorGUILayout.PropertyField(meshOcclusionMaterial);
                EditorGUILayout.HelpBox("Additional Components to add to the Spatial Mesh Observer\n\nNote: MeshFilter, MeshRenderer, and MeshCollider are already added automatically as they're required components for a mesh object.", MessageType.Info);
                additionalComponentsList.DoLayoutList();
                EditorGUILayout.PropertyField(meshObjectPrefab);
                EditorGUILayout.HelpBox("The mesh object is procedurally generated, but you can also use an empty prefab object as well with predefined components and data.", MessageType.Info);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}