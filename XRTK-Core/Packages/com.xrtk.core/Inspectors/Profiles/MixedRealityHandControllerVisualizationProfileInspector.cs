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

            jointPrefab = serializedObject.FindProperty("jointPrefab");
            fingertipPrefab = serializedObject.FindProperty("fingertipPrefab");
            palmPrefab = serializedObject.FindProperty("palmPrefab");
            handMeshPrefab = serializedObject.FindProperty("handMeshPrefab");
            enableHandMeshVisualization = serializedObject.FindProperty("enableHandMeshVisualization");
            enableHandJointVisualization = serializedObject.FindProperty("enableHandJointVisualization");
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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tracked Hand Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(jointPrefab);
            EditorGUILayout.PropertyField(palmPrefab);
            EditorGUILayout.PropertyField(fingertipPrefab);
            EditorGUILayout.PropertyField(handMeshPrefab);
            EditorGUILayout.PropertyField(enableHandMeshVisualization);
            EditorGUILayout.PropertyField(enableHandJointVisualization);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Virtual Hand Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Not available yet. Here you will find settings for VR hands once available.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
