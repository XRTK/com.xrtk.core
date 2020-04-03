// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Providers.SpatialObservers;

namespace XRTK.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialObserverProfile))]
    public abstract class BaseMixedRealitySpatialObserverProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty isStationaryObserver;
        private SerializedProperty updateInterval;
        private SerializedProperty physicsLayer;

        private static bool foldout = true;

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
            RenderHeader();

            EditorGUILayout.LabelField("Spatial Observer Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Spatial Awareness Observer Data Provider supplies the Spatial Awareness system with all the data it needs to understand the world around you.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            foldout = EditorGUILayout.Foldout(foldout, "General Settings", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startupBehavior);
                EditorGUILayout.PropertyField(observationExtents);
                EditorGUILayout.PropertyField(isStationaryObserver);
                EditorGUILayout.PropertyField(updateInterval);
                EditorGUILayout.PropertyField(physicsLayer);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}