// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Platform;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityPlatformSystemProfile))]
    public class MixedRealityPlatformSystemProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty platformConfigurations;

        protected override void OnEnable()
        {
            base.OnEnable();

            platformConfigurations = serializedObject.FindProperty("platformConfigurations");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Platform System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Platform System Profile helps developers define the platforms that their application can run on.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(platformConfigurations, true);

            if (EditorGUI.EndChangeCheck() && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}