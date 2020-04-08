// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputActionsProfile))]
    public class MixedRealityInputActionsProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Action");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Action", "Add New Action");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "The Name of the Action.");
        private static readonly GUIContent AxisConstraintContent = new GUIContent("Axis Constraint", "Optional Axis Constraint for this input source.");

        private static Vector2 scrollPosition = Vector2.zero;

        private SerializedProperty inputActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputActions = serializedObject.FindProperty(nameof(inputActions));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Input Actions are any/all actions your users will be able to make when interacting with your application.\n\nAfter defining all your actions, you can then wire up these actions to hardware sensors, controllers, and other input devices.");

            serializedObject.Update();
            RenderList(inputActions);
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderList(SerializedProperty list)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var inputAction = list.GetArrayElementAtIndex(list.arraySize - 1);
                var inputActionId = inputAction.FindPropertyRelative("id");
                var profileGuidProperty = inputAction.FindPropertyRelative("profileGuid");
                var inputActionDescription = inputAction.FindPropertyRelative("description");
                var inputActionConstraint = inputAction.FindPropertyRelative("axisConstraint");

                inputActionConstraint.intValue = 0;
                profileGuidProperty.stringValue = ThisProfileGuidString;
                inputActionDescription.stringValue = $"New Action {inputActionId.intValue = list.arraySize}";
            }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 36f;
            EditorGUILayout.LabelField(ActionContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(AxisConstraintContent, GUILayout.Width(96f));
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
            EditorGUIUtility.labelWidth = labelWidth;
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 64f;
                var inputAction = list.GetArrayElementAtIndex(i);
                var profileGuidProperty = inputAction.FindPropertyRelative("profileGuid");
                var inputActionDescription = inputAction.FindPropertyRelative("description");
                var inputActionConstraint = inputAction.FindPropertyRelative("axisConstraint");

                profileGuidProperty.stringValue = ThisProfileGuidString;

                EditorGUILayout.PropertyField(inputActionDescription, GUIContent.none);
                EditorGUILayout.PropertyField(inputActionConstraint, GUIContent.none, GUILayout.Width(96f));
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}
