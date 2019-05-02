// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions;

namespace XRTK.Inspectors.Utilities.Packages
{
    [CustomEditor(typeof(MixedRealityPackageSettings))]
    public class MixedRealityPackageSettingsInspector : Editor
    {
        private SerializedProperty mixedRealityPackages;

        private void OnEnable()
        {
            mixedRealityPackages = serializedObject.FindProperty("mixedRealityPackages");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            GUI.enabled = !MixedRealityPreferences.LockProfiles;
            serializedObject.Update();
            EditorGUILayout.PropertyField(mixedRealityPackages, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
