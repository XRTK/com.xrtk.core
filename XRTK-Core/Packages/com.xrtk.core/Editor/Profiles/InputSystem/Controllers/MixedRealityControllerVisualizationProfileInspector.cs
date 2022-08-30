// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Interfaces.InputSystem.Handlers;

namespace XRTK.Editor.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty controllerVisualizationType;
        private SerializedProperty useDefaultModels;
        private SerializedProperty controllerModel;
        private SerializedProperty pointerPose;
        private SerializedProperty alternatePointerPose;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerVisualizationProfile = target as MixedRealityControllerVisualizationProfile;

            controllerVisualizationType = serializedObject.FindProperty(nameof(controllerVisualizationType));
            useDefaultModels = serializedObject.FindProperty(nameof(useDefaultModels));
            controllerModel = serializedObject.FindProperty(nameof(controllerModel));
            pointerPose = serializedObject.FindProperty(nameof(pointerPose));
            alternatePointerPose = serializedObject.FindProperty(nameof(alternatePointerPose));
            controllerVisualizationSettings = serializedObject.FindProperty(nameof(controllerVisualizationSettings));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile controls how the controller should be rendered in the scene.");

            var defaultLabelWidth = EditorGUIUtility.labelWidth;

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 168f;

            var modelPrefab = controllerModel.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(controllerVisualizationType);

            if (controllerVisualizationProfile.ControllerVisualizationType == null ||
                controllerVisualizationProfile.ControllerVisualizationType.Type == null)
            {
                EditorGUILayout.HelpBox("A controller visualization type must be defined!", MessageType.Error);
            }

            EditorGUILayout.PropertyField(useDefaultModels);

            if (useDefaultModels.boolValue && modelPrefab != null)
            {
                EditorGUILayout.HelpBox("When default models are used, the global left and right hand models will only be used if the default models cannot be loaded from the driver.", MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            modelPrefab = EditorGUILayout.ObjectField(new GUIContent(controllerModel.displayName, "Note: If the default controllerModel is not found, the fallback is the global controllerModel."), modelPrefab, typeof(GameObject), false) as GameObject;

            if (EditorGUI.EndChangeCheck() && CheckVisualizer(modelPrefab))
            {
                controllerModel.objectReferenceValue = modelPrefab;
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(pointerPose);
            EditorGUILayout.PropertyField(alternatePointerPose);

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
