// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.InputSystem;

namespace XRTK.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputActionsProfile))]
    public class MixedRealityInputActionsProfileInspector : BaseMixedRealityProfileInspector
    {
        private ReorderableList inputActionsList;
        private int currentlySelectedInputActionElement;
        private SerializedProperty inputActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputActions = serializedObject.FindProperty(nameof(inputActions));
            inputActionsList = new ReorderableList(serializedObject, inputActions, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            inputActionsList.drawHeaderCallback += InputActionsList_DrawHeaderCallback;
            inputActionsList.drawElementCallback += InputActionsList_DrawConfigurationOptionElement;
            inputActionsList.onAddCallback += InputActionsList_OnConfigurationOptionAdded;
            inputActionsList.onRemoveCallback += InputActionsList_OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Input Actions are any/all actions your users will be able to make when interacting with your application.\n\nAfter defining all your actions, you can then wire up these actions to hardware sensors, controllers, and other input devices.");

            serializedObject.Update();
            inputActionsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void InputActionsList_DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, "Input Actions");

        private void InputActionsList_DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedInputActionElement = index;
            }

            rect.height = 1 * EditorGUIUtility.singleLineHeight;
            rect.y += 3f;

            var constraintWidth = 128f;
            var descriptionRect = new Rect(rect.x, rect.y, rect.width - constraintWidth - 8f, EditorGUIUtility.singleLineHeight);
            var constraintRect = new Rect(rect.x + rect.width - constraintWidth, descriptionRect.y, constraintWidth, EditorGUIUtility.singleLineHeight);

            var inputAction = inputActions.GetArrayElementAtIndex(index);
            var profileGuidProperty = inputAction.FindPropertyRelative("profileGuid");
            var inputActionDescription = inputAction.FindPropertyRelative("description");
            var inputActionConstraint = inputAction.FindPropertyRelative("axisConstraint");

            profileGuidProperty.stringValue = ThisProfileGuidString;

            EditorGUI.PropertyField(descriptionRect, inputActionDescription, GUIContent.none);
            EditorGUI.PropertyField(constraintRect, inputActionConstraint, GUIContent.none);
        }

        private void InputActionsList_OnConfigurationOptionAdded(ReorderableList list)
        {
            serializedObject.Update();

            inputActions.arraySize += 1;
            var inputAction = inputActions.GetArrayElementAtIndex(inputActions.arraySize - 1);
            var inputActionId = inputAction.FindPropertyRelative("id");
            var profileGuidProperty = inputAction.FindPropertyRelative("profileGuid");
            var inputActionDescription = inputAction.FindPropertyRelative("description");
            var inputActionConstraint = inputAction.FindPropertyRelative("axisConstraint");

            inputActionConstraint.intValue = 0;
            profileGuidProperty.stringValue = ThisProfileGuidString;
            inputActionDescription.stringValue = $"New Action {inputActionId.intValue = inputActions.arraySize}";

            serializedObject.ApplyModifiedProperties();
        }

        private void InputActionsList_OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedInputActionElement >= 0)
            {
                inputActions.DeleteArrayElementAtIndex(currentlySelectedInputActionElement);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
