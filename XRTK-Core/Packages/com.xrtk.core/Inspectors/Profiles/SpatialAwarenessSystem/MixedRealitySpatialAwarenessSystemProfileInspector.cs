// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.SpatialAwarenessSystem;

namespace XRTK.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty meshDisplayOption;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            meshDisplayOption = serializedObject.FindProperty(nameof(meshDisplayOption));
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Spatial Awareness System Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Spatial Awareness can enhance your experience by enabling objects to interact with the real world.\n\nBelow is a list of registered Spatial Observers that can gather data about your environment.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(meshDisplayOption);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}