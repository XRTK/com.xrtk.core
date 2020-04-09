// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityInteractionMappingProfile))]
    public class MixedRealityInteractionMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty interactionMapping;
        private SerializedProperty pointerProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            interactionMapping = serializedObject.FindProperty(nameof(interactionMapping));
            pointerProfile = serializedObject.FindProperty(nameof(pointerProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("A distinct input pattern that can be recognized on a physical control mechanism. An Interaction only triggers an InputAction when the device's raw input matches the criteria defined by the Processor.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(interactionMapping, true);

            var axisType = (AxisType)interactionMapping.FindPropertyRelative("axisType").intValue;

            if (axisType == AxisType.ThreeDofPosition || axisType == AxisType.SixDof)
            {
                EditorGUILayout.PropertyField(pointerProfile, true);
            }
            else
            {
                if (pointerProfile.arraySize > 0)
                {
                    pointerProfile.ClearArray();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}