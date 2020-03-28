// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Inspectors.Utilities;
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

            startupBehavior = serializedObject.FindProperty("startupBehavior");
            observationExtents = serializedObject.FindProperty("observationExtents");
            isStationaryObserver = serializedObject.FindProperty("isStationaryObserver");
            updateInterval = serializedObject.FindProperty("updateInterval");
            physicsLayer = serializedObject.FindProperty("physicsLayer");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (ThisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Spatial Awareness Profile"))
            {
                Selection.activeObject = ThisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Observer Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Spatial Awareness Observer Data Provider supplies the Spatial Awareness system with all the data it needs to understand the world around you.", MessageType.Info);
            EditorGUILayout.Space();

            ThisProfile.CheckProfileLock();

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