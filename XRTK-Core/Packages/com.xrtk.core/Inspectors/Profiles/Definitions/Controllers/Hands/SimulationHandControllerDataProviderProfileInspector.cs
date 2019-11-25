// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands.UnityEditor;
using XRTK.Inspectors.Profiles;
using XRTK.Inspectors.Utilities;

namespace XRTK.Definitions.Controllers.Hands.Inspectors.Profiles
{
    [CustomEditor(typeof(SimulationHandControllerDataProviderProfile))]
    public class SimulationHandControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty isSimulateHandTrackingEnabled;

        private SerializedProperty toggleLeftHandKey;
        private SerializedProperty toggleRightHandKey;
        private SerializedProperty handHideTimeout;
        private SerializedProperty leftHandTrackedKey;
        private SerializedProperty rightHandTrackedKey;

        private SerializedProperty poseDefinitions;
        private SerializedProperty handGestureAnimationSpeed;

        private SerializedProperty defaultHandDistance;
        private SerializedProperty handDepthMultiplier;
        private SerializedProperty handJitterAmount;

        private SerializedProperty yawHandCWKey;
        private SerializedProperty yawHandCCWKey;
        private SerializedProperty pitchHandCWKey;
        private SerializedProperty pitchHandCCWKey;
        private SerializedProperty rollHandCWKey;
        private SerializedProperty rollHandCCWKey;
        private SerializedProperty handRotationSpeed;

        private SerializedProperty holdStartDuration;
        private SerializedProperty manipulationStartThreshold;

        protected override void OnEnable()
        {
            base.OnEnable();

            isSimulateHandTrackingEnabled = serializedObject.FindProperty("isSimulateHandTrackingEnabled");

            toggleLeftHandKey = serializedObject.FindProperty("toggleLeftHandKey");
            toggleRightHandKey = serializedObject.FindProperty("toggleRightHandKey");
            handHideTimeout = serializedObject.FindProperty("handHideTimeout");
            leftHandTrackedKey = serializedObject.FindProperty("leftHandTrackedKey");
            rightHandTrackedKey = serializedObject.FindProperty("rightHandTrackedKey");

            poseDefinitions = serializedObject.FindProperty("poseDefinitions");
            handGestureAnimationSpeed = serializedObject.FindProperty("handGestureAnimationSpeed");

            holdStartDuration = serializedObject.FindProperty("holdStartDuration");
            manipulationStartThreshold = serializedObject.FindProperty("manipulationStartThreshold");

            defaultHandDistance = serializedObject.FindProperty("defaultHandDistance");
            handDepthMultiplier = serializedObject.FindProperty("handDepthMultiplier");
            handJitterAmount = serializedObject.FindProperty("handJitterAmount");

            yawHandCWKey = serializedObject.FindProperty("yawHandCWKey");
            yawHandCCWKey = serializedObject.FindProperty("yawHandCCWKey");
            pitchHandCWKey = serializedObject.FindProperty("pitchHandCWKey");
            pitchHandCCWKey = serializedObject.FindProperty("pitchHandCCWKey");
            rollHandCWKey = serializedObject.FindProperty("rollHandCWKey");
            rollHandCCWKey = serializedObject.FindProperty("rollHandCCWKey");
            handRotationSpeed = serializedObject.FindProperty("handRotationSpeed");
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

            EditorGUILayout.PropertyField(isSimulateHandTrackingEnabled);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Label");

            EditorGUILayout.PropertyField(toggleLeftHandKey);
            EditorGUILayout.PropertyField(toggleRightHandKey);
            EditorGUILayout.PropertyField(handHideTimeout);
            EditorGUILayout.PropertyField(leftHandTrackedKey);
            EditorGUILayout.PropertyField(rightHandTrackedKey);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(poseDefinitions, true);
            EditorGUILayout.PropertyField(handGestureAnimationSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(holdStartDuration);
            EditorGUILayout.PropertyField(manipulationStartThreshold);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(defaultHandDistance);
            EditorGUILayout.PropertyField(handDepthMultiplier);
            EditorGUILayout.PropertyField(handJitterAmount);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(yawHandCWKey);
            EditorGUILayout.PropertyField(yawHandCCWKey);
            EditorGUILayout.PropertyField(pitchHandCWKey);
            EditorGUILayout.PropertyField(pitchHandCCWKey);
            EditorGUILayout.PropertyField(rollHandCWKey);
            EditorGUILayout.PropertyField(rollHandCCWKey);
            EditorGUILayout.PropertyField(handRotationSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}