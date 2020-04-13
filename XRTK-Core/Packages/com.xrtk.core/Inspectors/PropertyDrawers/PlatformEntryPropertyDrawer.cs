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
                    switch (runtimePlatformsProperty.arraySize)
                    {
                        case 0:
                            TempContent.text = Nothing;
                            break;
                        case 1:
                            // Remove assembly name and namespace from content of popup control.
                            var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(0);
                            var classRefProperty = systemTypeProperty.FindPropertyRelative("reference");
                            var classRefParts = classRefProperty.stringValue.Split(',');
                            var className = classRefParts[0].Trim();
                            className = className.Substring(className.LastIndexOf(".", StringComparison.Ordinal) + 1);
                            TempContent.text = className.Replace("Platform", "").ToProperCase();

                            if (TempContent.text == string.Empty)
                            {
                                TempContent.text = Nothing;
                            }

                            break;
                        default:
                            TempContent.text = runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count ? "Everything" : "Multiple...";
                            break;
                    }

                    EditorStyles.popup.Draw(position, TempContent, controlId);
                    break;
            }

            if (triggerDropDown)
            {
                arraySize = runtimePlatformsProperty.arraySize;
                selectionControlId = controlId;

                var menu = new GenericMenu();

                menu.AddItem(NothingContent, arraySize == 0, OnNothingSelected, null);
                menu.AddItem(EverythingContent, arraySize == MixedRealityToolkit.AvailablePlatforms.Count, OnEverythingSelected, null);

                for (var i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                {
                    var platform = MixedRealityToolkit.AvailablePlatforms[i];
                    var platformType = platform.GetType();
                    if (platformType == typeof(AllPlatforms)) { continue; }
                    menu.AddItem(new GUIContent(platformType.Name.Replace("Platform", "").ToProperCase()), IsPlatformActive(platformType), OnSelectedTypeName, platformType);
                }

                menu.DropDown(position);

                void OnNothingSelected(object _)
                {
                    runtimePlatformsProperty.ClearArray();
                    runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                    EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
                }

                void OnEverythingSelected(object _)
                {
                    runtimePlatformsProperty.ClearArray();

                    for (int i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                    {
                        AddPlatformReference(SystemType.GetReference(MixedRealityToolkit.AvailablePlatforms[i].GetType()), true);
                    }

                    runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                    EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
                }

                void OnSelectedTypeName(object typeRef)
                {
                    var selectedPlatformType = typeRef as Type;
                    var selectedReference = SystemType.GetReference(selectedPlatformType);

                    if (!TryRemovePlatformReference(selectedReference))
                    {
                        if (runtimePlatformsProperty.arraySize + 2 == MixedRealityToolkit.AvailablePlatforms.Count)
                        {
                            OnEverythingSelected(null);
                            return;
                        }

                        if (selectedPlatformType != null)
                        {
                            AddPlatformReference(selectedReference);
                        }
                    }

                    EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
                }

                bool IsPlatformActive(Type platformType)
                {
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
                            return true;
                        }
                    }

                    return false;
                }

                void AddPlatformReference(string classReference, bool forceAdd = false)
                {
                    if (!forceAdd)
                    {
                        var selectedPlatformType = TypeExtensions.ResolveType(classReference);

                        for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                        {
                            var typeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                            var refProperty = typeProperty.FindPropertyRelative("reference");
                            var referenceType = TypeExtensions.ResolveType(refProperty.stringValue);

                            if (selectedPlatformType == referenceType)
                            {
                                return;
                            }

                            if (selectedPlatformType == typeof(CurrentBuildTargetPlatform) &&
                                referenceType == typeof(EditorPlatform) ||
                                selectedPlatformType == typeof(EditorPlatform) &&
                                referenceType == typeof(CurrentBuildTargetPlatform))
                            {
                                TryRemovePlatformReference(refProperty.stringValue);
                            }
                        }

                        if (selectedPlatformType == typeof(CurrentBuildTargetPlatform))
                        {
                            for (int i = 0; i < MixedRealityToolkit.AvailablePlatforms.Count; i++)
                            {
                                var activeBuildTarget = MixedRealityToolkit.AvailablePlatforms[i];

                                if (activeBuildTarget.IsBuildTargetAvailable)
                                {
                                    var activePlatformType = activeBuildTarget.GetType();

                                    if (activePlatformType != typeof(AllPlatforms) &&
                                        activePlatformType != typeof(EditorPlatform) &&
                                        activePlatformType != typeof(CurrentBuildTargetPlatform))
                                    {
                                        AddPlatformReference(SystemType.GetReference(activePlatformType));
                                    }
                                }
                            }
                        }
                        else if (selectedPlatformType == typeof(EditorPlatform))
                        {
                            TryRemovePlatformReference(SystemType.GetReference(typeof(CurrentBuildTargetPlatform)));
                        }
                    }

                    var index = runtimePlatformsProperty.arraySize;
                    runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                    runtimePlatformsProperty.InsertArrayElementAtIndex(index);
                    var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(index);
                    var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");
                    referenceProperty.stringValue = classReference;
                    runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                }

                bool TryRemovePlatformReference(string classReference)
                {
                    var selectedPlatformType = TypeExtensions.ResolveType(classReference);

                    for (int i = 0; i < runtimePlatformsProperty.arraySize; i++)
                    {
                        var systemTypeProperty = runtimePlatformsProperty.GetArrayElementAtIndex(i);
                        var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");
                        var referenceType = TypeExtensions.ResolveType(referenceProperty.stringValue);

                        if (selectedPlatformType == referenceType)
                        {
                            bool removeEveryPlatform = selectedPlatformType != typeof(AllPlatforms) &&
                                                       runtimePlatformsProperty.arraySize == MixedRealityToolkit.AvailablePlatforms.Count;

                            runtimePlatformsProperty.DeleteArrayElementAtIndex(i);

                            if (removeEveryPlatform)
                            {
                                TryRemovePlatformReference(SystemType.GetReference(typeof(AllPlatforms)));
                            }
                            else
                            {
                                runtimePlatformsProperty.serializedObject.ApplyModifiedProperties();
                            }

                            return true;
                        }
                    }

                    return false;
                }
            }
        }
    }
}