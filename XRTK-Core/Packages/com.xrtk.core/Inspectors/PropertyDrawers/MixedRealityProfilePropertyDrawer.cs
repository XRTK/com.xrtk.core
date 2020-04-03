// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Extensions;
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
                    selectedProfile = CreateNewProfileInstance(property, profileType, hasSelection);
                }
            }

            if (!(selectedProfile is null) &&
                !(selectedProfile is MixedRealityToolkitRootProfile))
            {
                if (selectedProfile.ParentProfile == null ||
                    selectedProfile.ParentProfile != parent)
                {
                    if (parent != null)
                    {
                        Debug.Log($"Set parent: {parent.name}.{selectedProfile.name}");
                        selectedProfile.ParentProfile = parent;
                    }
                }

                Debug.Assert(selectedProfile.ParentProfile != null);
            }

            DrawCloneButtons = true;
            ProfileTypeOverride = null;
            EditorGUI.EndProperty();
        }

        private BaseMixedRealityProfile CreateNewProfileInstance(SerializedProperty property, Type profileType, bool clone)
        {
            ScriptableObject instance;

            if (profileType == null)
            {
                if (!string.IsNullOrWhiteSpace(property.type))
                {
                    var profileTypeName = property.type?.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    instance = ScriptableObject.CreateInstance(profileTypeName);
                }
                else
                {
                    Debug.LogError("No property type found!");
                    return null;
                }
            }
            else
            {
                instance = ScriptableObject.CreateInstance(profileType);
            }

            var assetPath = AssetDatabase.GetAssetPath(parent);
            var newProfile = instance.CreateAsset(assetPath) as BaseMixedRealityProfile;
            Debug.Assert(newProfile != null);

            if (clone &&
                property.objectReferenceValue != null)
            {
                var oldProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                newProfile.CopySerializedValues(oldProfile);
            }

            newProfile.ParentProfile = parent;
            property.objectReferenceValue = newProfile;
            return newProfile;
        }
    }
}