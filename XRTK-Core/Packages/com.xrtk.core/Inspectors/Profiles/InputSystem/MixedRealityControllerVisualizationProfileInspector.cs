// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Inspectors.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers.UnityInput;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");

        private static readonly GUIContent[] HandednessSelections =
        {
            new GUIContent("Left Hand"),
            new GUIContent("Right Hand"),
            new GUIContent("Either Hand (Both)"),
        };

        private SerializedProperty renderMotionControllers;
        private SerializedProperty controllerVisualizationType;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;
        private SerializedProperty globalPointerPose;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        private float defaultLabelWidth;
        private float defaultFieldWidth;

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;

            controllerVisualizationProfile = target as MixedRealityControllerVisualizationProfile;

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            controllerVisualizationType = serializedObject.FindProperty("controllerVisualizationType");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandModel");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandModel");
            globalPointerPose = serializedObject.FindProperty("globalPointerPose");
            controllerVisualizationSettings = serializedObject.FindProperty("controllerVisualizationSettings");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = controllerVisualizationProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Visualizations", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.\n\n" +
                                    "Global settings are the default fallback, and any specific controller definitions take precedence.", MessageType.Info);

            serializedObject.Update();

            controllerVisualizationProfile.CheckProfileLock();

            EditorGUIUtility.labelWidth = 168f;
            EditorGUILayout.PropertyField(renderMotionControllers);

            if (renderMotionControllers.boolValue)
            {
                var leftHandModelPrefab = globalLeftHandModel.objectReferenceValue as GameObject;
                var rightHandModelPrefab = globalRightHandModel.objectReferenceValue as GameObject;

                EditorGUILayout.PropertyField(controllerVisualizationType);

                if (controllerVisualizationProfile.ControllerVisualizationType == null ||
                    controllerVisualizationProfile.ControllerVisualizationType.Type == null)
                {
                    EditorGUILayout.HelpBox("A controller visualization type must be defined!", MessageType.Error);
                }

                EditorGUILayout.PropertyField(useDefaultModels);

                if (useDefaultModels.boolValue && (leftHandModelPrefab != null || rightHandModelPrefab != null))
                {
                    EditorGUILayout.HelpBox("When default models are used, the global left and right hand models will only be used if the default models cannot be loaded from the driver.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalLeftHandModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
                {
                    globalLeftHandModel.objectReferenceValue = leftHandModelPrefab;
                }

                EditorGUI.BeginChangeCheck();
                rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalRightHandModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
                {
                    globalRightHandModel.objectReferenceValue = rightHandModelPrefab;
                }

                EditorGUILayout.PropertyField(globalPointerPose);

                EditorGUIUtility.labelWidth = defaultLabelWidth;

                RenderControllerList(controllerVisualizationSettings);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (controllerVisualizationProfile.ControllerVisualizationSettings.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
                var index = controllerList.arraySize - 1;
                var controllerSetting = controllerList.GetArrayElementAtIndex(index);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = typeof(GenericJoystickController).Name;
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                mixedRealityControllerHandedness.intValue = 1;
                serializedObject.ApplyModifiedProperties();
                controllerVisualizationProfile.ControllerVisualizationSettings[index].ControllerType.Type = typeof(GenericJoystickController);
                return;
            }

            for (int i = 0; i < controllerList.arraySize; i++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 64f;
                EditorGUIUtility.fieldWidth = 64f;
                var controllerSetting = controllerList.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                bool hasValidType = controllerVisualizationProfile.ControllerVisualizationSettings[i].ControllerType != null &&
                                    controllerVisualizationProfile.ControllerVisualizationSettings[i].ControllerType.Type != null;

                mixedRealityControllerMappingDescription.stringValue = hasValidType
                    ? controllerVisualizationProfile.ControllerVisualizationSettings[i].ControllerType.Type.Name.ToProperCase()
                    : "Undefined Controller";

                serializedObject.ApplyModifiedProperties();
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                EditorGUILayout.LabelField($"{mixedRealityControllerMappingDescription.stringValue} {((Handedness)mixedRealityControllerHandedness.intValue).ToString().ToProperCase()} Hand");

                EditorGUIUtility.fieldWidth = defaultFieldWidth;
                EditorGUIUtility.labelWidth = defaultLabelWidth;

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerList.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerType"));
                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerVisualizationType"));

                if (!hasValidType)
                {
                    EditorGUILayout.HelpBox("A controller type must be defined!", MessageType.Error);
                }

                var handednessValue = mixedRealityControllerHandedness.intValue - 1;
                
                // Reset in case it was set to something other than left, right or both.
                if (handednessValue < 0 || handednessValue > 2) { handednessValue = 0; }

                EditorGUI.BeginChangeCheck();
                handednessValue = EditorGUILayout.IntPopup(new GUIContent(mixedRealityControllerHandedness.displayName, mixedRealityControllerHandedness.tooltip), handednessValue, HandednessSelections, null);

                if (EditorGUI.EndChangeCheck())
                {
                    mixedRealityControllerHandedness.intValue = handednessValue + 1;
                }

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("poseAction"));
                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("alternatePoseAction"));

                var overrideModel = controllerSetting.FindPropertyRelative("overrideModel");
                var overrideModelPrefab = overrideModel.objectReferenceValue as GameObject;

                var controllerUseDefaultModelOverride = controllerSetting.FindPropertyRelative("useDefaultModel");
                EditorGUILayout.PropertyField(controllerUseDefaultModelOverride);

                if (controllerUseDefaultModelOverride.boolValue && overrideModelPrefab != null)
                {
                    EditorGUILayout.HelpBox("When default model is used, the override model will only be used if the default model cannot be loaded from the driver.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                overrideModelPrefab = EditorGUILayout.ObjectField(new GUIContent(overrideModel.displayName, "If no override model is set, the global model is used."), overrideModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(overrideModelPrefab))
                {
                    overrideModel.objectReferenceValue = overrideModelPrefab;
                }

                EditorGUI.indentLevel--;
            }
        }

        private bool CheckVisualizer(GameObject modelPrefab)
        {
            if (modelPrefab == null) { return true; }

            if (PrefabUtility.GetPrefabAssetType(modelPrefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogWarning("Assigned GameObject must be a prefab");
                return false;
            }

            var componentList = modelPrefab.GetComponentsInChildren<IMixedRealityControllerVisualizer>();

            if (componentList == null || componentList.Length == 0)
            {
                if (controllerVisualizationProfile.ControllerVisualizationType != null &&
                    controllerVisualizationProfile.ControllerVisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(controllerVisualizationProfile.ControllerVisualizationType.Type);
                    return true;
                }

                Debug.LogError("No controller visualization type specified!");
            }
            else if (componentList.Length == 1)
            {
                return true;
            }
            else if (componentList.Length > 1)
            {
                Debug.LogWarning("Found too many IMixedRealityControllerVisualizer components on your prefab. There can only be one.");
            }

            return false;
        }
    }
}
