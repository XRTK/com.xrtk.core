// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Extensions;
using XRTK.Editor.Profiles;
using XRTK.Extensions;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Editor.Data.Controllers.Hands
{
    [CustomEditor(typeof(HandControllerPoseProfile))]
    public class HandControllerPoseProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty id;
        private SerializedProperty description;
        private SerializedProperty isDefault;
        private SerializedProperty keyCode;
        private SerializedProperty data;

        // Baked data.
        private SerializedProperty didBake;
        private SerializedProperty isGripping;
        private SerializedProperty fingerCurlStrengths;
        private SerializedProperty gripStrength;

        private static readonly GUIContent generalSettingsHeader = new GUIContent("General Settings");
        private static readonly GUIContent simulationSettingsHeader = new GUIContent("Simulation Settings");
        private static readonly GUIContent bakeSettingsHeader = new GUIContent("Bake Settings");
        private static readonly GUIContent infoHeader = new GUIContent("Info");
        private static readonly GUIContent bakeButtonContent = new GUIContent("Generate Baked Data");
        private static readonly GUIContent updateBakeButtonContent = new GUIContent("Update Baked Data");
        private bool infoExpanded = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            id = serializedObject.FindProperty(nameof(id));
            description = serializedObject.FindProperty(nameof(description));
            isDefault = serializedObject.FindProperty(nameof(isDefault));
            keyCode = serializedObject.FindProperty(nameof(keyCode));
            data = serializedObject.FindProperty(nameof(data));

            // Baked data.
            didBake = serializedObject.FindProperty(nameof(didBake));
            isGripping = serializedObject.FindProperty(nameof(isGripping));
            fingerCurlStrengths = serializedObject.FindProperty(nameof(fingerCurlStrengths));
            gripStrength = serializedObject.FindProperty(nameof(gripStrength));
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

            EditorGUILayout.Space();

            didBake.isExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(didBake.isExpanded, bakeSettingsHeader);
            if (didBake.isExpanded)
            {
                if (!didBake.boolValue)
                {
                    EditorGUILayout.HelpBox($"This hand pose hasn't been baked yet. In order for the pose to be recognized when running your application you need to bake it," +
                    $" which will precompute important data and save performance.", MessageType.Warning);
                }

                if (GUILayout.Button(didBake.boolValue ? updateBakeButtonContent : bakeButtonContent))
                {
                    BakePoseData();
                }
            }

            EditorGUILayout.Space();

            infoExpanded = EditorGUILayoutExtensions.FoldoutWithBoldLabel(infoExpanded, infoHeader);
            if (infoExpanded)
            {
                EditorGUI.indentLevel++;

                if (didBake.boolValue)
                {
                    EditorGUILayout.LabelField($"Baked Is Gripping:\t\t{isGripping.boolValue}");
                    EditorGUILayout.LabelField($"Baked Grip Strength:\t\t{gripStrength.floatValue}");
                    EditorGUILayout.LabelField($"Baked Thumb Curl:\t\t{fingerCurlStrengths.GetArrayElementAtIndex((int)HandFinger.Thumb).floatValue}");
                    EditorGUILayout.LabelField($"Baked Index Curl:\t\t{fingerCurlStrengths.GetArrayElementAtIndex((int)HandFinger.Index).floatValue}");
                    EditorGUILayout.LabelField($"Baked Middle Curl:\t\t{fingerCurlStrengths.GetArrayElementAtIndex((int)HandFinger.Middle).floatValue}");
                    EditorGUILayout.LabelField($"Baked Ring Curl:\t\t{fingerCurlStrengths.GetArrayElementAtIndex((int)HandFinger.Ring).floatValue}");
                    EditorGUILayout.LabelField($"Baked Little Curl:\t\t{fingerCurlStrengths.GetArrayElementAtIndex((int)HandFinger.Little).floatValue}");
                }
                else
                {
                    EditorGUILayout.HelpBox("Bake the hand pose to see baked information here.", MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void BakePoseData()
        {
            // Initialize the hand data using the joint information from the recorded pose.
            var poseDefinition = target as HandControllerPoseProfile;
            var handData = poseDefinition.ToHandData();

            // Intialize processors needed.
            var gripPostProcessor = new HandGripPostProcessor();

            // Process the hand data, most hand data processors
            // will ignore the hand data if it is not tracked, so we
            // have to temporarily fake it's tracking state and then reset it.
            handData.TrackingState = Definitions.Devices.TrackingState.Tracked;
            gripPostProcessor.PostProcess(Handedness.Right, handData);
            handData.TrackingState = Definitions.Devices.TrackingState.NotTracked;

            isGripping.boolValue = handData.IsGripping;
            gripStrength.floatValue = handData.GripStrength;
            fingerCurlStrengths.ClearArray();
            fingerCurlStrengths.arraySize = handData.FingerCurlStrengths.Length;
            for (int i = 0; i < handData.FingerCurlStrengths.Length; i++)
            {
                fingerCurlStrengths.GetArrayElementAtIndex(i).floatValue = handData.FingerCurlStrengths[i];
            }

            didBake.boolValue = true;
        }
    }
}