// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

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

        private SerializedProperty cameraRigType;
        private SerializedProperty defaultHeadHeight;
        private SerializedProperty bodyAdjustmentAngle;
        private SerializedProperty bodyAdjustmentSpeed;

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");

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

            cameraRigType = serializedObject.FindProperty("cameraRigType");
            defaultHeadHeight = serializedObject.FindProperty("defaultHeadHeight");
            bodyAdjustmentAngle = serializedObject.FindProperty("bodyAdjustmentAngle");
            bodyAdjustmentSpeed = serializedObject.FindProperty("bodyAdjustmentSpeed");
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

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isCameraPersistent);
            EditorGUILayout.PropertyField(cameraRigType);
            EditorGUILayout.PropertyField(defaultHeadHeight);
            EditorGUILayout.PropertyField(bodyAdjustmentAngle);
            EditorGUILayout.PropertyField(bodyAdjustmentSpeed);

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

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized && EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}