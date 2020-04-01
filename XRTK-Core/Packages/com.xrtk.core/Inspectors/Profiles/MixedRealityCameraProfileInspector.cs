// Copyright (c) XRTK. All rights reserved.
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
        private SerializedProperty nearClipPlaneOpaqueDisplay;
        private SerializedProperty cameraClearFlagsOpaqueDisplay;
        private SerializedProperty backgroundColorOpaqueDisplay;
        private SerializedProperty opaqueQualityLevel;

        private SerializedProperty nearClipPlaneTransparentDisplay;
        private SerializedProperty cameraClearFlagsTransparentDisplay;
        private SerializedProperty backgroundColorTransparentDisplay;
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

            isCameraPersistent = serializedObject.FindProperty(nameof(isCameraPersistent));
            nearClipPlaneOpaqueDisplay = serializedObject.FindProperty(nameof(nearClipPlaneOpaqueDisplay));
            cameraClearFlagsOpaqueDisplay = serializedObject.FindProperty(nameof(cameraClearFlagsOpaqueDisplay));
            backgroundColorOpaqueDisplay = serializedObject.FindProperty(nameof(backgroundColorOpaqueDisplay));
            opaqueQualityLevel = serializedObject.FindProperty(nameof(opaqueQualityLevel));

            nearClipPlaneTransparentDisplay = serializedObject.FindProperty(nameof(nearClipPlaneTransparentDisplay));
            cameraClearFlagsTransparentDisplay = serializedObject.FindProperty(nameof(cameraClearFlagsTransparentDisplay));
            backgroundColorTransparentDisplay = serializedObject.FindProperty(nameof(backgroundColorTransparentDisplay));
            transparentQualityLevel = serializedObject.FindProperty(nameof(transparentQualityLevel));

            cameraRigType = serializedObject.FindProperty(nameof(cameraRigType));
            defaultHeadHeight = serializedObject.FindProperty(nameof(defaultHeadHeight));
            bodyAdjustmentAngle = serializedObject.FindProperty(nameof(bodyAdjustmentAngle));
            bodyAdjustmentSpeed = serializedObject.FindProperty(nameof(bodyAdjustmentSpeed));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Camera Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Camera Profile helps tweak camera settings no matter what platform you're building for.", MessageType.Info);

            ThisProfile.CheckProfileLock();

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
            EditorGUILayout.PropertyField(nearClipPlaneOpaqueDisplay, nearClipTitle);
            EditorGUILayout.PropertyField(cameraClearFlagsOpaqueDisplay, clearFlagsTitle);

            if ((CameraClearFlags)cameraClearFlagsOpaqueDisplay.intValue == CameraClearFlags.Color)
            {
                backgroundColorOpaqueDisplay.colorValue = EditorGUILayout.ColorField("Background Color", backgroundColorOpaqueDisplay.colorValue);
            }

            opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transparent Display Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(nearClipPlaneTransparentDisplay, nearClipTitle);
            EditorGUILayout.PropertyField(cameraClearFlagsTransparentDisplay, clearFlagsTitle);

            if ((CameraClearFlags)cameraClearFlagsTransparentDisplay.intValue == CameraClearFlags.Color)
            {
                backgroundColorTransparentDisplay.colorValue = EditorGUILayout.ColorField("Background Color", backgroundColorTransparentDisplay.colorValue);
            }

            transparentQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", transparentQualityLevel.intValue, QualitySettings.names);

            serializedObject.ApplyModifiedProperties();

            if (MixedRealityToolkit.IsInitialized && EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}