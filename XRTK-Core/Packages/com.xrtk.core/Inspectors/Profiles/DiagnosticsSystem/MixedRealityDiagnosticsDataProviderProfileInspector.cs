// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles.DiagnosticsSystem
{
    [CustomEditor(typeof(MixedRealityDiagnosticsDataProviderProfile))]
    public class MixedRealityDiagnosticsDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty windowAnchor;
        private SerializedProperty windowOffset;
        private SerializedProperty windowScale;
        private SerializedProperty windowFollowSpeed;
        private SerializedProperty windowBackgroundColor;

        private static bool showProfilerSettings = true;
        private SerializedProperty showProfiler;
        private SerializedProperty frameSampleRate;
        private SerializedProperty displayedDecimalDigits;
        private SerializedProperty targetFrameRateColor;
        private SerializedProperty missedFrameRateColor;
        private SerializedProperty memoryUsedColor;
        private SerializedProperty memoryPeakColor;
        private SerializedProperty memoryLimitColor;

        private static bool showConsoleSettings = true;
        private SerializedProperty showConsole;

        protected override void OnEnable()
        {
            base.OnEnable();

            windowAnchor = serializedObject.FindProperty(nameof(windowAnchor));
            windowOffset = serializedObject.FindProperty(nameof(windowOffset));
            windowScale = serializedObject.FindProperty(nameof(windowScale));
            windowFollowSpeed = serializedObject.FindProperty(nameof(windowFollowSpeed));
            windowBackgroundColor = serializedObject.FindProperty(nameof(windowBackgroundColor));

            showProfiler = serializedObject.FindProperty(nameof(showProfiler));
            frameSampleRate = serializedObject.FindProperty(nameof(frameSampleRate));
            displayedDecimalDigits = serializedObject.FindProperty(nameof(displayedDecimalDigits));
            targetFrameRateColor = serializedObject.FindProperty(nameof(targetFrameRateColor));
            missedFrameRateColor = serializedObject.FindProperty(nameof(missedFrameRateColor));
            memoryUsedColor = serializedObject.FindProperty(nameof(memoryUsedColor));
            memoryPeakColor = serializedObject.FindProperty(nameof(memoryPeakColor));
            memoryLimitColor = serializedObject.FindProperty(nameof(memoryLimitColor));

            showConsole = serializedObject.FindProperty(nameof(showConsole));
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(windowAnchor);
            EditorGUILayout.PropertyField(windowOffset);
            EditorGUILayout.PropertyField(windowScale);
            EditorGUILayout.PropertyField(windowFollowSpeed);
            EditorGUILayout.PropertyField(windowBackgroundColor);

            EditorGUILayout.Space();
            showProfilerSettings = EditorGUILayout.Foldout(showProfilerSettings, "Profiler Settings", true);
            if (showProfilerSettings)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showProfiler);
                    EditorGUILayout.PropertyField(frameSampleRate);
                    EditorGUILayout.PropertyField(displayedDecimalDigits);
                    EditorGUILayout.PropertyField(targetFrameRateColor);
                    EditorGUILayout.PropertyField(missedFrameRateColor);
                    EditorGUILayout.PropertyField(memoryUsedColor);
                    EditorGUILayout.PropertyField(memoryPeakColor);
                    EditorGUILayout.PropertyField(memoryLimitColor);
                }
            }

            EditorGUILayout.Space();
            showConsoleSettings = EditorGUILayout.Foldout(showConsoleSettings, "Console Settings", true);
            if (showConsoleSettings)
            {
                EditorGUILayout.PropertyField(showConsole);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}