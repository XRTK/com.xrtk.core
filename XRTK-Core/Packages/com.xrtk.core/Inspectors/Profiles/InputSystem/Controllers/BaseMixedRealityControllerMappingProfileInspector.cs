// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Utilities;
using XRTK.Definitions.Controllers;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseMixedRealityControllerMappingProfile))]
    public class BaseMixedRealityControllerMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private struct ControllerItem
        {
            public readonly SupportedControllerType ControllerType;
            public readonly Handedness Handedness;
            public readonly MixedRealityInteractionMapping[] Interactions;

            public ControllerItem(SupportedControllerType controllerType, Handedness handedness, MixedRealityInteractionMapping[] interactions)
            {
                ControllerType = controllerType;
                Handedness = handedness;
                Interactions = interactions;
            }
        }

        private readonly List<ControllerItem> controllerItems = new List<ControllerItem>();

        private SerializedProperty controllerMappings;

        private BaseMixedRealityControllerMappingProfile controllerMappingProfile;

        private GUIStyle controllerButtonStyle;

        protected override void OnEnable()
        {
            base.OnEnable();

            controllerMappings = serializedObject.FindProperty("controllerMappings");
            controllerMappingProfile = target as BaseMixedRealityControllerMappingProfile;
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to controller mapping list"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            var deviceName = controllerMappingProfile.ControllerType == SupportedControllerType.None ? "Custom Device" : controllerMappingProfile.ControllerType.ToString();
            EditorGUILayout.LabelField($"{deviceName} Mappings", EditorStyles.boldLabel);

            controllerMappingProfile.CheckProfileLock(false);

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

            serializedObject.Update();
            controllerItems.Clear();

            GUILayout.BeginVertical();

            if (controllerMappings.arraySize == 0)
            {
                EditorGUILayout.HelpBox("You must override the controller mappings in your custom implementation to see a list of mappings for your device.", MessageType.Error);
            }

            for (int i = 0; i < controllerMappings?.arraySize; i++)
            {
                var supportedControllerType = controllerMappingProfile.ControllerType;
                var controllerMapping = controllerMappings.GetArrayElementAtIndex(i);
                var handednessValue = controllerMapping.FindPropertyRelative("handedness");
                var handedness = (Handedness)handednessValue.intValue;
                var description = controllerMapping.FindPropertyRelative("description");
                var interactions = controllerMapping.FindPropertyRelative("interactions");

                bool skip = false;

                for (int j = 0; j < controllerItems.Count; j++)
                {
                    if (controllerItems[j].ControllerType == supportedControllerType &&
                        controllerItems[j].Handedness == handedness)
                    {
                        controllerMappingProfile.ControllerMappings[i].SynchronizeInputActions(controllerItems[j].Interactions);
                        serializedObject.ApplyModifiedProperties();
                        skip = true;
                    }
                }

                if (skip) { continue; }

                controllerItems.Add(new ControllerItem(supportedControllerType, handedness, controllerMappingProfile.ControllerMappings[i].Interactions));

                string handednessContent = string.Empty;

                switch (handedness)
                {
                    case Handedness.Left:
                    case Handedness.Right:
                    case Handedness.Other:
                        handednessContent = $" {handedness.ToString()} hand";
                        break;
                    case Handedness.Both:
                        handednessContent = $" {handedness.ToString()} hands";
                        break;
                }

                if (handedness != Handedness.Right)
                {
                    GUILayout.BeginHorizontal();
                }

                var buttonContent = new GUIContent($"Edit {description.stringValue}{handednessContent}", ControllerMappingLibrary.GetControllerTextureScaled(controllerMappingProfile, supportedControllerType, handedness));

                if (GUILayout.Button(buttonContent, controllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorApplication.delayCall += () => ControllerPopupWindow.Show(controllerMappingProfile, controllerMappingProfile.ControllerType, interactions, handedness, MixedRealityPreferences.LockProfiles && !thisProfile.IsCustomProfile);
                }

                if (handedness != Handedness.Left)
                {
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}