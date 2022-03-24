// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Extensions;

namespace XRTK.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealitySpeechCommandsProfile))]
    public class MixedRealitySpeechCommandsProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Speech Command");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Speech Command", "Add Speech Command");
        private static readonly GUIContent KeywordContent = new GUIContent("Keyword", "Spoken word that will trigger the action.");
        private static readonly GUIContent KeyCodeContent = new GUIContent("KeyCode", "The keyboard key that will trigger the action.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "The action to trigger when a keyboard key is pressed or keyword is recognized.");

        private SerializedProperty startBehavior;
        private SerializedProperty recognitionConfidenceLevel;
        private SerializedProperty speechCommands;
        private static GUIContent[] actionLabels;
        private static int[] actionIds;
        private MixedRealityInputSystemProfile inputSystemProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputSystemProfile = ThisProfile.ParentProfile as MixedRealityInputSystemProfile;

            if (inputSystemProfile.IsNull() ||
                inputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            startBehavior = serializedObject.FindProperty(nameof(startBehavior));
            recognitionConfidenceLevel = serializedObject.FindProperty(nameof(recognitionConfidenceLevel));
            speechCommands = serializedObject.FindProperty(nameof(speechCommands));
            actionLabels = inputSystemProfile.InputActionsProfile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            actionIds = inputSystemProfile.InputActionsProfile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Speech Commands are any/all spoken keywords your users will be able say to raise an Input Action in your application.");

            if (inputSystemProfile.IsNull())
            {
                EditorGUILayout.HelpBox("No input system profile found, please specify an input system profile in the main configuration profile.", MessageType.Error);
                return;
            }

            if (inputSystemProfile.InputActionsProfile.IsNull())
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the input system profile.", MessageType.Error);
                return;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(startBehavior);
            EditorGUILayout.PropertyField(recognitionConfidenceLevel);

            RenderList(speechCommands);
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var speechCommand = list.GetArrayElementAtIndex(list.arraySize - 1);
                var keyword = speechCommand.FindPropertyRelative("keyword");
                keyword.stringValue = string.Empty;
                var keyCode = speechCommand.FindPropertyRelative("keyCode");
                keyCode.intValue = (int)KeyCode.None;
                var action = speechCommand.FindPropertyRelative("action");
                var actionId = action.FindPropertyRelative("id");
                var profileGuidProperty = action.FindPropertyRelative("profileGuid");
                profileGuidProperty.stringValue = DefaultGuidString;
                actionId.intValue = 0;
            }

            GUILayout.Space(12f);

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Speech Command.", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 36f;
            EditorGUILayout.LabelField(KeywordContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(KeyCodeContent, GUILayout.Width(64f));
            EditorGUILayout.LabelField(ActionContent, GUILayout.Width(64f));
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
            EditorGUIUtility.labelWidth = labelWidth;
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty speechCommand = list.GetArrayElementAtIndex(i);
                var keyword = speechCommand.FindPropertyRelative("keyword");
                EditorGUILayout.PropertyField(keyword, GUIContent.none, GUILayout.ExpandWidth(true));
                var keyCode = speechCommand.FindPropertyRelative("keyCode");
                EditorGUILayout.PropertyField(keyCode, GUIContent.none, GUILayout.Width(64f));
                var action = speechCommand.FindPropertyRelative("action");
                var actionId = action.FindPropertyRelative("id");
                var profileGuidProperty = action.FindPropertyRelative("profileGuid");
                var actionDescription = action.FindPropertyRelative("description");
                var actionConstraint = action.FindPropertyRelative("axisConstraint");

                profileGuidProperty.stringValue = actionId.intValue > 0 ? ThisProfileGuidString : DefaultGuidString;

                EditorGUI.BeginChangeCheck();
                actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, actionId.intValue, actionLabels, actionIds, GUILayout.Width(64f));

                if (EditorGUI.EndChangeCheck())
                {
                    var inputAction = actionId.intValue == 0
                        ? MixedRealityInputAction.None
                        : inputSystemProfile.InputActionsProfile.InputActions[actionId.intValue - 1];

                    actionDescription.stringValue = inputAction.Description;
                    actionConstraint.enumValueIndex = (int)inputAction.AxisConstraint;
                }

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}