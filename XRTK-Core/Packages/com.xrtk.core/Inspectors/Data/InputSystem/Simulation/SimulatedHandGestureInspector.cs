// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Inspectors.Profiles;
using XRTK.Inspectors.Utilities;
using XRTK.Services.InputSystem.Simulation;

namespace XRTK.Inspectors.Data.InputSystem.Simulation
{
    [CustomEditor(typeof(SimulatedHandGesture))]
    public class SimulatedHandGestureInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty gestureName;
        private SerializedProperty gestureDescription;
        private SerializedProperty isDefault;
        private SerializedProperty keyCode;
        private SerializedProperty data;

        protected override void OnEnable()
        {
            base.OnEnable();

            gestureName = serializedObject.FindProperty(nameof(gestureName));
            gestureDescription = serializedObject.FindProperty(nameof(gestureDescription));
            isDefault = serializedObject.FindProperty(nameof(isDefault));
            keyCode = serializedObject.FindProperty(nameof(keyCode));
            data = serializedObject.FindProperty(nameof(data));
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.PropertyField(gestureName);
            EditorGUILayout.PropertyField(gestureDescription);
            EditorGUILayout.PropertyField(isDefault);
            EditorGUILayout.PropertyField(keyCode);
            EditorGUILayout.PropertyField(data);

            serializedObject.ApplyModifiedProperties();
        }
    }
}