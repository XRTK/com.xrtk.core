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
        private SerializedProperty frameSampleRate;
        private SerializedProperty targetFrameRateColor;
        private SerializedProperty missedFrameRateColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            frameSampleRate = serializedObject.FindProperty(nameof(frameSampleRate));
            targetFrameRateColor = serializedObject.FindProperty(nameof(targetFrameRateColor));
            missedFrameRateColor = serializedObject.FindProperty(nameof(missedFrameRateColor));
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

            EditorGUILayout.PropertyField(frameSampleRate);
            EditorGUILayout.PropertyField(targetFrameRateColor);
            EditorGUILayout.PropertyField(missedFrameRateColor);

            serializedObject.ApplyModifiedProperties();
        }
    }
}