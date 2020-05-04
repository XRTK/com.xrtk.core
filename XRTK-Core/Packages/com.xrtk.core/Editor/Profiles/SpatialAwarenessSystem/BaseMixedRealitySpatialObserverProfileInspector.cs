// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Editor.Extensions;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Editor.Profiles.SpatialAwareness
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

            if (startupBehavior.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader))
            {
                EditorGUI.indentLevel++;
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