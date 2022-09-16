// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;

namespace XRTK.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealitySpeechCommandsProfile))]
    public class MixedRealitySpeechCommandsProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Speech Command");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Speech Command", "Add Speech Command");
        private static readonly GUIContent KeywordContent = new GUIContent("Keywords", "Spoken words and input actions that will trigger the input event.");

        private SerializedProperty startBehavior;
        private SerializedProperty recognitionConfidenceLevel;
        private SerializedProperty speechCommands;

        protected override void OnEnable()
        {
            base.OnEnable();

            startBehavior = serializedObject.FindProperty(nameof(startBehavior));
            recognitionConfidenceLevel = serializedObject.FindProperty(nameof(recognitionConfidenceLevel));
            speechCommands = serializedObject.FindProperty(nameof(speechCommands));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("Speech Commands are any/all spoken keywords your users will be able say to raise an Input Action in your application.");

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
            EditorGUILayout.LabelField(KeywordContent, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty speechCommand = list.GetArrayElementAtIndex(i);
                var keyword = speechCommand.FindPropertyRelative("keyword");
                EditorGUILayout.PropertyField(keyword, GUIContent.none, GUILayout.ExpandWidth(true));

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
