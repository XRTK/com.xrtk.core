// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedControllerDataProviderProfile))]
    public class SimulatedControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty controllerSimulationEnabled;
        private SerializedProperty simulatedControllerType;
        private SerializedProperty simulatedUpdateFrequency;
        private SerializedProperty controllerHideTimeout;

        private SerializedProperty defaultDistance;
        private SerializedProperty depthMultiplier;
        private SerializedProperty jitterAmount;

        private SerializedProperty toggleLeftPersistentKey;
        private SerializedProperty leftControllerTrackedKey;
        private SerializedProperty toggleRightPersistentKey;
        private SerializedProperty rightControllerTrackedKey;

        private SerializedProperty poseDefinitions;
        private SerializedProperty handPoseAnimationSpeed;


        private SerializedProperty yawCWKey;
        private SerializedProperty yawCCWKey;
        private SerializedProperty pitchCWKey;
        private SerializedProperty pitchCCWKey;
        private SerializedProperty rollCWKey;
        private SerializedProperty rollCCWKey;
        private SerializedProperty rotationSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerSimulationEnabled = serializedObject.FindProperty(nameof(controllerSimulationEnabled));
            simulatedControllerType = serializedObject.FindProperty(nameof(simulatedControllerType));
            simulatedUpdateFrequency = serializedObject.FindProperty(nameof(simulatedUpdateFrequency));
            controllerHideTimeout = serializedObject.FindProperty(nameof(controllerHideTimeout));

            defaultDistance = serializedObject.FindProperty(nameof(defaultDistance));
            depthMultiplier = serializedObject.FindProperty(nameof(depthMultiplier));
            jitterAmount = serializedObject.FindProperty(nameof(jitterAmount));

            toggleLeftPersistentKey = serializedObject.FindProperty(nameof(toggleLeftPersistentKey));
            toggleRightPersistentKey = serializedObject.FindProperty(nameof(toggleRightPersistentKey));
            leftControllerTrackedKey = serializedObject.FindProperty(nameof(leftControllerTrackedKey));
            rightControllerTrackedKey = serializedObject.FindProperty(nameof(rightControllerTrackedKey));

            yawCWKey = serializedObject.FindProperty(nameof(yawCWKey));
            yawCCWKey = serializedObject.FindProperty(nameof(yawCCWKey));
            pitchCWKey = serializedObject.FindProperty(nameof(pitchCWKey));
            pitchCCWKey = serializedObject.FindProperty(nameof(pitchCCWKey));
            rollCWKey = serializedObject.FindProperty(nameof(rollCWKey));
            rollCCWKey = serializedObject.FindProperty(nameof(rollCCWKey));
            rotationSpeed = serializedObject.FindProperty(nameof(rotationSpeed));

            poseDefinitions = serializedObject.FindProperty(nameof(poseDefinitions));
            handPoseAnimationSpeed = serializedObject.FindProperty(nameof(handPoseAnimationSpeed));
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
            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.BeginVertical("Label");

            EditorGUILayout.PropertyField(controllerSimulationEnabled);
            EditorGUILayout.PropertyField(simulatedControllerType);
            EditorGUILayout.PropertyField(simulatedUpdateFrequency);
            EditorGUILayout.PropertyField(controllerHideTimeout);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(defaultDistance);
            EditorGUILayout.PropertyField(depthMultiplier);
            EditorGUILayout.PropertyField(jitterAmount);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(toggleLeftPersistentKey);
            EditorGUILayout.PropertyField(leftControllerTrackedKey);
            EditorGUILayout.PropertyField(toggleRightPersistentKey);
            EditorGUILayout.PropertyField(rightControllerTrackedKey);
            EditorGUILayout.PropertyField(yawCWKey);
            EditorGUILayout.PropertyField(yawCCWKey);
            EditorGUILayout.PropertyField(pitchCWKey);
            EditorGUILayout.PropertyField(pitchCCWKey);
            EditorGUILayout.PropertyField(rollCWKey);
            EditorGUILayout.PropertyField(rollCCWKey);
            EditorGUILayout.PropertyField(rotationSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(poseDefinitions, true);
            EditorGUILayout.PropertyField(handPoseAnimationSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}