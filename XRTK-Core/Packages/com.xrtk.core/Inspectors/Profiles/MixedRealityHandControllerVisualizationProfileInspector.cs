// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityHandControllerVisualizationProfile))]
    public class MixedRealityHandControllerVisualizationProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty jointPrefab;
        private SerializedProperty palmPrefab;
        private SerializedProperty fingertipPrefab;
        private SerializedProperty handMeshPrefab;

        private SerializedProperty enableHandMeshVisualization;
        private SerializedProperty enableHandJointVisualization;

        protected override void OnEnable()
        {
            base.OnEnable();

            enableHandJointVisualization = serializedObject.FindProperty("enableHandJointVisualization");
            jointPrefab = serializedObject.FindProperty("jointPrefab");
            fingertipPrefab = serializedObject.FindProperty("fingertipPrefab");
            palmPrefab = serializedObject.FindProperty("palmPrefab");

            enableHandMeshVisualization = serializedObject.FindProperty("enableHandMeshVisualization");
            handMeshPrefab = serializedObject.FindProperty("handMeshPrefab");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Visualization Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.PropertyField(enableHandJointVisualization);
            EditorGUILayout.PropertyField(jointPrefab);
            EditorGUILayout.PropertyField(palmPrefab);
            EditorGUILayout.PropertyField(fingertipPrefab);

            EditorGUILayout.PropertyField(enableHandMeshVisualization);
            EditorGUILayout.PropertyField(handMeshPrefab);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Virtual Hand Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Not available yet. Here you will find settings for VR hands once available.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
