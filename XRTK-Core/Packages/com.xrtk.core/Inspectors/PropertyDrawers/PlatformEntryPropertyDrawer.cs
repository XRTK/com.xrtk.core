// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Platforms;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RuntimePlatformEntry))]
    public class PlatformEntryPropertyDrawer : PropertyDrawer
    {
        private const string Nothing = "Nothing";
        private const string TypeReferenceUpdated = "TypeReferenceUpdated";

        private static readonly GUIContent NothingContent = new GUIContent(Nothing);
        private static readonly GUIContent EverythingContent = new GUIContent("Everything");
        private static readonly GUIContent RuntimePlatformContent = new GUIContent("Runtime Platforms");
        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly int ControlHint = typeof(PlatformEntryPropertyDrawer).GetHashCode();

        private static int selectionControlId;
        private static int arraySize = 0;

        private static GUIContent editorContent;

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

                var editorIsActive = IsPlatformActive(typeof(EditorPlatform));
                var isAllPlatformsActive = IsPlatformActive(typeof(AllPlatforms));
                var editorBuildTargetIsActive = IsPlatformActive(typeof(CurrentBuildTargetPlatform));

                if (editorIsActive || editorBuildTargetIsActive)
                {
                    Debug.Assert(editorIsActive != editorBuildTargetIsActive);
                }

                menu.AddItem(NothingContent, arraySize == 0, OnNothingSelected, null);
                menu.AddItem(EverythingContent, isAllPlatformsActive, OnEverythingSelected, null);

                if (!isAllPlatformsActive)
                {
                    menu.AddItem(new GUIContent("Editor Only"), editorIsActive, () =>
                    {
                        if (!TryRemovePlatformReference(SystemType.GetReference(typeof(EditorPlatform))))
                        {
                            runtimePlatformsProperty.ClearArray();
                            TryAddPlatformReference(SystemType.GetReference(typeof(EditorPlatform)));
                        }
                    });
                }

                menu.AddItem(isAllPlatformsActive ? new GUIContent("Editor") : new GUIContent("Editor Build Target"), isAllPlatformsActive || editorBuildTargetIsActive, OnEditorSelected, null);
                menu.AddSeparator("");

                for (var i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                {
                    var platform = MixedRealityToolkit.AvailablePlatforms[i];
                    var platformType = platform.GetType();

                    if (platformType == typeof(AllPlatforms) ||
                        platformType == typeof(EditorPlatform) ||
                        platformType == typeof(CurrentBuildTargetPlatform))
                    {
                        continue;
                    }

                    menu.AddItem(new GUIContent(platformType.Name.Replace("Platform", "").ToProperCase()), IsPlatformActive(platformType) || isAllPlatformsActive, OnSelectedTypeName, platformType);
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
                    if (IsPlatformActive(typeof(AllPlatforms)))
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
                TryAddPlatformReference(SystemType.GetReference(typeof(AllPlatforms)));
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

                    if (referenceType == typeof(CurrentBuildTargetPlatform))
                    {
                        isCurrentBuildTargetPlatformActive = true;
                    }

                    if (referenceType == typeof(AllPlatforms))
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

                        if (platformType == typeof(AllPlatforms) ||
                            platformType == typeof(EditorPlatform) ||
                            platformType == typeof(CurrentBuildTargetPlatform))
                        {
                            continue;
                        }

                        TryAddPlatformReference(SystemType.GetReference(platformType));
                    }

                    return;
                }

                if (isCurrentBuildTargetPlatformActive)
                {
                    TryRemovePlatformReference(SystemType.GetReference(typeof(CurrentBuildTargetPlatform)));

                    if (runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count - 3)
                    {
                        OnEverythingSelected(null);
                    }
                }
                else
                {
                    TryAddPlatformReference(SystemType.GetReference(typeof(CurrentBuildTargetPlatform)));

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
                if (TryRemovePlatformReference(SystemType.GetReference(typeof(EditorPlatform))))
                {
                    TryAddPlatformReference(SystemType.GetReference(typeof(CurrentBuildTargetPlatform)));
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

                if (IsPlatformActive(typeof(AllPlatforms)))
                {
                    runtimePlatformsProperty.ClearArray();

                    for (int j = 0; j < MixedRealityToolkit.AvailablePlatforms.Count; j++)
                    {
                        var platformType = MixedRealityToolkit.AvailablePlatforms[j].GetType();

                        if (platformType != selectedPlatformType &&
                            platformType != typeof(AllPlatforms) &&
                            platformType != typeof(EditorPlatform))
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
                            IsPlatformActive(typeof(CurrentBuildTargetPlatform)) &&
                            selectedPlatformType != typeof(CurrentBuildTargetPlatform))
                        {
                            runtimePlatformsProperty.ClearArray();
                            runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                            TryAddPlatformReference(SystemType.GetReference(typeof(EditorPlatform)));
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