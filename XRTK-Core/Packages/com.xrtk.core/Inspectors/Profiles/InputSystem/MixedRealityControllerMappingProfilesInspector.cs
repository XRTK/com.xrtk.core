// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfiles))]
    public class MixedRealityControllerMappingProfilesInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent AddMappingDefinitionContent = new GUIContent("+ Add a new Mapping Definition");
        private static readonly GUIContent RemoveMappingDefinitionContent = new GUIContent("-", "Remove Mapping Definition");

        private SerializedProperty controllerMappingProfiles;
        private List<Type> mappingTypes;
        private bool changed;
        private Rect dropdownRect;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerMappingProfiles = serializedObject.FindProperty(nameof(controllerMappingProfiles));

            mappingTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(BaseMixedRealityControllerMappingProfile).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToList();
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Controller Input Mappings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the controllers and their inputs your users will be able to use in your application.\n\n" +
                                    "You'll want to define all your Input Actions and Controller Data Providers first so you can wire up actions to hardware sensors, controllers, gestures, and other input devices.", MessageType.Info);
            EditorGUILayout.Space();

            ThisProfile.CheckProfileLock();
            serializedObject.Update();

            EditorGUILayout.LabelField("Select a profile type:");

            changed = false;

            var showDropdown = EditorGUILayout.DropdownButton(AddMappingDefinitionContent, FocusType.Keyboard);

            if (Event.current.type == EventType.Repaint)
            {
                dropdownRect = GUILayoutUtility.GetLastRect();
            }

            if (showDropdown)
            {
                TypeReferencePropertyDrawer.DisplayDropDown(dropdownRect, mappingTypes, null, TypeGrouping.ByNamespaceFlat);
            }

            if (Event.current.type == EventType.ExecuteCommand)
            {
                if (Event.current.commandName == TypeReferencePropertyDrawer.TypeReferenceUpdated)
                {
                    controllerMappingProfiles.arraySize += 1;
                    var mappingProfileProperty = controllerMappingProfiles.GetArrayElementAtIndex(controllerMappingProfiles.arraySize - 1);
                    ThisProfile.CreateNewProfileInstance(mappingProfileProperty, TypeReferencePropertyDrawer.SelectedType);

                    TypeReferencePropertyDrawer.SelectedType = null;
                    TypeReferencePropertyDrawer.SelectedReference = null;

                    changed = true;
                }
            }

            EditorGUILayout.Space();

            for (int i = 0; i < controllerMappingProfiles.arraySize; i++)
            {
                var controllerProfile = controllerMappingProfiles.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                MixedRealityProfilePropertyDrawer.DrawCloneButtons = false;
                MixedRealityProfilePropertyDrawer.ProfileTypeOverride = typeof(BaseMixedRealityControllerMappingProfile);
                EditorGUILayout.PropertyField(controllerProfile, GUIContent.none);
                changed |= EditorGUI.EndChangeCheck();

                if (GUILayout.Button(RemoveMappingDefinitionContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerMappingProfiles.DeleteArrayElementAtIndex(i);
                    changed = true;
                }

                EditorGUILayout.EndHorizontal();

                if (changed)
                {
                    break;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}