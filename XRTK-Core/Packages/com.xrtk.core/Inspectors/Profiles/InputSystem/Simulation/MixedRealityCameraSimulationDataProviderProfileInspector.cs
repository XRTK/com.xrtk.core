// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles.InputSystem.Simulation
{
    [CustomEditor(typeof(CameraSimulationDataProviderProfile))]
    public class MixedRealityCameraSimulationDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty isCameraControlEnabled;

        private SerializedProperty extraMouseSensitivityScale;
        private SerializedProperty defaultMouseSensitivity;
        private SerializedProperty mouseLookButton;
        private SerializedProperty isControllerLookInverted;
        private SerializedProperty currentControlMode;
        private SerializedProperty fastControlKey;
        private SerializedProperty controlSlowSpeed;
        private SerializedProperty controlFastSpeed;
        private SerializedProperty moveHorizontal;
        private SerializedProperty moveVertical;
        private SerializedProperty mouseX;
        private SerializedProperty mouseY;
        private SerializedProperty lookHorizontal;
        private SerializedProperty lookVertical;

        protected override void OnEnable()
        {
            base.OnEnable();

            isCameraControlEnabled = serializedObject.FindProperty("isCameraControlEnabled");

            extraMouseSensitivityScale = serializedObject.FindProperty("extraMouseSensitivityScale");
            defaultMouseSensitivity = serializedObject.FindProperty("defaultMouseSensitivity");
            mouseLookButton = serializedObject.FindProperty("mouseLookButton");
            isControllerLookInverted = serializedObject.FindProperty("isControllerLookInverted");
            currentControlMode = serializedObject.FindProperty("currentControlMode");
            fastControlKey = serializedObject.FindProperty("fastControlKey");
            controlSlowSpeed = serializedObject.FindProperty("controlSlowSpeed");
            controlFastSpeed = serializedObject.FindProperty("controlFastSpeed");
            moveHorizontal = serializedObject.FindProperty("moveHorizontal");
            moveVertical = serializedObject.FindProperty("moveVertical");
            mouseX = serializedObject.FindProperty("mouseX");
            mouseY = serializedObject.FindProperty("mouseY");
            lookHorizontal = serializedObject.FindProperty("lookHorizontal");
            lookVertical = serializedObject.FindProperty("lookVertical");
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

            EditorGUILayout.PropertyField(isCameraControlEnabled);
            {
                EditorGUILayout.BeginVertical("Label");

                EditorGUILayout.PropertyField(extraMouseSensitivityScale);
                EditorGUILayout.PropertyField(defaultMouseSensitivity);
                EditorGUILayout.PropertyField(mouseLookButton);
                EditorGUILayout.PropertyField(isControllerLookInverted);
                EditorGUILayout.PropertyField(currentControlMode);
                EditorGUILayout.PropertyField(fastControlKey);
                EditorGUILayout.PropertyField(controlSlowSpeed);
                EditorGUILayout.PropertyField(controlFastSpeed);
                EditorGUILayout.PropertyField(moveHorizontal);
                EditorGUILayout.PropertyField(moveVertical);
                EditorGUILayout.PropertyField(mouseX);
                EditorGUILayout.PropertyField(mouseY);
                EditorGUILayout.PropertyField(lookHorizontal);
                EditorGUILayout.PropertyField(lookVertical);

                EditorGUILayout.EndVertical();

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}