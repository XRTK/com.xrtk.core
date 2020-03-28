// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Inspectors.Utilities;

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
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (ThisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = ThisProfile.ParentProfile;
            }

            ThisProfile.CheckProfileLock();
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diagnostic System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Diagnostic can help monitor system resources and performance inside an application during development.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(diagnosticsWindowPrefab);
            EditorGUILayout.PropertyField(showDiagnosticsWindowOnStart);
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
