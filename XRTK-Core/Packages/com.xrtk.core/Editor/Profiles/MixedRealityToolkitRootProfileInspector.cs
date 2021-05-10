// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Platforms;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityToolkitRootProfile))]
    public class MixedRealityToolkitRootProfileInspector : MixedRealityServiceProfileInspector
    {
        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private MixedRealityToolkitRootProfile rootProfile;
        private bool didPromptToConfigure = false;

        private readonly GUIContent profileLabel = new GUIContent("Profile");

        private int platformIndex;
        private readonly List<IMixedRealityPlatform> platforms = new List<IMixedRealityPlatform>();

        private List<IMixedRealityPlatform> Platforms
        {
            get
            {
                if (platforms.Count == 0)
                {
                    foreach (var availablePlatform in MixedRealityToolkit.AvailablePlatforms)
                    {
                        if (availablePlatform is AllPlatforms ||
                            availablePlatform is EditorPlatform ||
                            availablePlatform is CurrentBuildTargetPlatform)
                        {
                            continue;
                        }

                        platforms.Add(availablePlatform);
                    }

                    for (var i = 0; i < platforms.Count; i++)
                    {
                        if (MixedRealityPreferences.CurrentPlatformTarget == platforms[i])
                        {
                            platformIndex = i;
                            break;
                        }
                    }
                }

                return platforms;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rootProfile = target as MixedRealityToolkitRootProfile;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            // Create the MixedRealityToolkit object if none exists.
            if (!MixedRealityToolkit.IsInitialized && prefabStage == null && !didPromptToConfigure)
            {
                // Search for all instances, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (!ValidateImplementationsExists())
                    {
                        if (EditorUtility.DisplayDialog(
                            "Attention!",
                            $"We were unable to find any services or data providers to configure. Would you like to install the {nameof(MixedRealityToolkit)} SDK?",
                            "Yes",
                            "Later",
                            DialogOptOutDecisionType.ForThisSession,
                            "XRTK_Prompt_Install_SDK"))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                Client.Add("com.xrtk.sdk");
                            };
                        }

                        Selection.activeObject = null;
                        return;
                    }

                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Toolkit in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later",
                        DialogOptOutDecisionType.ForThisSession,
                        "XRTK_Prompt_Configure_Scene"))
                    {
                        if (MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem))
                        {
                            var playspace = cameraSystem.MainCameraRig.PlayspaceTransform;
                            Debug.Assert(playspace != null);
                        }

                        MixedRealityToolkit.Instance.ActiveProfile = rootProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Toolkit in your scene.");
                        didPromptToConfigure = true;
                    }
                }
            }

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty(nameof(registeredServiceProvidersProfile));

            platforms.Clear();
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("The Mixed Reality Toolkit", MixedRealityInspectorUtility.BoldCenteredHeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            RenderSystemFields();
        }

        internal void RenderSystemFields()
        {

            for (var i = 0; i < Platforms.Count; i++)
            {
                if (MixedRealityPreferences.CurrentPlatformTarget == Platforms[i])
                {
                    platformIndex = i;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck();
            var prevPlatformIndex = platformIndex;
            platformIndex = EditorGUILayout.Popup("Platform Target", platformIndex, Platforms.Select(p => p.Name).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Platforms.Count; i++)
                {
                    if (i == platformIndex)
                    {
                        var platform = Platforms[i];

                        var buildTarget = platform.ValidBuildTargets[0]; // For now just get the highest priority one.

                        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(buildTarget), buildTarget))
                        {
                            platformIndex = prevPlatformIndex;
                            Debug.LogWarning($"Failed to switch {platform.Name} active build target to {buildTarget}");
                        }
                        else
                        {
                            MixedRealityPreferences.CurrentPlatformTarget = platform;
                        }
                    }
                }
            }

            RenderConfigurationOptions();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            profileLabel.tooltip = registeredServiceProvidersProfile.tooltip;
            EditorGUILayout.PropertyField(registeredServiceProvidersProfile, profileLabel);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized &&
                MixedRealityToolkit.Instance.ActiveProfile == rootProfile)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(rootProfile);
            }
        }

        private static bool ValidateImplementationsExists()
        {
            return TypeExtensions.HasValidImplementations<IMixedRealitySystem>() &&
                   TypeExtensions.HasValidImplementations<IMixedRealityService>() &&
                   TypeExtensions.HasValidImplementations<IMixedRealityDataProvider>();
        }
    }
}
