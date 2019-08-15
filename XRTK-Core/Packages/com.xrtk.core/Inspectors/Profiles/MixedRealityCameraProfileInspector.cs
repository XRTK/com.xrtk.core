// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityCameraProfile))]
    public class MixedRealityCameraProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty isCameraPersistent;
        private SerializedProperty opaqueNearClip;
        private SerializedProperty opaqueClearFlags;
        private SerializedProperty opaqueBackgroundColor;
        private SerializedProperty opaqueQualityLevel;

        private SerializedProperty transparentNearClip;
        private SerializedProperty transparentClearFlags;
        private SerializedProperty transparentBackgroundColor;
        private SerializedProperty transparentQualityLevel;

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");

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

            isCameraPersistent = serializedObject.FindProperty("isCameraPersistent");
            opaqueNearClip = serializedObject.FindProperty("nearClipPlaneOpaqueDisplay");
            opaqueClearFlags = serializedObject.FindProperty("cameraClearFlagsOpaqueDisplay");
            opaqueBackgroundColor = serializedObject.FindProperty("backgroundColorOpaqueDisplay");
            opaqueQualityLevel = serializedObject.FindProperty("opaqueQualityLevel");

            transparentNearClip = serializedObject.FindProperty("nearClipPlaneTransparentDisplay");
            transparentClearFlags = serializedObject.FindProperty("cameraClearFlagsTransparentDisplay");
            transparentBackgroundColor = serializedObject.FindProperty("backgroundColorTransparentDisplay");
            transparentQualityLevel = serializedObject.FindProperty("transparentQualityLevel");

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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Camera Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Camera Profile helps tweak camera settings no matter what platform you're building for.", MessageType.Info);

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isCameraPersistent);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Opaque Display Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
            EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);

            if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
            {
                opaqueBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", opaqueBackgroundColor.colorValue);
            }

            opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transparent Display Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(transparentNearClip, nearClipTitle);
            EditorGUILayout.PropertyField(transparentClearFlags, clearFlagsTitle);

            if ((CameraClearFlags)transparentClearFlags.intValue == CameraClearFlags.Color)
            {
                transparentBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", transparentBackgroundColor.colorValue);
            }

            transparentQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", transparentQualityLevel.intValue, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Controls Settings:", EditorStyles.boldLabel);
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