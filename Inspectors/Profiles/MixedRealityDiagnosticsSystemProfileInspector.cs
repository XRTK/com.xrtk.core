// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Diagnostics;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty visible;
        private SerializedProperty handlerType;
        private SerializedProperty showFps;
        private SerializedProperty fpsBuffer;
        private SerializedProperty showCpu;
        private SerializedProperty cpuBuffer;
        private SerializedProperty showMemory;
        private SerializedProperty memoryBuffer;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            visible = serializedObject.FindProperty("visible");
            handlerType = serializedObject.FindProperty("handlerType");
            showCpu = serializedObject.FindProperty("showCpu");
            cpuBuffer = serializedObject.FindProperty("cpuBuffer");
            showFps = serializedObject.FindProperty("showFps");
            fpsBuffer = serializedObject.FindProperty("fpsBuffer");
            showMemory = serializedObject.FindProperty("showMemory");
            memoryBuffer = serializedObject.FindProperty("memoryBuffer");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            (target as BaseMixedRealityProfile).CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diagnostic Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Diagnostic visualizations can help monitor system resources and performance inside an application.", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(visible);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(handlerType);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(showCpu);
            EditorGUILayout.PropertyField(cpuBuffer);
            EditorGUILayout.PropertyField(showFps);
            EditorGUILayout.PropertyField(fpsBuffer);
            EditorGUILayout.PropertyField(showMemory);
            EditorGUILayout.PropertyField(memoryBuffer);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
