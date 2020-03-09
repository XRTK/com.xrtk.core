// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Data;
using XRTK.Inspectors.Utilities;
using XRTK.Definitions.Controllers;
using XRTK.Utilities.Editor;
using XRTK.Extensions;

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
        private static int[] actionIds;
        private static GUIContent[] actionLabels;
        private static int[] rawActionIds;
        private static GUIContent[] rawActionLabels;
        private static int[] digitalActionIds;
        private static GUIContent[] digitalActionLabels;
        private static int[] singleAxisActionIds;
        private static GUIContent[] singleAxisActionLabels;
        private static int[] dualAxisActionIds;
        private static GUIContent[] dualAxisActionLabels;
        private static int[] threeDofPositionActionIds;
        private static GUIContent[] threeDofPositionActionLabels;
        private static int[] threeDofRotationActionIds;
        private static GUIContent[] threeDofRotationActionLabels;
        private static int[] sixDofActionIds;
        private static GUIContent[] sixDofActionLabels;
        private static bool[] isMouseInRects;

        private static bool editInputActionPositions;

        private static float defaultLabelWidth;
        private static float defaultFieldWidth;

        private static Vector2 horizontalScrollPosition;

        private bool isLocked = false;
        private SerializedProperty currentInteractionList;

        private Handedness currentHandedness;
        private SupportedControllerType currentControllerType;

        private Vector2 mouseDragOffset;
        private GUIStyle flippedLabelStyle;
        private Texture2D currentControllerTexture;
        private ControllerInputActionOption currentControllerOption;

        private MixedRealityInputSystemProfile inputSystemProfile;
        private BaseMixedRealityControllerMappingProfile mappingProfile;

        private bool IsCustomController => currentControllerType == SupportedControllerType.GenericOpenVR ||
                                           currentControllerType == SupportedControllerType.GenericUnity;
        private static string EditorWindowOptionsPath => $"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/Inspectors/Data/EditorWindowOptions.json";

        private void OnFocus()
        {
            if (window == null)
            {
                Close();
            }

            currentControllerTexture = ControllerMappingLibrary.GetControllerTexture(mappingProfile, currentControllerType, currentHandedness);
            inputSystemProfile = mappingProfile.ParentProfile.ParentProfile as MixedRealityInputSystemProfile;

            if (inputSystemProfile == null)
            {
                Debug.LogWarning("No Input System Profile found. Be sure to assign this mapping profile to an input system.");
                return;
            }

            #region Interaction Constraint Setup

            actionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            axisLabels = ControllerMappingLibrary.UnityInputManagerAxes
                .Select(axis => new GUIContent(axis.Name))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseScroll))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseY))
                .Prepend(new GUIContent(ControllerMappingLibrary.MouseX))
                .Prepend(new GUIContent(ControllerMappingLibrary.Vertical))
                .Prepend(new GUIContent(ControllerMappingLibrary.Horizontal))
                .Prepend(new GUIContent("None")).ToArray();

            actionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.None)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            actionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.None)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            rawActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.Raw)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            rawActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                 .Where(inputAction => inputAction.AxisConstraint == AxisType.Raw)
                 .Select(inputAction => new GUIContent(inputAction.Description))
                 .Prepend(new GUIContent("None")).ToArray();

            digitalActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.Digital)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            digitalActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.Digital)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            singleAxisActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.SingleAxis)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            singleAxisActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.SingleAxis)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            dualAxisActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.DualAxis)
                .Select(action => (int)action.Id).Prepend(0).ToArray();

            dualAxisActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.DualAxis)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            threeDofPositionActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.ThreeDofPosition)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            threeDofPositionActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.ThreeDofPosition)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            threeDofRotationActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.ThreeDofRotation)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            threeDofRotationActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.ThreeDofRotation)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            sixDofActionIds = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.SixDof)
                .Select(action => (int)action.Id)
                .Prepend(0).ToArray();

            sixDofActionLabels = inputSystemProfile.InputActionsProfile.InputActions
                .Where(inputAction => inputAction.AxisConstraint == AxisType.SixDof)
                .Select(inputAction => new GUIContent(inputAction.Description))
                .Prepend(new GUIContent("None")).ToArray();

            #endregion  Interaction Constraint Setup
        }

        public static void Show(BaseMixedRealityControllerMappingProfile profile, SupportedControllerType controllerType, SerializedProperty interactionsList, Handedness handedness = Handedness.None, bool isLocked = false)
        {
            var handednessTitleText = handedness != Handedness.None ? $"{handedness} Hand " : string.Empty;

            if (window != null)
            {
                window.Close();
            }

            window = (ControllerPopupWindow)CreateInstance(typeof(ControllerPopupWindow));
            window.titleContent = new GUIContent($"{controllerType} {handednessTitleText}Input Action Assignment");
            window.isLocked = isLocked;
            window.mappingProfile = profile;
            window.currentHandedness = handedness;
            window.currentControllerType = controllerType;
            window.currentInteractionList = interactionsList;
            window.currentControllerTexture = ControllerMappingLibrary.GetControllerTexture(profile, controllerType, handedness);

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
                            Controller = SupportedControllerType.None,
                            Handedness = Handedness.None,
                            InputLabelPositions = new[] {new Vector2(0, 0)},
                            IsLabelFlipped = new []{false}
                        }
                    }
                };

                File.WriteAllText(Path.GetFullPath(EditorWindowOptionsPath), JsonUtility.ToJson(empty, true));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                controllerInputActionOptions = JsonUtility.FromJson<ControllerInputActionOptions>(asset.text);

                if (controllerInputActionOptions.Controllers.Any(option => option.Controller == controllerType && option.Handedness == handedness))
                {
                    window.currentControllerOption = controllerInputActionOptions.Controllers.FirstOrDefault(option => option.Controller == controllerType && option.Handedness == handedness);

                    if (window.currentControllerOption.IsLabelFlipped == null)
                    {
                        window.currentControllerOption.IsLabelFlipped = new bool[interactionsList.arraySize];
                    }

                    if (window.currentControllerOption.InputLabelPositions == null)
                    {
                        window.currentControllerOption.InputLabelPositions = new Vector2[interactionsList.arraySize];
                    }
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
            GUI.enabled = !isLocked;

            if (interactionList == null)
            {
                Debug.LogError("No interaction list found!");
                Close();
                return;
            }

            bool noInteractions = interactionList.arraySize == 0;

            if (!useCustomInteractionMapping)
            {
                if (currentControllerOption.IsLabelFlipped == null || currentControllerOption.IsLabelFlipped.Length != interactionList.arraySize)
                {
                    var newArray = new bool[interactionList.arraySize];

                    for (int i = 0; i < currentControllerOption.IsLabelFlipped?.Length; i++)
                    {
                        newArray[i] = currentControllerOption.IsLabelFlipped[i];
                    }

                    currentControllerOption.IsLabelFlipped = newArray;
                }

                if (currentControllerOption.InputLabelPositions == null || currentControllerOption.InputLabelPositions.Length != interactionList.arraySize)
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
                useCustomInteractionMapping = !(currentControllerType == SupportedControllerType.WindowsMixedReality && currentHandedness == Handedness.None);
            }

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
                    var actionDescription = action.FindPropertyRelative("description");
                    actionDescription.stringValue = "None";
                    actionId.intValue = 0;
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

                            if (controllerInputActionOptions.Controllers.Any(option => option.Controller == SupportedControllerType.None))
                            {
                                controllerInputActionOptions.Controllers.Remove(
                                    controllerInputActionOptions.Controllers.Find(option =>
                                        option.Controller == SupportedControllerType.None));
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
                EditorGUILayout.BeginHorizontal();
                var interaction = interactionList.GetArrayElementAtIndex(i);

                if (useCustomInteractionMapping)
                {
                    EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(32f));
                    var inputType = interaction.FindPropertyRelative("inputType");
                    EditorGUILayout.PropertyField(inputType, GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    var axisType = interaction.FindPropertyRelative("axisType");
                    EditorGUILayout.PropertyField(axisType, GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    var invertXAxis = interaction.FindPropertyRelative("invertXAxis");
                    var invertYAxis = interaction.FindPropertyRelative("invertYAxis");
                    var interactionAxisConstraint = interaction.FindPropertyRelative("axisType");

                    var action = interaction.FindPropertyRelative("inputAction");
                    var actionId = action.FindPropertyRelative("id");
                    var actionDescription = action.FindPropertyRelative("description");
                    var actionConstraint = action.FindPropertyRelative("axisConstraint");

                    GUIContent[] labels;
                    int[] ids;

                    switch ((AxisType)interactionAxisConstraint.intValue)
                    {
                        // case AxisType.None:
                        default:
                            labels = actionLabels;
                            ids = actionIds;
                            break;
                        case AxisType.Raw:
                            labels = rawActionLabels;
                            ids = rawActionIds;
                            break;
                        case AxisType.Digital:
                            labels = digitalActionLabels;
                            ids = digitalActionIds;
                            break;
                        case AxisType.SingleAxis:
                            labels = singleAxisActionLabels;
                            ids = singleAxisActionIds;
                            break;
                        case AxisType.DualAxis:
                            labels = dualAxisActionLabels;
                            ids = dualAxisActionIds;
                            break;
                        case AxisType.ThreeDofPosition:
                            labels = threeDofPositionActionLabels;
                            ids = threeDofPositionActionIds;
                            break;
                        case AxisType.ThreeDofRotation:
                            labels = threeDofRotationActionLabels;
                            ids = threeDofRotationActionIds;
                            break;
                        case AxisType.SixDof:
                            labels = sixDofActionLabels;
                            ids = sixDofActionIds;
                            break;
                    }

                    EditorGUI.BeginChangeCheck();
                    actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, actionId.intValue, labels, ids, GUILayout.Width(InputActionLabelWidth));

                    if (EditorGUI.EndChangeCheck())
                    {
                        var inputAction = actionId.intValue == 0 ? MixedRealityInputAction.None : inputSystemProfile.InputActionsProfile.InputActions[actionId.intValue - 1];
                        actionDescription.stringValue = inputAction.Description;
                        actionConstraint.enumValueIndex = (int)inputAction.AxisConstraint;
                    }

                    if ((AxisType)axisType.intValue == AxisType.Digital)
                    {
                        var keyCode = interaction.FindPropertyRelative("keyCode");
                        EditorGUILayout.PropertyField(keyCode, GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    }
                    else
                    {
                        if ((AxisType)axisType.intValue == AxisType.DualAxis)
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
                        else if ((AxisType)axisType.intValue == AxisType.SingleAxis)
                        {
                            invertXAxis.boolValue = EditorGUILayout.ToggleLeft("Invert X", invertXAxis.boolValue, GUILayout.Width(InputActionLabelWidth));
                            EditorGUIUtility.labelWidth = defaultLabelWidth;
                        }
                        else
                        {
                            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                        }
                    }

                    if ((AxisType)axisType.intValue == AxisType.SingleAxis ||
                        (AxisType)axisType.intValue == AxisType.DualAxis)
                    {
                        var axisCodeX = interaction.FindPropertyRelative("axisCodeX");
                        RenderAxisPopup(axisCodeX, InputActionLabelWidth);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(InputActionLabelWidth));
                    }

                    if ((AxisType)axisType.intValue == AxisType.DualAxis)
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
                    var interactionDescription = interaction.FindPropertyRelative("description");
                    var interactionAxisConstraint = interaction.FindPropertyRelative("axisType");
                    var action = interaction.FindPropertyRelative("inputAction");
                    var actionId = action.FindPropertyRelative("id");
                    var actionDescription = action.FindPropertyRelative("description");
                    var actionConstraint = action.FindPropertyRelative("axisConstraint");
                    var invertXAxis = interaction.FindPropertyRelative("invertXAxis");
                    var invertYAxis = interaction.FindPropertyRelative("invertYAxis");

                    int[] ids;
                    GUIContent[] labels;
                    var axisConstraint = (AxisType)interactionAxisConstraint.intValue;

                    switch (axisConstraint)
                    {
                        // case AxisType.None:
                        default:
                            labels = actionLabels;
                            ids = actionIds;
                            break;
                        case AxisType.Raw:
                            labels = rawActionLabels;
                            ids = rawActionIds;
                            break;
                        case AxisType.Digital:
                            labels = digitalActionLabels;
                            ids = digitalActionIds;
                            break;
                        case AxisType.SingleAxis:
                            labels = singleAxisActionLabels;
                            ids = singleAxisActionIds;
                            break;
                        case AxisType.DualAxis:
                            labels = dualAxisActionLabels;
                            ids = dualAxisActionIds;
                            break;
                        case AxisType.ThreeDofPosition:
                            labels = threeDofPositionActionLabels;
                            ids = threeDofPositionActionIds;
                            break;
                        case AxisType.ThreeDofRotation:
                            labels = threeDofRotationActionLabels;
                            ids = threeDofRotationActionIds;
                            break;
                        case AxisType.SixDof:
                            labels = sixDofActionLabels;
                            ids = sixDofActionIds;
                            break;
                    }

                    EditorGUI.BeginChangeCheck();

                    if (currentControllerTexture == null)
                    {
                        bool skip = false;
                        var description = interactionDescription.stringValue;

                        if (currentControllerType == SupportedControllerType.WindowsMixedReality && currentHandedness == Handedness.None)
                        {
                            switch (description)
                            {
                                case "Grip Press":
                                case "Trigger Position":
                                case "Trigger Touched":
                                case "Touchpad Position":
                                case "Touchpad Touch":
                                case "Touchpad Press":
                                case "Menu Press":
                                case "Thumbstick Position":
                                case "Thumbstick Press":
                                    skip = true;
                                    break;
                                case "Trigger Press (Select)":
                                    description = "Air Tap (Select)";
                                    break;
                            }
                        }

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
                            actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, actionId.intValue, labels, ids, GUILayout.Width(80f));
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

                        actionId.intValue = EditorGUI.IntPopup(popupRect, actionId.intValue, labels, ids);
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

                    if (EditorGUI.EndChangeCheck())
                    {
                        var inputAction = actionId.intValue == 0
                            ? MixedRealityInputAction.None
                            : inputSystemProfile.InputActionsProfile.InputActions[actionId.intValue - 1];
                        actionId.intValue = (int)inputAction.Id;
                        actionDescription.stringValue = inputAction.Description;
                        actionConstraint.enumValueIndex = (int)inputAction.AxisConstraint;
                        interactionList.serializedObject.ApplyModifiedProperties();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (useCustomInteractionMapping)
            {
                EditorGUILayout.EndScrollView();
                interactionList.serializedObject.ApplyModifiedProperties();
            }

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
