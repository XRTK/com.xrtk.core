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
                var defaultControllerOptions = dataProviderProfile.GetDefaultControllerOptions();

                Debug.Assert(defaultControllerOptions != null, $"Missing default controller definitions for {dataProviderProfile.name}");

                var defaultProfiles = new MixedRealityControllerMappingProfile[defaultControllerOptions.Length];
                var profileRootPath = AssetDatabase.GetAssetPath(dataProviderProfile);

                controllerMappingProfiles.ClearArray();

                for (int i = 0; i < defaultProfiles.Length; i++)
                {
                    var instance = CreateInstance(nameof(MixedRealityControllerMappingProfile)).CreateAsset($"{profileRootPath}/{defaultControllerOptions[i].Description}/", $"{defaultControllerOptions[i].Description}Profile", false) as MixedRealityControllerMappingProfile;
                    Debug.Assert(instance != null);
                    instance.ControllerType = defaultControllerOptions[i].ControllerType;
                    instance.Handedness = defaultControllerOptions[i].Handedness;
                    instance.UseCustomInteractions = defaultControllerOptions[i].UseCustomInteractions;
                    defaultProfiles[i] = instance;
                    controllerMappingProfiles.InsertArrayElementAtIndex(i);
                    var mappingProfile = controllerMappingProfiles.GetArrayElementAtIndex(i);
                    mappingProfile.objectReferenceValue = instance;
                    SetDefaultInteractionMapping(instance);
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

        private void SetDefaultInteractionMapping(MixedRealityControllerMappingProfile mappingProfile)
        {
            if (Activator.CreateInstance(mappingProfile.ControllerType, null, TrackingState.NotTracked, mappingProfile.Handedness, null, null) is BaseController detectedController)
            {
                switch (mappingProfile.Handedness)
                {
                    case Handedness.Left:
                        mappingProfile.InteractionMappingProfiles = CreateDefaultMappingProfiles(detectedController.DefaultLeftHandedInteractions);
                        break;
                    case Handedness.Right:
                        mappingProfile.InteractionMappingProfiles = CreateDefaultMappingProfiles(detectedController.DefaultRightHandedInteractions);
                        break;
                    default:
                        mappingProfile.InteractionMappingProfiles = CreateDefaultMappingProfiles(detectedController.DefaultInteractions);
                        break;
                }
            }

            MixedRealityInteractionMappingProfile[] CreateDefaultMappingProfiles(MixedRealityInteractionMapping[] defaultMappings)
            {
                var mappingProfiles = new MixedRealityInteractionMappingProfile[defaultMappings.Length];
                var profileRootPath = AssetDatabase.GetAssetPath(mappingProfile);

                for (int i = 0; i < defaultMappings.Length; i++)
                {
                    var instance = CreateInstance(nameof(MixedRealityInteractionMappingProfile)).CreateAsset($"{profileRootPath}/", $"{defaultMappings[i].Description}Profile", false) as MixedRealityInteractionMappingProfile;
                    Debug.Assert(instance != null);
                    instance.InteractionMapping = defaultMappings[i];
                    mappingProfiles[i] = instance;
                }

                return mappingProfiles;
            }
        }
    }
}