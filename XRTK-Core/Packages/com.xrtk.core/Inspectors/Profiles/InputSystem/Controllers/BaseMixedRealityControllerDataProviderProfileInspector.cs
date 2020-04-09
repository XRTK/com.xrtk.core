// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Extensions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Providers.Controllers;

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
                        if (Activator.CreateInstance(new SystemType(controllerTypeProperty.stringValue), null, TrackingState.NotTracked, (Handedness)handednessProperty.intValue, null, null) is BaseController detectedController)
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

                            for (int j = 0; j < defaultMappings.Length; j++)
                            {
                                var interactionMappingProfileAsset = CreateInstance(nameof(MixedRealityInteractionMappingProfile)).CreateAsset($"{mappingProfileRootPath}/", $"{defaultMappings[j].Description}Profile", false) as MixedRealityInteractionMappingProfile;
                                Debug.Assert(interactionMappingProfileAsset != null);
                                var mapping = defaultMappings[j];

                                var interactionMappingProfileSerializedObject = new SerializedObject(interactionMappingProfileAsset);
                                interactionMappingProfileSerializedObject.Update();

                                var interactionMappingProperty = interactionMappingProfileSerializedObject.FindProperty("interactionMapping");

                                var descriptionProperty = interactionMappingProperty.FindPropertyRelative("description");
                                var stateChangeTypeProperty = interactionMappingProperty.FindPropertyRelative("stateChangeType");
                                var inputNameProperty = interactionMappingProperty.FindPropertyRelative("inputName");
                                var axisTypeProperty = interactionMappingProperty.FindPropertyRelative("axisType");
                                var inputTypeProperty = interactionMappingProperty.FindPropertyRelative("inputType");
                                var keyCodeProperty = interactionMappingProperty.FindPropertyRelative("keyCode");
                                var axisCodeXProperty = interactionMappingProperty.FindPropertyRelative("axisCodeX");
                                var axisCodeYProperty = interactionMappingProperty.FindPropertyRelative("axisCodeY");
                                var inputProcessorsProperty = interactionMappingProperty.FindPropertyRelative("inputProcessors");

                                descriptionProperty.stringValue = mapping.Description;
                                stateChangeTypeProperty.intValue = (int)mapping.StateChangeType;
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

            serializedObject.Update();

            showMappingProfiles = EditorGUILayout.Foldout(showMappingProfiles, new GUIContent("Controller Mapping Profiles"), true);

            if (showMappingProfiles)
            {
                mappingProfileList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
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