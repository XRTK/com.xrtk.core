// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using XRTK.Definitions.CameraSystem;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.CameraSystem
{
    [CustomEditor(typeof(MixedRealityCameraSystemProfile))]
    public class MixedRealityCameraSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private SerializedProperty globalCameraProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            globalCameraProfile = serializedObject.FindProperty(nameof(globalCameraProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The Camera Profile helps tweak camera settings no matter what platform you're building for.");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(globalCameraProfile);

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized && EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}