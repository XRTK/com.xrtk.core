// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty controllerVisualizationType;
        private SerializedProperty useDefaultModels;
        private SerializedProperty leftHandModel;
        private SerializedProperty rightHandModel;
        private SerializedProperty pointerPose;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        private float defaultLabelWidth;

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;

            controllerVisualizationProfile = target as MixedRealityControllerVisualizationProfile;

            controllerVisualizationType = serializedObject.FindProperty(nameof(controllerVisualizationType));
            useDefaultModels = serializedObject.FindProperty(nameof(useDefaultModels));
            leftHandModel = serializedObject.FindProperty(nameof(leftHandModel));
            rightHandModel = serializedObject.FindProperty(nameof(rightHandModel));
            pointerPose = serializedObject.FindProperty(nameof(pointerPose));
            controllerVisualizationSettings = serializedObject.FindProperty(nameof(controllerVisualizationSettings));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Controller Visualizations", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.\n\n" +
                                    "Global settings are the default fallback, and any specific controller definitions take precedence.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 168f;

            var leftHandModelPrefab = leftHandModel.objectReferenceValue as GameObject;
            var rightHandModelPrefab = rightHandModel.objectReferenceValue as GameObject;

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
            leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(leftHandModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

            if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
            {
                leftHandModel.objectReferenceValue = leftHandModelPrefab;
            }

            EditorGUI.BeginChangeCheck();
            rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(rightHandModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

            if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
            {
                rightHandModel.objectReferenceValue = rightHandModelPrefab;
            }

            EditorGUILayout.PropertyField(pointerPose);

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            serializedObject.ApplyModifiedProperties();
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
