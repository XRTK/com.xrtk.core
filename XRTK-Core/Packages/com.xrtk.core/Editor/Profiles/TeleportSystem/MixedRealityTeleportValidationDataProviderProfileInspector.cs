// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.TeleportSystem;

namespace XRTK.Editor.Profiles.TeleportSystem
{
    [CustomEditor(typeof(MixedRealityTeleportValidationDataProviderProfile))]
    public class MixedRealityTeleportValidationDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty validLayers;
        private SerializedProperty invalidLayers;
        private SerializedProperty upDirectionThreshold;
        private SerializedProperty maxDistance;

        protected override void OnEnable()
        {
            base.OnEnable();

            validLayers = serializedObject.FindProperty(nameof(validLayers));
            invalidLayers = serializedObject.FindProperty(nameof(invalidLayers));
            upDirectionThreshold = serializedObject.FindProperty(nameof(upDirectionThreshold));
            maxDistance = serializedObject.FindProperty(nameof(maxDistance));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines the set of rules to validate against when deciding whether a teleport target location is valid or not.");

            EditorGUILayout.PropertyField(validLayers);
            EditorGUILayout.PropertyField(invalidLayers);
            EditorGUILayout.PropertyField(upDirectionThreshold);
            EditorGUILayout.PropertyField(maxDistance);
        }
    }
}
