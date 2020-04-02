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

            var currentAction = new MixedRealityInputAction(Guid.Parse(profileGuid.stringValue), (uint)id.intValue, description.stringValue, (AxisType)axisConstraint.intValue);

            if (dropdownMenu == null)
            {
                dropdownMenu = new GenericMenu();
                dropdownMenu.AddItem(new GUIContent("None"), false, data => SetInputAction(MixedRealityInputAction.None), null);

                allInputActionProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityInputActionsProfile>();

                foreach (var inputActionProfile in allInputActionProfiles)
                {
                    foreach (var inputAction in inputActionProfile.InputActions)
                    {
                        // Upgrade old action references
                        if (id.intValue != 0 && profileGuid.stringValue == DefaultGuidString)
                        {
                            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(inputActionProfile, out var guid, out long _);
                            var upgradedAction = new MixedRealityInputAction(Guid.Parse(guid), currentAction.Id, currentAction.Description, currentAction.AxisConstraint);
                            SetInputAction(upgradedAction);
                            AddInputActionItem(inputAction, upgradedAction);
                        }
                        else
                        {
                            AddInputActionItem(inputAction, currentAction);
                        }

                        void AddInputActionItem(MixedRealityInputAction actionItem, MixedRealityInputAction selectedItem)
                        {
                            dropdownMenu.AddItem(
                                new GUIContent(inputAction.Description),
                                actionItem.ProfileGuid == selectedItem.ProfileGuid && actionItem.Id == selectedItem.Id,
                                data => SetInputAction(actionItem),
                                null);
                        }
                    }
                }

                void SetInputAction(MixedRealityInputAction inputAction)
                {
                    id.intValue = (int)inputAction.Id;
                    description.stringValue = inputAction.Description;
                    axisConstraint.intValue = (int)inputAction.AxisConstraint;
                    profileGuid.stringValue = inputAction.ProfileGuid.ToString("N");
                    property.serializedObject.ApplyModifiedProperties();
                    GUI.changed = currentAction != inputAction;
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
