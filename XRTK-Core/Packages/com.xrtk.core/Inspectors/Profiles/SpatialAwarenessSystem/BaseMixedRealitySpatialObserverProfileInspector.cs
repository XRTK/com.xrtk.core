// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Inspectors.Extensions;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialObserverProfile), true, isFallback = true)]
    public abstract class BaseMixedRealitySpatialObserverProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty isStationaryObserver;
        private SerializedProperty updateInterval;
        private SerializedProperty physicsLayer;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            startupBehavior = serializedObject.FindProperty(nameof(startupBehavior));
            startupBehavior.isExpanded = true;
            observationExtents = serializedObject.FindProperty(nameof(observationExtents));
            isStationaryObserver = serializedObject.FindProperty(nameof(isStationaryObserver));
            updateInterval = serializedObject.FindProperty(nameof(updateInterval));
            physicsLayer = serializedObject.FindProperty(nameof(physicsLayer));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            RenderHeader("The Spatial Awareness Observer Data Provider supplies the Spatial Awareness System with all the data it needs to understand the world around you.");

            serializedObject.Update();

            startupBehavior.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(startupBehavior.isExpanded, generalSettingsFoldoutHeader, true);
            if (startupBehavior.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startupBehavior);
                EditorGUILayout.PropertyField(observationExtents);
                EditorGUILayout.PropertyField(isStationaryObserver);
                EditorGUILayout.PropertyField(updateInterval);
                EditorGUILayout.PropertyField(physicsLayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}