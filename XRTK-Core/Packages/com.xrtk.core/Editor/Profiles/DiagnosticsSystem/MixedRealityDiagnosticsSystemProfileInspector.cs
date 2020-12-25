// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Editor.Extensions;

namespace XRTK.Editor.Profiles.DiagnosticsSystem
{
    [CustomEditor(typeof(MixedRealityDiagnosticsSystemProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty diagnosticsWindowPrefab;
        private SerializedProperty showDiagnosticsWindowOnStart;

        private readonly GUIContent generalSettingsFoldoutHeader = new GUIContent("General Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            diagnosticsWindowPrefab = serializedObject.FindProperty(nameof(diagnosticsWindowPrefab));
            showDiagnosticsWindowOnStart = serializedObject.FindProperty(nameof(showDiagnosticsWindowOnStart));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Diagnostics monitors system resources and performance inside an application during development.");

            serializedObject.Update();

            if (diagnosticsWindowPrefab.FoldoutWithBoldLabelPropertyField(generalSettingsFoldoutHeader))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(showDiagnosticsWindowOnStart);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
