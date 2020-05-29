// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Editor.Extensions;
using XRTK.Editor.Profiles;
using XRTK.Extensions;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Editor.Data.Controllers.Hands
{
    [CustomEditor(typeof(HandControllerPoseDefinition))]
    public class HandControllerPoseDefinitionInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty id;
        private SerializedProperty description;
        private SerializedProperty isDefault;
        private SerializedProperty keyCode;
        private SerializedProperty data;
        private SerializedProperty bakedHandData;

        private static readonly GUIContent generalSettingsHeader = new GUIContent("General Settings");
        private static readonly GUIContent simulationSettingsHeader = new GUIContent("Simulation Settings");
        private static readonly GUIContent bakeSettingsHeader = new GUIContent("Bake Settings");
        private static readonly GUIContent debugHeader = new GUIContent("Debug");
        private static readonly GUIContent bakeButtonContent = new GUIContent("Bake");
        private bool bakeSettingsExpanded = true;
        private bool debugExpanded = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            id = serializedObject.FindProperty(nameof(id));
            description = serializedObject.FindProperty(nameof(description));
            isDefault = serializedObject.FindProperty(nameof(isDefault));
            keyCode = serializedObject.FindProperty(nameof(keyCode));
            data = serializedObject.FindProperty(nameof(data));
            bakedHandData = serializedObject.FindProperty(nameof(bakedHandData));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines a hand pose that can be recognized at runtime and trigger input events.");

            serializedObject.Update();

            description.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(description.isExpanded, generalSettingsHeader);
            if (description.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(description);
                EditorGUILayout.PropertyField(data);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            isDefault.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(isDefault.isExpanded, simulationSettingsHeader);
            if (isDefault.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(isDefault);
                EditorGUILayout.PropertyField(keyCode);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            bakeSettingsExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(bakeSettingsExpanded, bakeSettingsHeader);
            if (bakeSettingsExpanded)
            {
                var poseDefinition = target as HandControllerPoseDefinition;

                EditorGUILayout.HelpBox($"In order for the pose to be recognized when running your application you need to bake it," +
                    $" which will precompute important data and save performance.", MessageType.Info);
                if (GUILayout.Button(bakeButtonContent))
                {
                    BakePoseData(poseDefinition);
                }

                var empty = poseDefinition.BakedHandData.IsEmpty;
                var status = poseDefinition.BakedHandData != null && !empty ? "Complete" : "Not baked";
                EditorGUILayout.LabelField($"Bake Status: {status}");
            }

            EditorGUILayout.Space();

            debugExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(debugExpanded, debugHeader);
            if (debugExpanded)
            {
                EditorGUI.indentLevel++;

                if (target is HandControllerPoseDefinition poseDefinition && poseDefinition.BakedHandData != null && !poseDefinition.BakedHandData.IsEmpty)
                {
                    EditorGUILayout.LabelField($"Baked Grip Strength:\t\t{poseDefinition.BakedHandData.GripStrength}");
                    EditorGUILayout.LabelField($"Baked Thumb Curl:\t\t{poseDefinition.BakedHandData.FingerCurlStrengths[(int)HandFinger.Thumb]}");
                    EditorGUILayout.LabelField($"Baked Index Curl:\t\t{poseDefinition.BakedHandData.FingerCurlStrengths[(int)HandFinger.Index]}");
                    EditorGUILayout.LabelField($"Baked Middle Curl:\t\t{poseDefinition.BakedHandData.FingerCurlStrengths[(int)HandFinger.Middle]}");
                    EditorGUILayout.LabelField($"Baked Ring Curl:\t\t{poseDefinition.BakedHandData.FingerCurlStrengths[(int)HandFinger.Ring]}");
                    EditorGUILayout.LabelField($"Baked Little Curl:\t\t{poseDefinition.BakedHandData.FingerCurlStrengths[(int)HandFinger.Little]}");
                    EditorGUILayout.LabelField($"Baked Is Gripping:\t\t{poseDefinition.BakedHandData.IsGripping}");
                }
                else
                {
                    EditorGUILayout.HelpBox("Bake the hand pose to see debug information here.", MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }
        }

        private void BakePoseData(HandControllerPoseDefinition poseDefinition)
        {
            // Initialize the hand data using the joint information from the recorded pose.
            var handData = poseDefinition.ToHandData();

            // Intialize processors needed.
            var gripPostProcessor = new HandGripPostProcessor();

            // Process the hand data.
            handData.IsTracked = true;
            gripPostProcessor.Process(handData);
            handData.IsTracked = false;

            // Save the baked hand data to the asset.
            serializedObject.Update();
            poseDefinition.BakedHandData = handData;
            serializedObject.ApplyModifiedProperties();
        }
    }
}