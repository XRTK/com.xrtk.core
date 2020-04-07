// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.OpenVR;
using XRTK.Definitions.Controllers.UnityInput;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Inspectors.Data;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Inspectors.Utilities;
using XRTK.Utilities.Editor;

namespace XRTK.Inspectors
{
    public class ControllerPopupWindow : EditorWindow
    {
        private const float InputActionLabelWidth = 128f;

        /// <summary>
        /// Used to enable editing the input axis label positions on controllers
        /// </summary>
        private static readonly bool EnableWysiwyg = false;

        private static readonly GUIContent InteractionAddButtonContent = new GUIContent("+ Add a New Interaction Mapping");
        private static readonly GUIContent InteractionMinusButtonContent = new GUIContent("-", "Remove Interaction Mapping");
        private static readonly GUIContent AxisTypeContent = new GUIContent("Axis Type", "The axis type of the button, e.g. Analogue, Digital, etc.");
        private static readonly GUIContent ControllerInputTypeContent = new GUIContent("Input Type", "The primary action of the input as defined by the controller SDK.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "Action to be raised to the Input Manager when the input data has changed.");
        private static readonly GUIContent KeyCodeContent = new GUIContent("KeyCode", "Unity Input KeyCode id to listen for.");
        private static readonly GUIContent XAxisContent = new GUIContent("X Axis", "Horizontal Axis to listen for.");
        private static readonly GUIContent YAxisContent = new GUIContent("Y Axis", "Vertical Axis to listen for.");
        private static readonly GUIContent InvertContent = new GUIContent("Invert", "Should an Axis be inverted?");
        private static readonly GUIContent[] InvertAxisContent =
        {
            new GUIContent("None"),
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Both")
        };

        private static readonly int[] InvertAxisValues = { 0, 1, 2, 3 };

        private static readonly Vector2 HorizontalSpace = new Vector2(8f, 0f);
        private static readonly Vector2 InputActionLabelPosition = new Vector2(256f, 0f);
        private static readonly Vector2 InputActionDropdownPosition = new Vector2(88f, 0f);
        private static readonly Vector2 InputActionFlipTogglePosition = new Vector2(-24f, 0f);

        private static readonly Rect ControllerRectPosition = new Rect(new Vector2(128f, 0f), new Vector2(512f, 512f));

        private static ControllerPopupWindow window;
        private static ControllerInputActionOptions controllerInputActionOptions;

        private static GUIContent[] axisLabels;
        private static bool[] isMouseInRects;

        private static bool editInputActionPositions;

        private static float defaultLabelWidth;
        private static float defaultFieldWidth;

        private static Vector2 horizontalScrollPosition;

        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        private SerializedProperty currentInteractionList;

        private Handedness currentHandedness;
        private SystemType currentControllerType;

        private Vector2 mouseDragOffset;
        private GUIStyle flippedLabelStyle;
        private Texture2D currentControllerTexture;
        private ControllerInputActionOption currentControllerOption;

        private MixedRealityControllerMappingProfile controllerDataProviderProfile;

        private bool IsCustomController => currentControllerType == typeof(GenericOpenVRController) ||
                                           currentControllerType == typeof(GenericJoystickController);
        private static string EditorWindowOptionsPath => $"{PathFinderUtility.XRTK_Core_RelativeFolderPath}/Inspectors/Data/EditorWindowOptions.json";

        private void OnFocus()
        {
            if (window == null)
            {
                Close();
            }

            currentControllerTexture = ControllerMappingLibrary.GetControllerTexture(controllerDataProviderProfile, currentHandedness);

            #region Interaction Constraint Setup

            axisLabels = ControllerMappingLibrary.UnityInputManagerAxes
                .Select(axis => new GUIContent(axis.Name))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseScroll))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseY))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseX))
                .Prepend(new GUIContent(ControllerMappingLibrary.Vertical))
                .Prepend(new GUIContent(ControllerMappingLibrary.Horizontal))
                .Prepend(new GUIContent("None")).ToArray();

            #endregion  Interaction Constraint Setup
        }

        public static void Show(MixedRealityControllerMappingProfile profile, SerializedProperty interactionsList, Handedness handedness = Handedness.None)
        {
            var handednessTitleText = handedness != Handedness.None ? $"{handedness} Hand " : string.Empty;

            if (window != null)
            {
                window.Close();
            }

            window = (ControllerPopupWindow)CreateInstance(typeof(ControllerPopupWindow));
            window.titleContent = new GUIContent($"{profile.ControllerType} {handednessTitleText}Input Action Assignment");
            window.controllerDataProviderProfile = profile;
            window.currentHandedness = handedness;
            window.currentControllerType = profile.ControllerType;
            window.currentInteractionList = interactionsList;
            window.currentControllerTexture = ControllerMappingLibrary.GetControllerTexture(profile, handedness);

            isMouseInRects = new bool[interactionsList.arraySize];

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorWindowOptionsPath);

            if (asset == null)
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
                if (!controllerInputActionOptions.Controllers.Any(option => option.Controller == window.currentControllerType && option.Handedness == handedness))
                {
                    var newOption = new ControllerInputActionOption
                    {
                        Controller = window.currentControllerType,
                        Handedness = handedness,
                        InputLabelPositions = new Vector2[interactionsList.arraySize],
                        IsLabelFlipped = new bool[interactionsList.arraySize]
                    };

                    controllerInputActionOptions.Controllers.Add(newOption);
                }

                window.currentControllerOption = controllerInputActionOptions.Controllers.FirstOrDefault(option => option.Controller == window.currentControllerType && option.Handedness == handedness);

                if (window.currentControllerOption.IsLabelFlipped == null)
                {
                    window.currentControllerOption.IsLabelFlipped = new bool[interactionsList.arraySize];
                }

                if (window.currentControllerOption.InputLabelPositions == null)
                {
                    window.currentControllerOption.InputLabelPositions = new Vector2[interactionsList.arraySize];
                }
            }

            var windowSize = new Vector2(window.IsCustomController || window.currentControllerTexture == null ? 896f : 768f, 512f);

            window.ShowUtility();
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.CenterOnMainWin();
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;
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
            if (flippedLabelStyle == null)
            {
                flippedLabelStyle = new GUIStyle("Label")
                {
                    alignment = TextAnchor.UpperRight,
                    stretchWidth = true
                };
            }

            if (!IsCustomController && currentControllerTexture != null)
            {
                GUILayout.BeginHorizontal();
                GUI.DrawTexture(ControllerRectPosition, currentControllerTexture);
                GUILayout.EndHorizontal();
            }

            try
            {
                RenderInteractionList(currentInteractionList, IsCustomController || currentControllerTexture == null);
            }
            catch
            {
                Close();
            }
        }

        private void RenderInteractionList(SerializedProperty interactionList, bool useCustomInteractionMapping)
        {
            if (interactionList == null)
            {
                Debug.LogError("No interaction list found!");
                Close();
                return;
            }

            bool noInteractions = interactionList.arraySize == 0;

            if (!useCustomInteractionMapping)
            {
                if (currentControllerOption.IsLabelFlipped == null ||
                    currentControllerOption.IsLabelFlipped.Length != interactionList.arraySize)
                {
                    var newArray = new bool[interactionList.arraySize];

                    for (int i = 0; i < currentControllerOption.IsLabelFlipped?.Length; i++)
                    {
                        newArray[i] = currentControllerOption.IsLabelFlipped[i];
                    }

                    currentControllerOption.IsLabelFlipped = newArray;
                }

                if (currentControllerOption.InputLabelPositions == null ||
                    currentControllerOption.InputLabelPositions.Length != interactionList.arraySize)
                {
                    var newArray = new Vector2[interactionList.arraySize].InitialiseArray(new Vector2(0, 25));

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

            if (useCustomInteractionMapping)
            {
                if (GUILayout.Button(InteractionAddButtonContent))
                {
                    interactionList.arraySize += 1;
                    var interaction = interactionList.GetArrayElementAtIndex(interactionList.arraySize - 1);
                    var axisType = interaction.FindPropertyRelative("axisType");
                    axisType.enumValueIndex = 0;
                    var inputType = interaction.FindPropertyRelative("inputType");
                    inputType.enumValueIndex = 0;
                    var action = interaction.FindPropertyRelative("inputAction");
                    var actionId = action.FindPropertyRelative("id");
                    actionId.intValue = 0;
                    var profileGuid = action.FindPropertyRelative("profileGuid");
                    profileGuid.stringValue = default(Guid).ToString("N");
                    var actionDescription = action.FindPropertyRelative("description");
                    actionDescription.stringValue = "None";
                }

                if (noInteractions)
                {
                    EditorGUILayout.HelpBox("Create an Interaction Mapping.", MessageType.Warning);
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    return;
                }
            }
            else if (EnableWysiwyg)
            {
                EditorGUI.BeginChangeCheck();
                editInputActionPositions = EditorGUILayout.Toggle("Edit Input Action Positions", editInputActionPositions);

                if (EditorGUI.EndChangeCheck())
                {
                    if (!editInputActionPositions)
                    {
                        File.WriteAllText(Path.GetFullPath(EditorWindowOptionsPath), JsonUtility.ToJson(controllerInputActionOptions, true));
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    else
                    {
                        if (!controllerInputActionOptions.Controllers.Any(option => option.Controller == currentControllerType && option.Handedness == currentHandedness))
                        {
                            currentControllerOption = new ControllerInputActionOption
                            {
                                Controller = currentControllerType,
                                Handedness = currentHandedness,
                                InputLabelPositions = new Vector2[currentInteractionList.arraySize],
                                IsLabelFlipped = new bool[currentInteractionList.arraySize]
                            };

                            controllerInputActionOptions.Controllers.Add(currentControllerOption);
                            isMouseInRects = new bool[currentInteractionList.arraySize];

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

            GUILayout.BeginHorizontal();

            if (useCustomInteractionMapping)
            {
                EditorGUILayout.LabelField("Id", GUILayout.Width(32f));
                EditorGUIUtility.labelWidth = 24f;
                EditorGUIUtility.fieldWidth = 24f;
                EditorGUILayout.LabelField(ControllerInputTypeContent, GUILayout.Width(InputActionLabelWidth));
                EditorGUILayout.LabelField(AxisTypeContent, GUILayout.Width(InputActionLabelWidth));
                EditorGUILayout.LabelField(ActionContent, GUILayout.Width(InputActionLabelWidth));
                EditorGUILayout.LabelField(KeyCodeContent, GUILayout.Width(InputActionLabelWidth));
                EditorGUILayout.LabelField(XAxisContent, GUILayout.Width(InputActionLabelWidth));
                EditorGUILayout.LabelField(YAxisContent, GUILayout.Width(InputActionLabelWidth));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));

                EditorGUIUtility.labelWidth = defaultLabelWidth;
                EditorGUIUtility.fieldWidth = defaultFieldWidth;
            }

            GUILayout.EndHorizontal();

            for (int i = 0; i < interactionList.arraySize; i++)
            {
                var interaction = interactionList.GetArrayElementAtIndex(i);
                var axisType = interaction.FindPropertyRelative("axisType");
                var action = interaction.FindPropertyRelative("inputAction");
                var inputType = interaction.FindPropertyRelative("inputType");
                var invertXAxis = interaction.FindPropertyRelative("invertXAxis");
                var invertYAxis = interaction.FindPropertyRelative("invertYAxis");
                var interactionDescription = interaction.FindPropertyRelative("description");

                var axisConstraint = (AxisType)axisType.intValue;

                EditorGUILayout.BeginHorizontal();

                if (useCustomInteractionMapping)
                {
                    EditorGUILayout.LabelField($"{i}", GUILayout.Width(32f));
                    EditorGUILayout.PropertyField(inputType, GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    EditorGUILayout.PropertyField(axisType, GUIContent.none, GUILayout.Width(InputActionLabelWidth));

                    inputActionDropdown.OnGui(GUIContent.none, action, axisConstraint);

                    if (axisConstraint == AxisType.Digital)
                    {
                        EditorGUILayout.PropertyField(interaction.FindPropertyRelative("keyCode"), GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    }
                    else
                    {
                        if (axisConstraint == AxisType.DualAxis)
                        {
                            EditorGUIUtility.labelWidth = InputActionLabelWidth * 0.5f;
                            EditorGUIUtility.fieldWidth = InputActionLabelWidth * 0.5f;

                            int currentAxisSetting = 0;

                            if (invertXAxis.boolValue)
                            {
                                currentAxisSetting += 1;
                            }

                            if (invertYAxis.boolValue)
                            {
                                currentAxisSetting += 2;
                            }

                            EditorGUI.BeginChangeCheck();
                            currentAxisSetting = EditorGUILayout.IntPopup(InvertContent, currentAxisSetting, InvertAxisContent, InvertAxisValues, GUILayout.Width(InputActionLabelWidth));

                            if (EditorGUI.EndChangeCheck())
                            {
                                switch (currentAxisSetting)
                                {
                                    case 0:
                                        invertXAxis.boolValue = false;
                                        invertYAxis.boolValue = false;
                                        break;
                                    case 1:
                                        invertXAxis.boolValue = true;
                                        invertYAxis.boolValue = false;
                                        break;
                                    case 2:
                                        invertXAxis.boolValue = false;
                                        invertYAxis.boolValue = true;
                                        break;
                                    case 3:
                                        invertXAxis.boolValue = true;
                                        invertYAxis.boolValue = true;
                                        break;
                                }
                            }

                            EditorGUIUtility.labelWidth = defaultLabelWidth;
                            EditorGUIUtility.fieldWidth = defaultFieldWidth;
                        }
                        else if (axisConstraint == AxisType.SingleAxis)
                        {
                            invertXAxis.boolValue = EditorGUILayout.ToggleLeft("Invert X", invertXAxis.boolValue, GUILayout.Width(InputActionLabelWidth));
                            EditorGUIUtility.labelWidth = defaultLabelWidth;
                        }
                        else
                        {
                            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                        }
                    }

                    if (axisConstraint == AxisType.SingleAxis ||
                        axisConstraint == AxisType.DualAxis)
                    {
                        var axisCodeX = interaction.FindPropertyRelative("axisCodeX");
                        RenderAxisPopup(axisCodeX, InputActionLabelWidth);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    }

                    if (axisConstraint == AxisType.DualAxis)
                    {
                        var axisCodeY = interaction.FindPropertyRelative("axisCodeY");
                        RenderAxisPopup(axisCodeY, InputActionLabelWidth);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    }

                    if (GUILayout.Button(InteractionMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
                    {
                        interactionList.DeleteArrayElementAtIndex(i);
                    }
                }
                else
                {
                    if (currentControllerTexture == null)
                    {
                        bool skip = false;
                        var description = interactionDescription.stringValue;

                        //if (currentControllerType == SupportedControllerType.WindowsMixedReality && currentHandedness == Handedness.None)
                        //{
                        //    switch (description)
                        //    {
                        //        case "Grip Press":
                        //        case "Trigger Position":
                        //        case "Trigger Touched":
                        //        case "Touchpad Position":
                        //        case "Touchpad Touch":
                        //        case "Touchpad Press":
                        //        case "Menu Press":
                        //        case "Thumbstick Position":
                        //        case "Thumbstick Press":
                        //            skip = true;
                        //            break;
                        //        case "Trigger Press (Select)":
                        //            description = "Air Tap (Select)";
                        //            break;
                        //    }
                        //}

                        if (!skip)
                        {
                            var currentLabelWidth = EditorGUIUtility.labelWidth;

                            if (axisConstraint == AxisType.SingleAxis ||
                                axisConstraint == AxisType.DualAxis)
                            {
                                EditorGUIUtility.labelWidth = 12f;
                                EditorGUILayout.LabelField("Invert:");
                                invertXAxis.boolValue = EditorGUILayout.Toggle("X", invertXAxis.boolValue);
                            }

                            if (axisConstraint == AxisType.DualAxis)
                            {
                                invertYAxis.boolValue = EditorGUILayout.Toggle("Y", invertYAxis.boolValue);
                            }

                            EditorGUIUtility.labelWidth = currentLabelWidth;
                            inputActionDropdown.OnGui(GUIContent.none, action, axisConstraint);
                            EditorGUILayout.LabelField(description, GUILayout.ExpandWidth(true));
                            GUILayout.FlexibleSpace();
                        }
                    }
                    else
                    {
                        var flipped = currentControllerOption.IsLabelFlipped[i];
                        var rectPosition = currentControllerOption.InputLabelPositions[i];
                        var rectSize = InputActionLabelPosition + InputActionDropdownPosition + new Vector2(flipped ? 0f : 8f, EditorGUIUtility.singleLineHeight);

                        GUI.Box(new Rect(rectPosition, rectSize), GUIContent.none, EditorGUIUtility.isProSkin ? "ObjectPickerBackground" : "ObjectPickerResultsEven");

                        var offset = flipped ? InputActionLabelPosition : Vector2.zero;
                        var popupRect = new Rect(rectPosition + offset, new Vector2(InputActionDropdownPosition.x, EditorGUIUtility.singleLineHeight));

                        inputActionDropdown.OnGui(popupRect, action, GUIContent.none, axisConstraint);

                        offset = flipped ? Vector2.zero : InputActionDropdownPosition;
                        var labelRect = new Rect(rectPosition + offset, new Vector2(InputActionLabelPosition.x, EditorGUIUtility.singleLineHeight));
                        EditorGUI.LabelField(labelRect, interactionDescription.stringValue, flipped ? flippedLabelStyle : EditorStyles.label);

                        if (!editInputActionPositions)
                        {
                            offset = flipped
                                ? InputActionLabelPosition + InputActionDropdownPosition + HorizontalSpace
                                : Vector2.zero;

                            if (axisConstraint == AxisType.SingleAxis ||
                                axisConstraint == AxisType.DualAxis)
                            {
                                if (!flipped)
                                {
                                    if (axisConstraint == AxisType.DualAxis)
                                    {
                                        offset += new Vector2(-112f, 0f);
                                    }
                                    else
                                    {
                                        offset += new Vector2(-76f, 0f);
                                    }
                                }

                                var boxSize = axisConstraint == AxisType.DualAxis ? new Vector2(112f, EditorGUIUtility.singleLineHeight) : new Vector2(76f, EditorGUIUtility.singleLineHeight);

                                GUI.Box(new Rect(rectPosition + offset, boxSize), GUIContent.none, EditorGUIUtility.isProSkin ? "ObjectPickerBackground" : "ObjectPickerResultsEven");

                                labelRect = new Rect(rectPosition + offset, new Vector2(48f, EditorGUIUtility.singleLineHeight));
                                EditorGUI.LabelField(labelRect, "Invert X", flipped ? flippedLabelStyle : EditorStyles.label);
                                offset += new Vector2(52f, 0f);
                                var toggleXAxisRect = new Rect(rectPosition + offset, new Vector2(12f, EditorGUIUtility.singleLineHeight));
                                invertXAxis.boolValue = EditorGUI.Toggle(toggleXAxisRect, invertXAxis.boolValue);
                            }

                            if (axisConstraint == AxisType.DualAxis)
                            {
                                offset += new Vector2(24f, 0f);
                                labelRect = new Rect(rectPosition + offset, new Vector2(8f, EditorGUIUtility.singleLineHeight));
                                EditorGUI.LabelField(labelRect, "Y", flipped ? flippedLabelStyle : EditorStyles.label);
                                offset += new Vector2(12f, 0f);
                                var toggleYAxisRect = new Rect(rectPosition + offset, new Vector2(12f, EditorGUIUtility.singleLineHeight));
                                invertYAxis.boolValue = EditorGUI.Toggle(toggleYAxisRect, invertYAxis.boolValue);
                            }
                        }
                        else
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
                                    case EventType.MouseDrag when labelRect.Contains(Event.current.mousePosition) && !isMouseInRects[i]:
                                        isMouseInRects[i] = true;
                                        mouseDragOffset = Event.current.mousePosition - currentControllerOption.InputLabelPositions[i];
                                        break;
                                    case EventType.Repaint when isMouseInRects[i]:
                                    case EventType.DragUpdated when isMouseInRects[i]:
                                        currentControllerOption.InputLabelPositions[i] = Event.current.mousePosition - mouseDragOffset;
                                        break;
                                    case EventType.MouseUp when isMouseInRects[i]:
                                        currentControllerOption.InputLabelPositions[i] = Event.current.mousePosition - mouseDragOffset;
                                        mouseDragOffset = Vector2.zero;
                                        isMouseInRects[i] = false;
                                        break;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (useCustomInteractionMapping)
            {
                EditorGUILayout.EndScrollView();
            }

            interactionList.serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        }

        private static void RenderAxisPopup(SerializedProperty axisCode, float customLabelWidth)
        {
            int axisId = -1;

            for (int i = 0; i < axisLabels.Length; i++)
            {
                if (axisLabels[i].text.Equals(axisCode.stringValue))
                {
                    axisId = i;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();
            axisId = EditorGUILayout.IntPopup(GUIContent.none, axisId, axisLabels, null, GUILayout.Width(customLabelWidth));

            if (EditorGUI.EndChangeCheck())
            {
                axisCode.stringValue = axisId == 0
                    ? string.Empty
                    : axisLabels[axisId].text;
                axisCode.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
