// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Extensions;

namespace XRTK.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MixedRealityInputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private static readonly string DefaultGuidString = default(Guid).ToString("N");
        private static readonly Tuple<MixedRealityInputActionsProfile, GUIContent, MixedRealityInputAction> DefaultNoneEntry = new Tuple<MixedRealityInputActionsProfile, GUIContent, MixedRealityInputAction>(null, new GUIContent("None"), MixedRealityInputAction.None);

        private GenericMenu dropdownMenu;
        private MixedRealityInputActionsProfile[] allInputActionProfiles;

        private SerializedProperty id;
        private SerializedProperty profileGuid;
        private SerializedProperty description;
        private SerializedProperty axisConstraint;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            id = property.FindPropertyRelative(nameof(id));
            profileGuid = property.FindPropertyRelative(nameof(profileGuid));
            description = property.FindPropertyRelative(nameof(description));
            axisConstraint = property.FindPropertyRelative(nameof(axisConstraint));
            var prevAction = new MixedRealityInputAction(Guid.Parse(profileGuid.stringValue), (uint)id.intValue, description.stringValue, (AxisType)axisConstraint.intValue);

            if (dropdownMenu == null)
            {
                dropdownMenu = new GenericMenu();
                dropdownMenu.AddItem(new GUIContent("None"), false, data => SetInputAction(MixedRealityInputAction.None), null);

                allInputActionProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityInputActionsProfile>();

                foreach (var inputActionProfile in allInputActionProfiles)
                {
                    foreach (var inputAction in inputActionProfile.InputActions)
                    {
                        dropdownMenu.AddItem(
                            new GUIContent(inputAction.Description),
                            inputAction.ProfileGuid == prevAction.ProfileGuid && inputAction.Id == prevAction.Id,
                            data => SetInputAction(inputAction),
                            null);
                    }
                }

                void SetInputAction(MixedRealityInputAction action)
                {
                    id.intValue = (int)action.Id;
                    description.stringValue = action.Description;
                    axisConstraint.intValue = (int)action.AxisConstraint;
                    profileGuid.stringValue = action.ProfileGuid.ToString("N");
                    property.serializedObject.ApplyModifiedProperties();
                    GUI.changed = prevAction != action;
                }
            }

            var label = EditorGUI.BeginProperty(rect, content, property);
            var prefix = EditorGUI.PrefixLabel(rect, label);

            if (EditorGUI.DropdownButton(prefix, new GUIContent(description.stringValue), FocusType.Passive))
            {
                dropdownMenu.DropDown(prefix);
            }

            EditorGUI.EndProperty();
        }
    }
}
