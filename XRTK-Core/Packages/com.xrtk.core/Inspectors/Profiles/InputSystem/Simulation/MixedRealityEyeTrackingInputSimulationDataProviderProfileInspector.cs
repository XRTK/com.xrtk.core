// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem.Simulation;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles.InputSystem.Simulation
{
    [CustomEditor(typeof(MixedRealityEyeTrackingInputSimulationDataProviderProfile))]
    public class MixedRealityEyeTrackingInputSimulationDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty simulateEyePosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            simulateEyePosition = serializedObject.FindProperty("simulateEyePosition");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.PropertyField(simulateEyePosition);

            serializedObject.ApplyModifiedProperties();
        }
    }
}