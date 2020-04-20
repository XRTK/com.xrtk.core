﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Extensions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Providers.Controllers;
using XRTK.Extensions;
using System.Collections.Generic;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseMixedRealityControllerDataProviderProfile), true, isFallback = true)]
    public class BaseMixedRealityControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty hasSetupDefaults;
        private SerializedProperty controllerMappingProfiles;

        private BaseMixedRealityControllerDataProviderProfile dataProviderProfile;
        private bool showMappingProfiles = true;

        private ReorderableList mappingProfileList;
        private int currentlySelectedElement;
        private int selectedMappingsViewModeTab = 0;
        private GUIStyle controllerButtonStyle;

        private static readonly GUIContent controllerProfilesFoldoutHeader = new GUIContent("Controller Mapping Profiles");
        private static readonly string[] viewModeToolbarOptions = new string[] { "Simple", "Advanced" };
        private static readonly List<int> nullElementIndexes = new List<int>();

        protected override void OnEnable()
        {
            base.OnEnable();

            serializedObject.Update();

            hasSetupDefaults = serializedObject.FindProperty(nameof(hasSetupDefaults));
            controllerMappingProfiles = serializedObject.FindProperty(nameof(controllerMappingProfiles));

            dataProviderProfile = (BaseMixedRealityControllerDataProviderProfile)serializedObject.targetObject;

            if (!hasSetupDefaults.boolValue)
            {
                var defaultControllerMappingProfiles = dataProviderProfile.GetDefaultControllerOptions();

                Debug.Assert(defaultControllerMappingProfiles != null, $"Missing default controller definitions for {dataProviderProfile.name}");

                var profileRootPath = AssetDatabase.GetAssetPath(dataProviderProfile);
                profileRootPath = $"{Directory.GetParent(Directory.GetParent(profileRootPath).FullName).FullName}/DataProviders";

                controllerMappingProfiles.ClearArray();

                for (int i = 0; i < defaultControllerMappingProfiles.Length; i++)
                {
                    var defaultControllerMappingProfile = defaultControllerMappingProfiles[i];
                    var controllerMappingAsset = CreateInstance(nameof(MixedRealityControllerMappingProfile)).CreateAsset($"{profileRootPath}/", $"{defaultControllerMappingProfile.Description}Profile", false) as MixedRealityControllerMappingProfile;

                    Debug.Assert(controllerMappingAsset != null);

                    var mappingProfileSerializedObject = new SerializedObject(controllerMappingAsset);
                    mappingProfileSerializedObject.Update();

                    var controllerTypeProperty = mappingProfileSerializedObject.FindProperty("controllerType").FindPropertyRelative("reference");
                    var handednessProperty = mappingProfileSerializedObject.FindProperty("handedness");
                    var useCustomInteractionsProperty = mappingProfileSerializedObject.FindProperty("useCustomInteractions");
                    var interactionMappingProfilesProperty = mappingProfileSerializedObject.FindProperty("interactionMappingProfiles");

                    controllerTypeProperty.stringValue = SystemType.GetReference(defaultControllerMappingProfile.ControllerType);
                    handednessProperty.intValue = (int)defaultControllerMappingProfile.Handedness;
                    useCustomInteractionsProperty.boolValue = defaultControllerMappingProfile.UseCustomInteractions;

                    SetDefaultInteractionMapping();
                    mappingProfileSerializedObject.ApplyModifiedProperties();

                    controllerMappingProfiles.InsertArrayElementAtIndex(i);
                    var mappingProfile = controllerMappingProfiles.GetArrayElementAtIndex(i);
                    mappingProfile.objectReferenceValue = controllerMappingAsset;
                    mappingProfile.serializedObject.ApplyModifiedProperties();

                    void SetDefaultInteractionMapping()
                    {
                        if (Activator.CreateInstance(new SystemType(controllerTypeProperty.stringValue)) is BaseController detectedController)
                        {
                            interactionMappingProfilesProperty.ClearArray();

                            switch ((Handedness)handednessProperty.intValue)
                            {
                                case Handedness.Left:
                                    CreateDefaultMappingProfiles(detectedController.DefaultLeftHandedInteractions);
                                    break;
                                case Handedness.Right:
                                    CreateDefaultMappingProfiles(detectedController.DefaultRightHandedInteractions);
                                    break;
                                default:
                                    CreateDefaultMappingProfiles(detectedController.DefaultInteractions);
                                    break;
                            }
                        }

                        void CreateDefaultMappingProfiles(MixedRealityInteractionMapping[] defaultMappings)
                        {
                            var mappingProfileRootPath = AssetDatabase.GetAssetPath(controllerMappingAsset);
                            mappingProfileRootPath = mappingProfileRootPath.Replace("DataProviderProfile", string.Empty);

                            for (int j = 0; j < defaultMappings.Length; j++)
                            {
                                var interactionMappingProfileAsset = CreateInstance(nameof(MixedRealityInteractionMappingProfile)).CreateAsset($"{mappingProfileRootPath}/", $"{defaultMappings[j].Description}Profile", false) as MixedRealityInteractionMappingProfile;
                                Debug.Assert(interactionMappingProfileAsset != null);
                                var mapping = defaultMappings[j];

                                var interactionMappingProfileSerializedObject = new SerializedObject(interactionMappingProfileAsset);
                                interactionMappingProfileSerializedObject.Update();

                                var interactionMappingProperty = interactionMappingProfileSerializedObject.FindProperty("interactionMapping");

                                var descriptionProperty = interactionMappingProperty.FindPropertyRelative("description");
                                //var stateChangeTypeProperty = interactionMappingProperty.FindPropertyRelative("stateChangeType");
                                var inputNameProperty = interactionMappingProperty.FindPropertyRelative("inputName");
                                var axisTypeProperty = interactionMappingProperty.FindPropertyRelative("axisType");
                                var inputTypeProperty = interactionMappingProperty.FindPropertyRelative("inputType");
                                var keyCodeProperty = interactionMappingProperty.FindPropertyRelative("keyCode");
                                var axisCodeXProperty = interactionMappingProperty.FindPropertyRelative("axisCodeX");
                                var axisCodeYProperty = interactionMappingProperty.FindPropertyRelative("axisCodeY");
                                var inputProcessorsProperty = interactionMappingProperty.FindPropertyRelative("inputProcessors");

                                descriptionProperty.stringValue = mapping.Description;
                                //stateChangeTypeProperty.intValue = (int)mapping.StateChangeType;
                                inputNameProperty.stringValue = mapping.InputName;
                                axisTypeProperty.intValue = (int)mapping.AxisType;
                                inputTypeProperty.intValue = (int)mapping.InputType;
                                keyCodeProperty.intValue = (int)mapping.KeyCode;
                                axisCodeXProperty.stringValue = mapping.AxisCodeX;
                                axisCodeYProperty.stringValue = mapping.AxisCodeY;

                                for (int k = 0; k < mapping.InputProcessors.Count; k++)
                                {
                                    inputProcessorsProperty.InsertArrayElementAtIndex(k);
                                    var processorProperty = inputProcessorsProperty.GetArrayElementAtIndex(k);
                                    var processor = mapping.InputProcessors[k];
                                    var processorAsset = processor.GetOrCreateAsset($"{mappingProfileRootPath}/", false);
                                    processorProperty.objectReferenceValue = processorAsset;
                                }

                                interactionMappingProfileSerializedObject.ApplyModifiedProperties();

                                interactionMappingProfilesProperty.InsertArrayElementAtIndex(j);
                                var interactionsProperty = interactionMappingProfilesProperty.GetArrayElementAtIndex(j);
                                interactionsProperty.objectReferenceValue = interactionMappingProfileAsset;
                            }
                        }
                    }
                }

                hasSetupDefaults.boolValue = true;

                serializedObject.ApplyModifiedProperties();
            }

            mappingProfileList = new ReorderableList(serializedObject, controllerMappingProfiles, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            mappingProfileList.drawElementCallback += DrawConfigurationOptionElement;
            mappingProfileList.onAddCallback += OnConfigurationOptionAdded;
            mappingProfileList.onRemoveCallback += OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines all of the controllers and their mappings to use. Additional platform settings can also be available as well.");

            showMappingProfiles = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showMappingProfiles, controllerProfilesFoldoutHeader, true);
            if (showMappingProfiles)
            {
                EditorGUILayout.Space();
                selectedMappingsViewModeTab = GUILayout.Toolbar(selectedMappingsViewModeTab, viewModeToolbarOptions);
                switch (selectedMappingsViewModeTab)
                {
                    case 0:
                        DrawSimpleControllerMappingProfilesView();
                        break;
                    case 1:
                        DrawAdvancedControllerMappingProfilesView();
                        break;
                }
            }

            EditorGUILayout.Space();
        }

        private void DrawAdvancedControllerMappingProfilesView()
        {
            serializedObject.Update();
            mappingProfileList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSimpleControllerMappingProfilesView()
        {
            if (controllerMappingProfiles == null) { return; }

            // Clear found null element indexes from previous loop.
            nullElementIndexes.Clear();

            for (int i = 0; i < controllerMappingProfiles.arraySize; i++)
            {
                var targetObjectReference = controllerMappingProfiles.GetArrayElementAtIndex(i).objectReferenceValue;
                var controllerMappingProfile = (MixedRealityControllerMappingProfile)targetObjectReference;

                // In advanced mode new profile entries might have been created
                // but not assigned, which leads to null entries in the mapping profiles list.
                // We can safely ignore those for the simplified view but will remember the index
                // to remove them later on and cleanup the list of nulls.
                if (controllerMappingProfile != null)
                {
                    RenderControllerMappingButton(controllerMappingProfile);
                }
                else
                {
                    nullElementIndexes.Add(i);
                }
            }

            if (nullElementIndexes.Count > 0)
            {
                serializedObject.Update();

                for (int i = 0; i < nullElementIndexes.Count; i++)
                {
                    // Every time we remove an element from the mapping profiles list,
                    // we need to reduce the null element index by the count of already removed items,
                    // since the list is shrinking.
                    controllerMappingProfiles.DeleteArrayElementAtIndex(nullElementIndexes[i] - i);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        internal void RenderControllerMappingButton(MixedRealityControllerMappingProfile controllerMappingProfile)
        {
            if (controllerButtonStyle == null)
            {
                controllerButtonStyle = new GUIStyle("LargeButton")
                {
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold,
                    stretchHeight = true,
                    stretchWidth = true,
                    wordWrap = true,
                    fontSize = 10,
                };
            }

            var handedness = controllerMappingProfile.Handedness;

            if (handedness != Handedness.Right)
            {
                GUILayout.BeginHorizontal();
            }

            var typeName = controllerMappingProfile.ControllerType.Type.Name.ToProperCase();

            if (controllerMappingProfile.ControllerType.Type.Name == "WindowsMixedRealityMotionController" && controllerMappingProfile.Handedness == Handedness.None)
            {
                typeName = "HoloLens 1";
            }

            var buttonContent = new GUIContent($"Edit {typeName} Action Mapping", ControllerMappingLibrary.GetControllerTextureScaled(controllerMappingProfile));
            if (GUILayout.Button(buttonContent, controllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
            {
                EditorApplication.delayCall += () => ControllerPopupWindow.Show(controllerMappingProfile, new SerializedObject(controllerMappingProfile).FindProperty("interactionMappingProfiles"));
            }

            if (handedness != Handedness.Left)
            {
                GUILayout.EndHorizontal();
            }
        }

        private void DrawConfigurationOptionElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedElement = index;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += 3;
            var mappingProfileProperty = controllerMappingProfiles.GetArrayElementAtIndex(index);
            MixedRealityProfilePropertyDrawer.ProfileTypeOverride = typeof(MixedRealityControllerMappingProfile);
            EditorGUI.PropertyField(position, mappingProfileProperty, GUIContent.none);
        }

        private void OnConfigurationOptionAdded(ReorderableList list)
        {
            controllerMappingProfiles.arraySize += 1;
            var index = controllerMappingProfiles.arraySize - 1;

            var mappingProfileProperty = controllerMappingProfiles.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedElement >= 0)
            {
                controllerMappingProfiles.DeleteArrayElementAtIndex(currentlySelectedElement);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}