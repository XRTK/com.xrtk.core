// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.DiagnosticsSystem;

namespace XRTK.Inspectors.Profiles.DiagnosticsSystem
{
    [CustomEditor(typeof(MixedRealityDiagnosticsSystemProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty diagnosticsWindowPrefab;
        private SerializedProperty showDiagnosticsWindowOnStart;

        protected override void OnEnable()
        {
            base.OnEnable();

            diagnosticsWindowPrefab = serializedObject.FindProperty(nameof(diagnosticsWindowPrefab));
            showDiagnosticsWindowOnStart = serializedObject.FindProperty(nameof(showDiagnosticsWindowOnStart));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Diagnostic System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Diagnostic can help monitor system resources and performance inside an application during development.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(diagnosticsWindowPrefab);
            EditorGUILayout.PropertyField(showDiagnosticsWindowOnStart);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
