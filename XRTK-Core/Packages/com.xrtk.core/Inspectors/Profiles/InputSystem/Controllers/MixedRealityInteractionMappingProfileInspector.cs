// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.PropertyDrawers;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityInteractionMappingProfile))]
    public class MixedRealityInteractionMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty interactionMapping;
        private SerializedProperty pointerProfiles;

        private int currentlySelectedElement;

        private ReorderableList profileList;

        protected override void OnEnable()
        {
            base.OnEnable();

            interactionMapping = serializedObject.FindProperty(nameof(interactionMapping));
            pointerProfiles = serializedObject.FindProperty(nameof(pointerProfiles));

            profileList = new ReorderableList(serializedObject, pointerProfiles, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            profileList.drawElementCallback += DrawConfigurationOptionElement;
            profileList.onAddCallback += OnConfigurationOptionAdded;
            profileList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("A distinct input pattern that can be recognized on a physical control mechanism. An Interaction only triggers an InputAction when the device's raw input matches the criteria defined by the Processor.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(interactionMapping, true);

            var axisType = (AxisType)interactionMapping.FindPropertyRelative("axisType").intValue;

            if (axisType == AxisType.ThreeDofPosition || axisType == AxisType.SixDof)
            {
                profileList.DoLayoutList();
            }
            else
            {
                if (pointerProfiles.arraySize > 0)
                {
                    pointerProfiles.ClearArray();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedElement = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var mappingProfileProperty = pointerProfiles.GetArrayElementAtIndex(index);
            MixedRealityProfilePropertyDrawer.ProfileTypeOverride = typeof(MixedRealityPointerProfile);
            EditorGUI.PropertyField(rect, mappingProfileProperty, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            pointerProfiles.arraySize += 1;
            var index = pointerProfiles.arraySize - 1;

            var mappingProfileProperty = pointerProfiles.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedElement >= 0)
            {
                pointerProfiles.DeleteArrayElementAtIndex(currentlySelectedElement);
            }
        }
    }
}