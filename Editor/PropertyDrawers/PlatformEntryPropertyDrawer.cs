// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Platforms;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Services;

namespace XRTK.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RuntimePlatformEntry))]
    public class PlatformEntryPropertyDrawer : PropertyDrawer
    {
        private const string Nothing = "Nothing";
        private const string Everything = "Everything";
        private const string Platform = "Platform";
        private const string TypeReferenceUpdated = "TypeReferenceUpdated";

        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly GUIContent EditorContent = new GUIContent("Editor");
        private static readonly GUIContent NothingContent = new GUIContent(Nothing);
        private static readonly GUIContent EverythingContent = new GUIContent(Everything);
        private static readonly GUIContent EditorOnlyContent = new GUIContent("Editor Only");
        private static readonly GUIContent RuntimePlatformContent = new GUIContent("Runtime Platforms");
        private static readonly GUIContent EditorBuildTargetContent = new GUIContent("Editor + Build Target");

        private static readonly int ControlHint = typeof(PlatformEntryPropertyDrawer).GetHashCode();

        private static readonly Type AllPlatformsType = typeof(AllPlatforms);
        private static readonly Type EditorPlatformType = typeof(EditorPlatform);
        private static readonly Type EditorBuildTargetType = typeof(CurrentBuildTargetPlatform);

        private static readonly string AllPlatformTypeReference = SystemType.GetReference(AllPlatformsType);
        private static readonly string EditorPlatformTypeReference = SystemType.GetReference(EditorPlatformType);
        private static readonly string EditorBuildTargetTypeReference = SystemType.GetReference(EditorBuildTargetType);

        private static int selectionControlId;
        private static int arraySize = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, RuntimePlatformContent);

            var runtimePlatformsProperty = property.FindPropertyRelative("runtimePlatforms");

            DrawTypeSelectionControl(position, runtimePlatformsProperty);
        }

        private static void DrawTypeSelectionControl(Rect position, SerializedProperty runtimePlatformsProperty)
        {
            var triggerDropDown = false;
            var controlId = GUIUtility.GetControlID(ControlHint, FocusType.Keyboard, position);

            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == TypeReferenceUpdated &&
                        selectionControlId == controlId)
                    {
                        if (runtimePlatformsProperty.arraySize != arraySize)
                        {
                            GUI.changed = true;
                        }

                        arraySize = 0;
                        selectionControlId = 0;
                    }

                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlId;
                        triggerDropDown = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (GUI.enabled && GUIUtility.keyboardControl == controlId)
                    {
                        if (Event.current.keyCode == KeyCode.Return ||
                            Event.current.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            Event.current.Use();
                        }
                    }

                    break;

                case EventType.Repaint:
                    TempContent.text = GetDropdownContentText();
                    EditorStyles.popup.Draw(position, TempContent, controlId);
                    break;
            }

            if (triggerDropDown)
            {
                arraySize = runtimePlatformsProperty.arraySize;
                selectionControlId = controlId;

                var menu = new GenericMenu();

                var editorIsActive = IsPlatformActive(EditorPlatformType);
                var isAllPlatformsActive = IsPlatformActive(AllPlatformsType);
                var editorBuildTargetIsActive = IsPlatformActive(EditorBuildTargetType);

                if (editorIsActive || editorBuildTargetIsActive)
                {
                    Debug.Assert(editorIsActive != editorBuildTargetIsActive);
                }

                menu.AddItem(NothingContent, arraySize == 0, OnNothingSelected, null);
                menu.AddItem(EverythingContent, isAllPlatformsActive, OnEverythingSelected, null);

                if (!isAllPlatformsActive)
                {
                    menu.AddItem(EditorOnlyContent, editorIsActive, () =>
                    {
                        if (!TryRemovePlatformReference(EditorPlatformTypeReference))
                        {
                            runtimePlatformsProperty.ClearArray();
                            TryAddPlatformReference(EditorPlatformTypeReference);
                            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
                        }
                    });
                }

                menu.AddItem(isAllPlatformsActive ? EditorContent : EditorBuildTargetContent, isAllPlatformsActive || editorBuildTargetIsActive, OnEditorSelected, null);
                menu.AddSeparator(string.Empty);

                for (var i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                {
                    var platform = MixedRealityToolkit.AvailablePlatforms[i];
                    var platformType = platform.GetType();

                    if (platformType == AllPlatformsType ||
                        platformType == EditorPlatformType ||
                        platformType == EditorBuildTargetType)
                    {
                        continue;
                    }

                    menu.AddItem(new GUIContent(platformType.Name.Replace(Platform, string.Empty).ToProperCase()), IsPlatformActive(platformType) || isAllPlatformsActive, OnSelectedTypeName, platformType);
                }

                menu.DropDown(position);
            }

            bool IsPlatformActive(Type platformType)
            {
                var isActive = false;

                for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                {
                    var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                    var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");
                    var referenceType = TypeExtensions.ResolveType(referenceProperty.stringValue);

                    // Clean up any broken references
                    if (referenceType == null)
                    {
                        Debug.LogError($"Failed to resolve {referenceProperty.stringValue}! Removing from runtime platform entry...");
                        runtimePlatformsProperty.DeleteArrayElementAtIndex(i);
                        runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                        continue;
                    }

                    if (platformType == referenceType)
                    {
                        isActive = true;
                    }
                }

                return isActive;
            }

            string GetDropdownContentText()
            {
                if (runtimePlatformsProperty.arraySize == 0)
                {
                    return Nothing;
                }

                if (runtimePlatformsProperty.arraySize == 1)
                {
                    if (IsPlatformActive(AllPlatformsType))
                    {
                        return "Everything";
                    }

                    // Remove assembly name and namespace from content of popup control.
                    var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(0);
                    var classRefProperty = systemTypeProperty.FindPropertyRelative("reference");
                    var classRefParts = classRefProperty.stringValue.Split(',');
                    var className = classRefParts[0].Trim();
                    className = className.Substring(className.LastIndexOf(".", StringComparison.Ordinal) + 1);
                    var contentText = className.Replace("Platform", "").ToProperCase();

                    if (contentText == string.Empty)
                    {
                        contentText = Nothing;
                    }

                    return contentText;
                }

                return "Multiple...";
            }

            void OnNothingSelected(object _)
            {
                runtimePlatformsProperty.ClearArray();
                runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
            }

            void OnEverythingSelected(object _)
            {
                runtimePlatformsProperty.ClearArray();
                TryAddPlatformReference(AllPlatformTypeReference);
                runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
            }

            void OnEditorSelected(object _)
            {
                var isAllPlatformsActive = false;
                var isCurrentBuildTargetPlatformActive = false;

                for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                {
                    var typeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                    var refProperty = typeProperty.FindPropertyRelative("reference");
                    var referenceType = TypeExtensions.ResolveType(refProperty.stringValue);

                    if (referenceType == EditorBuildTargetType)
                    {
                        isCurrentBuildTargetPlatformActive = true;
                    }

                    if (referenceType == AllPlatformsType)
                    {
                        isAllPlatformsActive = true;
                    }
                }

                if (isAllPlatformsActive)
                {
                    runtimePlatformsProperty.ClearArray();

                    for (int i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                    {
                        var platformType = MixedRealityToolkit.AvailablePlatforms[i].GetType();

                        if (platformType == AllPlatformsType ||
                            platformType == EditorPlatformType ||
                            platformType == EditorBuildTargetType)
                        {
                            continue;
                        }

                        TryAddPlatformReference(SystemType.GetReference(platformType));
                    }
                }
                else
                {
                    if (isCurrentBuildTargetPlatformActive)
                    {
                        TryRemovePlatformReference(EditorBuildTargetTypeReference);

                        if (runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count - 3)
                        {
                            OnEverythingSelected(null);
                        }
                    }
                    else
                    {
                        TryAddPlatformReference(EditorBuildTargetTypeReference);

                        foreach (var platform in MixedRealityToolkit.AvailablePlatforms)
                        {
                            if (platform is AllPlatforms ||
                                platform is EditorPlatform ||
                                platform is CurrentBuildTargetPlatform)
                            {
                                continue;
                            }

                            if (platform.IsBuildTargetAvailable)
                            {
                                TryAddPlatformReference(SystemType.GetReference(platform.GetType()));
                            }
                        }

                        if (runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count - 2)
                        {
                            OnEverythingSelected(null);
                        }
                    }
                }

                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
            }

            void OnSelectedTypeName(object typeRef)
            {
                var selectedPlatformType = typeRef as Type;
                var selectedReference = SystemType.GetReference(selectedPlatformType);

                if (!TryRemovePlatformReference(selectedReference))
                {
                    if (runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count - 3)
                    {
                        OnEverythingSelected(null);
                    }
                    else
                    {
                        TryAddPlatformReference(selectedReference);
                    }
                }

                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
            }

            bool TryAddPlatformReference(string classReference)
            {
                if (TryRemovePlatformReference(EditorPlatformTypeReference))
                {
                    TryAddPlatformReference(EditorBuildTargetTypeReference);
                }

                var selectedPlatformType = TypeExtensions.ResolveType(classReference);

                for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                {
                    var existingSystemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                    var existingReferenceProperty = existingSystemTypeProperty.FindPropertyRelative("reference");
                    var existingPlatformType = TypeExtensions.ResolveType(existingReferenceProperty.stringValue);

                    if (selectedPlatformType == existingPlatformType)
                    {
                        return false;
                    }
                }

                var index = runtimePlatformsProperty.arraySize;
                runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                runtimePlatformsProperty.InsertArrayElementAtIndex(index);
                var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(index);
                var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");
                referenceProperty.stringValue = classReference;
                runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                return true;
            }

            bool TryRemovePlatformReference(string classReference)
            {
                var selectedPlatformType = TypeExtensions.ResolveType(classReference);

                if (IsPlatformActive(AllPlatformsType))
                {
                    runtimePlatformsProperty.ClearArray();

                    for (int j = 0; j < MixedRealityToolkit.AvailablePlatforms.Count; j++)
                    {
                        var platformType = MixedRealityToolkit.AvailablePlatforms[j].GetType();

                        if (platformType != selectedPlatformType &&
                            platformType != AllPlatformsType &&
                            platformType != EditorPlatformType)
                        {
                            TryAddPlatformReference(SystemType.GetReference(platformType));
                        }
                    }

                    return true;
                }

                for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                {
                    var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                    var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");
                    var referenceType = TypeExtensions.ResolveType(referenceProperty.stringValue);

                    if (selectedPlatformType == referenceType)
                    {
                        if (runtimePlatformsProperty.arraySize == 2 &&
                            IsPlatformActive(EditorBuildTargetType) &&
                            selectedPlatformType != EditorBuildTargetType)
                        {
                            runtimePlatformsProperty.ClearArray();
                            runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                            TryAddPlatformReference(EditorPlatformTypeReference);
                            return true;
                        }

                        runtimePlatformsProperty.DeleteArrayElementAtIndex(i);
                        runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                        return true;
                    }
                }

                return false;
            }
        }
    }
}