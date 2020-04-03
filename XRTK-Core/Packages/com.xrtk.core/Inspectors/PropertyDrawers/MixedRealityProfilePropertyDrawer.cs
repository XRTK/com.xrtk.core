// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(BaseMixedRealityProfile), true)]
    public class MixedRealityProfilePropertyDrawer : PropertyDrawer
    {
        private const int BUTTON_PADDING = 4;

        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        private static readonly GUIContent CloneProfileContent = new GUIContent("Clone", "Replace with a copy of the default profile.");

        public static bool DrawCloneButtons { get; set; } = true;

        public static Type ProfileTypeOverride { get; set; } = null;

        public static BaseMixedRealityProfile ParentProfileOverride { get; set; } = null;

        private BaseMixedRealityProfile parent = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BaseMixedRealityProfile profile = null;

            if (parent == null)
            {
                parent = ParentProfileOverride;

                if (parent == null)
                {
                    parent = Selection.activeObject as BaseMixedRealityProfile;
                }

                ParentProfileOverride = null;
            }

            if (property.objectReferenceValue != null)
            {
                profile = property.objectReferenceValue as BaseMixedRealityProfile;
            }

            Debug.Assert(profile == null || !(profile is MixedRealityToolkitRootProfile) && parent != null || profile is MixedRealityToolkitRootProfile && parent == null);

            var propertyLabel = EditorGUI.BeginProperty(position, label, property);
            var profileType = ProfileTypeOverride ?? fieldInfo.FieldType;
            var hasSelection = property.objectReferenceValue != null;
            var buttonWidth = hasSelection ? 42f : 20f;
            var objectRect = position;

            if (DrawCloneButtons)
            {
                objectRect.width -= buttonWidth + BUTTON_PADDING;
            }

            EditorGUI.BeginChangeCheck();

            var selectedProfile = EditorGUI.ObjectField(objectRect, propertyLabel, profile, profileType, false) as BaseMixedRealityProfile;

            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = selectedProfile;

                if (!(selectedProfile is null) &&
                    !(selectedProfile is MixedRealityToolkitRootProfile))
                {
                    Debug.Assert(parent != null, $"Failed to find a valid parent profile for {selectedProfile.name}");
                    selectedProfile.ParentProfile = parent;
                }
            }

            if (DrawCloneButtons)
            {
                hasSelection = selectedProfile != null;
                var buttonContent = hasSelection ? CloneProfileContent : NewProfileContent;
                var buttonRect = new Rect(objectRect.xMax + BUTTON_PADDING, position.y, buttonWidth, position.height);

                if (GUI.Button(buttonRect, buttonContent, EditorStyles.miniButton))
                {
                    selectedProfile = parent.CreateNewProfileInstance(property, profileType, hasSelection);
                }
            }

            if (!(selectedProfile is null) &&
                !(selectedProfile is MixedRealityToolkitRootProfile))
            {
                if (selectedProfile.ParentProfile == null ||
                    selectedProfile.ParentProfile != parent)
                {
                    if (parent != null &&
                        parent != selectedProfile)
                    {
                        selectedProfile.ParentProfile = parent;
                    }
                }

                Debug.Assert(selectedProfile.ParentProfile != null);
                Debug.Assert(selectedProfile.ParentProfile != selectedProfile);
            }

            DrawCloneButtons = true;
            ProfileTypeOverride = null;
            EditorGUI.EndProperty();
        }
    }
}