// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty meshDisplayOption;
        private SerializedProperty globalMeshObserverProfile;
        private SerializedProperty globalSurfaceObserverProfile;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            meshDisplayOption = serializedObject.FindProperty(nameof(meshDisplayOption));
            globalMeshObserverProfile = serializedObject.FindProperty(nameof(globalMeshObserverProfile));
            globalSurfaceObserverProfile = serializedObject.FindProperty(nameof(globalSurfaceObserverProfile));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            RenderHeader("Spatial Awareness can enhance your experience by enabling objects to interact with the real world.\n\nBelow is a list of registered Spatial Observers that can gather data about your environment.");

            serializedObject.Update();

            if (meshDisplayOption.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(globalMeshObserverProfile);
                EditorGUILayout.PropertyField(globalSurfaceObserverProfile);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}