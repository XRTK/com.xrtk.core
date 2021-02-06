// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Data;
using XRTK.Editor.PropertyDrawers;
using XRTK.Extensions;
using XRTK.Editor.Utilities;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Providers.Controllers.OpenVR;
using XRTK.Providers.Controllers.UnityInput;

namespace XRTK.Editor
{
    public class ControllerPopupWindow : EditorWindow
    {
        private const float INPUT_ACTION_LABEL_WIDTH = 119f;

        /// <summary>
        /// Used to enable editing the input axis label positions on controllers
        /// </summary>
        private static readonly bool EnableWysiwyg = false;

        private static readonly Vector2 HorizontalSpace = new Vector2(8f, 0f);
        private static readonly Vector2 InputActionLabelPosition = new Vector2(256f, 0f);
        private static readonly Vector2 InputActionDropdownPosition = new Vector2(88f, 0f);
        private static readonly Vector2 InputActionFlipTogglePosition = new Vector2(-24f, 0f);

        private static readonly Rect ControllerRectPosition = new Rect(new Vector2(128f, 0f), new Vector2(512f, 512f));

        private static ControllerPopupWindow window;
        private static ControllerInputActionOptions controllerInputActionOptions;

        private static bool[] isMouseInRects;

        private static bool editInputActionPositions;

        private static Vector2 horizontalScrollPosition;

        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        private SerializedProperty currentInteractionProfiles;

        private Vector2 mouseDragOffset;

        private static GUIStyle labelStyle;
        private static GUIStyle LabelStyle => labelStyle ?? (labelStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState
            {
                textColor = EditorGUIUtility.isProSkin
                    ? Color.white
                    : Color.black
            },
            stretchHeight = true,
            stretchWidth = true,
        });

        private static GUIStyle flippedLabelStyle;
        private static GUIStyle FlippedLabelStyle => flippedLabelStyle ?? (flippedLabelStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleRight,
            normal = new GUIStyleState
            {
                textColor = EditorGUIUtility.isProSkin
                    ? Color.white
                    : Color.black
            },
            stretchHeight = true,
            stretchWidth = true,
        });

        private static GUIStyle backgroundStyle;

        private static GUIStyle BackgroundStyle => backgroundStyle ?? (backgroundStyle = new GUIStyle(EditorStyles.toolbar));

        private Texture2D currentControllerTexture;
        private ControllerInputActionOption currentControllerOption;

        private MixedRealityControllerMappingProfile controllerDataProviderProfile;
        private string currentControllerName;

        private Type ControllerType => controllerDataProviderProfile.ControllerType;
        private Handedness Handedness => controllerDataProviderProfile.Handedness;

        private bool IsCustomController => ControllerType == typeof(GenericOpenVRController) ||
                                           ControllerType == typeof(GenericJoystickController) ||
                                           ControllerType == typeof(IMixedRealityHandController);

        private static string EditorWindowOptionsPath => $"{PathFinderUtility.XRTK_Core_RelativeFolderPath}/Editor/Data/EditorWindowOptions.json";

        private void OnFocus()
        {
            if (window.IsNull())
            {
                Close();
            }

            currentControllerTexture = ControllerMappingUtilities.GetControllerTexture(controllerDataProviderProfile);
        }

        /// <summary>
        /// Shows the controller pop out window using the provided profile and serialized interaction mapping property
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="interactionMappingProfiles"></param>
        public static void Show(MixedRealityControllerMappingProfile profile, SerializedProperty interactionMappingProfiles)
        {
            var handednessTitleText = profile.Handedness != Handedness.None ? $"{profile.Handedness} Hand " : string.Empty;

            if (window != null)
            {
                window.Close();
            }

            if (profile.ControllerType?.Type == null)
            {
                Debug.LogError($"No controller type defined for {profile.name}");
                return;
            }

            window = (ControllerPopupWindow)CreateInstance(typeof(ControllerPopupWindow));
            window.currentControllerName = profile.ControllerType?.Type?.Name;

            window.currentControllerName = profile.ControllerType?.Type?.Name;
            window.titleContent = new GUIContent($"{window.currentControllerName} {handednessTitleText}Input Action Assignment");
            window.controllerDataProviderProfile = profile;
            window.currentInteractionProfiles = interactionMappingProfiles;
            window.currentControllerTexture = ControllerMappingUtilities.GetControllerTexture(profile);

            isMouseInRects = new bool[interactionMappingProfiles.arraySize];

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorWindowOptionsPath);

            if (asset.IsNull())
            {
                var empty = new ControllerInputActionOptions
                {
                    Controllers = new List<ControllerInputActionOption>
                    {
                        new ControllerInputActionOption
                        {
                            Controller = null,
                            Handedness = Handedness.None,
                            InputLabelPositions = new[] { new Vector2(0, 0) },
                            IsLabelFlipped = new []{ false }
                        }
                    }
                };

                File.WriteAllText(Path.GetFullPath(EditorWindowOptionsPath), JsonUtility.ToJson(empty, true));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                controllerInputActionOptions = JsonUtility.FromJson<ControllerInputActionOptions>(asset.text);

                // if the controller option doesn't exist, then make a new one.
                if (!controllerInputActionOptions.Controllers.Any(option => option.Controller == window.currentControllerName && option.Handedness == window.Handedness))
                {
                    var newOption = new ControllerInputActionOption
                    {
                        Controller = window.currentControllerName,
                        Handedness = window.Handedness,
                        InputLabelPositions = new Vector2[interactionMappingProfiles.arraySize],
                        IsLabelFlipped = new bool[interactionMappingProfiles.arraySize]
                    };

                    controllerInputActionOptions.Controllers.Add(newOption);
                }

                window.currentControllerOption = controllerInputActionOptions.Controllers.FirstOrDefault(option => option.Controller == window.currentControllerName && option.Handedness == window.Handedness);

                if (window.currentControllerOption.IsLabelFlipped == null)
                {
                    window.currentControllerOption.IsLabelFlipped = new bool[interactionMappingProfiles.arraySize];
                }

                if (window.currentControllerOption.InputLabelPositions == null)
                {
                    window.currentControllerOption.InputLabelPositions = new Vector2[interactionMappingProfiles.arraySize];
                }
            }

            var windowSize = new Vector2(window.IsCustomController || window.currentControllerTexture == null ? 896f : 768f, 512f);

            window.ShowUtility();
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.CenterOnMainWin();
        }

        private void Update()
        {
            if (editInputActionPositions)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!IsCustomController && currentControllerTexture != null)
            {
                GUILayout.BeginHorizontal();
                GUI.DrawTexture(ControllerRectPosition, currentControllerTexture);
                GUILayout.EndHorizontal();
            }

            try
            {
                RenderInteractionList(currentInteractionProfiles, IsCustomController || currentControllerTexture == null);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Close();
            }
        }

        private void RenderInteractionList(SerializedProperty interactionProfilesList, bool useCustomInteractionMapping)
        {
            if (interactionProfilesList == null ||
                interactionProfilesList.arraySize == 0)
            {
                Debug.LogError("No interaction list found!");
                Close();
                return;
            }

            if (!useCustomInteractionMapping)
            {
                if (currentControllerOption.IsLabelFlipped == null ||
                    currentControllerOption.IsLabelFlipped.Length != interactionProfilesList.arraySize)
                {
                    var newArray = new bool[interactionProfilesList.arraySize];

                    for (int i = 0; i < currentControllerOption.IsLabelFlipped?.Length; i++)
                    {
                        newArray[i] = currentControllerOption.IsLabelFlipped[i];
                    }

                    currentControllerOption.IsLabelFlipped = newArray;
                }

                if (currentControllerOption.InputLabelPositions == null ||
                    currentControllerOption.InputLabelPositions.Length != interactionProfilesList.arraySize)
                {
                    var newArray = new Vector2[interactionProfilesList.arraySize].InitialiseArray(new Vector2(0, 25));

                    for (int i = 0; i < currentControllerOption.InputLabelPositions?.Length; i++)
                    {
                        newArray[i] = currentControllerOption.InputLabelPositions[i];
                    }

                    currentControllerOption.InputLabelPositions = newArray;
                }
            }

            GUILayout.BeginVertical();

            if (useCustomInteractionMapping)
            {
                horizontalScrollPosition = EditorGUILayout.BeginScrollView(horizontalScrollPosition, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true));
            }

            if (!useCustomInteractionMapping &&
                EnableWysiwyg)
            {
                EditorGUI.BeginChangeCheck();
                editInputActionPositions =
                    EditorGUILayout.Toggle("Edit Input Action Positions", editInputActionPositions);

                if (EditorGUI.EndChangeCheck())
                {
                    if (!editInputActionPositions)
                    {
                        File.WriteAllText(Path.GetFullPath(EditorWindowOptionsPath), JsonUtility.ToJson(controllerInputActionOptions, true));
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    else
                    {
                        if (!controllerInputActionOptions.Controllers.Any(option => option.Controller == currentControllerName && option.Handedness == Handedness))
                        {
                            currentControllerOption = new ControllerInputActionOption
                            {
                                Controller = currentControllerName,
                                Handedness = Handedness,
                                InputLabelPositions = new Vector2[currentInteractionProfiles.arraySize],
                                IsLabelFlipped = new bool[currentInteractionProfiles.arraySize]
                            };

                            controllerInputActionOptions.Controllers.Add(currentControllerOption);
                            isMouseInRects = new bool[currentInteractionProfiles.arraySize];

                            if (controllerInputActionOptions.Controllers.Any(option => option.Controller == null))
                            {
                                controllerInputActionOptions.Controllers.Remove(
                                    controllerInputActionOptions.Controllers.Find(option =>
                                        option.Controller == null));
                            }

                            AssetDatabase.DeleteAsset(EditorWindowOptionsPath);
                            File.WriteAllText(Path.GetFullPath(EditorWindowOptionsPath), JsonUtility.ToJson(controllerInputActionOptions, true));
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                        }
                    }
                }
            }

            for (int i = 0; i < interactionProfilesList.arraySize; i++)
            {
                var interactionProfileProperty = interactionProfilesList.GetArrayElementAtIndex(i);
                var mappingProfile = interactionProfileProperty.objectReferenceValue as MixedRealityInteractionMappingProfile;

                if (mappingProfile.IsNull())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(interactionProfileProperty, GUIContent.none, GUILayout.Width(INPUT_ACTION_LABEL_WIDTH));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                var interactionMappingProperty = new SerializedObject(mappingProfile).FindProperty("interactionMapping");
                var axisType = interactionMappingProperty.FindPropertyRelative("axisType");
                var action = interactionMappingProperty.FindPropertyRelative("inputAction");
                var interactionDescription = interactionMappingProperty.FindPropertyRelative("description");
                var description = interactionDescription.stringValue;
                var axisConstraint = (AxisType)axisType.intValue;

                EditorGUILayout.BeginHorizontal();

                if (useCustomInteractionMapping ||
                    currentControllerTexture.IsNull())
                {
                    inputActionDropdown.OnGui(GUIContent.none, action, axisConstraint, GUILayout.Width(INPUT_ACTION_LABEL_WIDTH));
                    EditorGUILayout.LabelField(description, GUILayout.ExpandWidth(true));
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    var flipped = currentControllerOption.IsLabelFlipped[i];
                    var rectPosition = currentControllerOption.InputLabelPositions[i];
                    var rectSize = InputActionLabelPosition + InputActionDropdownPosition + new Vector2(flipped ? 0f : 8f, EditorGUIUtility.singleLineHeight);

                    if (rectPosition == Vector2.zero)
                    {
                        rectPosition.y += (i + 1) * (EditorGUIUtility.singleLineHeight + 2);
                    }

                    GUI.Box(new Rect(rectPosition, rectSize), GUIContent.none, BackgroundStyle);

                    var offset = flipped ? InputActionLabelPosition : Vector2.zero;
                    var popupRect = new Rect(rectPosition + offset, new Vector2(InputActionDropdownPosition.x, EditorGUIUtility.singleLineHeight));

                    inputActionDropdown.OnGui(popupRect, action, GUIContent.none, axisConstraint);

                    offset = flipped ? Vector2.zero : InputActionDropdownPosition;
                    var labelRect = new Rect(rectPosition + offset, new Vector2(InputActionLabelPosition.x, EditorGUIUtility.singleLineHeight));
                    EditorGUI.LabelField(labelRect, interactionDescription.stringValue, flipped ? FlippedLabelStyle : LabelStyle);

                    if (editInputActionPositions)
                    {
                        offset = flipped
                            ? InputActionLabelPosition + InputActionDropdownPosition + HorizontalSpace
                            : InputActionFlipTogglePosition;

                        var toggleRect = new Rect(rectPosition + offset, new Vector2(-InputActionFlipTogglePosition.x, EditorGUIUtility.singleLineHeight));

                        EditorGUI.BeginChangeCheck();

                        currentControllerOption.IsLabelFlipped[i] = EditorGUI.Toggle(toggleRect, flipped);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (currentControllerOption.IsLabelFlipped[i])
                            {
                                currentControllerOption.InputLabelPositions[i] -= InputActionLabelPosition;
                            }
                            else
                            {
                                currentControllerOption.InputLabelPositions[i] += InputActionLabelPosition;
                            }
                        }

                        if (!isMouseInRects.Any(value => value) || isMouseInRects[i])
                        {
                            switch (Event.current.type)
                            {
                                case EventType.MouseDrag
                                    when labelRect.Contains(Event.current.mousePosition) && !isMouseInRects[i]:
                                    isMouseInRects[i] = true;
                                    mouseDragOffset = Event.current.mousePosition - rectPosition;
                                    break;
                                case EventType.Repaint when isMouseInRects[i]:
                                case EventType.DragUpdated when isMouseInRects[i]:
                                    currentControllerOption.InputLabelPositions[i] =
                                        Event.current.mousePosition - mouseDragOffset;
                                    break;
                                case EventType.MouseUp when isMouseInRects[i]:
                                    currentControllerOption.InputLabelPositions[i] =
                                        Event.current.mousePosition - mouseDragOffset;
                                    mouseDragOffset = Vector2.zero;
                                    isMouseInRects[i] = false;
                                    break;
                            }
                        }
                    }
                }

                interactionMappingProperty.serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
            }

            if (useCustomInteractionMapping)
            {
                EditorGUILayout.EndScrollView();
            }

            interactionProfilesList.serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        }
    }
}
