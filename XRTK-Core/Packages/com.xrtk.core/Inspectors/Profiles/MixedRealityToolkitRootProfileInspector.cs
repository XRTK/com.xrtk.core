// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Extensions;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityToolkitRootProfile))]
    public class MixedRealityToolkitRootProfileInspector : BaseMixedRealityProfileInspector
    {
        // Camera system properties
        private SerializedProperty enableCameraSystem;
        private SerializedProperty cameraSystemType;
        private SerializedProperty cameraProfile;

        // Input system properties
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputSystemProfile;

        // Boundary system properties
        private SerializedProperty enableBoundarySystem;
        private SerializedProperty boundarySystemType;
        private SerializedProperty boundaryVisualizationProfile;

        // Teleport system properties
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;

        // Spatial Awareness system properties
        private SerializedProperty enableSpatialAwarenessSystem;
        private SerializedProperty spatialAwarenessSystemType;
        private SerializedProperty spatialAwarenessProfile;

        // Networking system properties
        private SerializedProperty enableNetworkingSystem;
        private SerializedProperty networkingSystemType;
        private SerializedProperty networkingSystemProfile;

        // Diagnostic system properties
        private SerializedProperty enableDiagnosticsSystem;
        private SerializedProperty diagnosticsSystemType;
        private SerializedProperty diagnosticsSystemProfile;

        // Native Library system properties
        private SerializedProperty enableNativeLibrarySystem;
        private SerializedProperty nativeLibrarySystemType;
        private SerializedProperty nativeLibrarySystemProfile;

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private MixedRealityToolkitRootProfile rootProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            rootProfile = target as MixedRealityToolkitRootProfile;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized && prefabStage == null)
            {
                // TODO Check base scene for service locator existence?

                // Search for all instances, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Toolkit in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later"))
                    {
                        if (MixedRealityToolkit.CameraSystem != null)
                        {
                            var playspace = MixedRealityToolkit.CameraSystem.CameraRig.PlayspaceTransform;
                            Debug.Assert(playspace != null);
                        }

                        MixedRealityToolkit.Instance.ActiveProfile = rootProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Toolkit in your scene.");
                    }
                }
            }

            // Camera system configuration
            enableCameraSystem = serializedObject.FindProperty(nameof(enableCameraSystem));
            cameraSystemType = serializedObject.FindProperty(nameof(cameraSystemType));
            cameraProfile = serializedObject.FindProperty(nameof(cameraProfile));

            // Input system configuration
            enableInputSystem = serializedObject.FindProperty(nameof(enableInputSystem));
            inputSystemType = serializedObject.FindProperty(nameof(inputSystemType));
            inputSystemProfile = serializedObject.FindProperty(nameof(inputSystemProfile));

            // Boundary system configuration
            enableBoundarySystem = serializedObject.FindProperty(nameof(enableBoundarySystem));
            boundarySystemType = serializedObject.FindProperty(nameof(boundarySystemType));
            boundaryVisualizationProfile = serializedObject.FindProperty(nameof(boundaryVisualizationProfile));

            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty(nameof(enableTeleportSystem));
            teleportSystemType = serializedObject.FindProperty(nameof(teleportSystemType));

            // Spatial Awareness system configuration
            enableSpatialAwarenessSystem = serializedObject.FindProperty(nameof(enableSpatialAwarenessSystem));
            spatialAwarenessSystemType = serializedObject.FindProperty(nameof(spatialAwarenessSystemType));
            spatialAwarenessProfile = serializedObject.FindProperty(nameof(spatialAwarenessProfile));

            // Networking system configuration
            enableNetworkingSystem = serializedObject.FindProperty(nameof(enableNetworkingSystem));
            networkingSystemType = serializedObject.FindProperty(nameof(networkingSystemType));
            networkingSystemProfile = serializedObject.FindProperty(nameof(networkingSystemProfile));

            // Diagnostics system configuration
            enableDiagnosticsSystem = serializedObject.FindProperty(nameof(enableDiagnosticsSystem));
            diagnosticsSystemType = serializedObject.FindProperty(nameof(diagnosticsSystemType));
            diagnosticsSystemProfile = serializedObject.FindProperty(nameof(diagnosticsSystemProfile));

            // Native library system configuration
            enableNativeLibrarySystem = serializedObject.FindProperty(nameof(enableNativeLibrarySystem));
            nativeLibrarySystemType = serializedObject.FindProperty(nameof(nativeLibrarySystemType));
            nativeLibrarySystemProfile = serializedObject.FindProperty(nameof(nativeLibrarySystemProfile));

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty(nameof(registeredServiceProvidersProfile));
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            serializedObject.Update();

            if (!MixedRealityToolkit.IsInitialized) { return; }

            if (!rootProfile.IsEditable)
            {
                EditorGUILayout.HelpBox("The Mixed Reality Toolkit's core SDK profiles can be used to get up and running quickly.\n\n" +
                                        "You can use the default profiles provided, copy and customize the default profiles, or create your own.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Clone & Customize"))
                {
                    CreateCloneProfile();
                }

                if (GUILayout.Button("Create new profiles"))
                {
                    var profile = CreateInstance(nameof(MixedRealityToolkitRootProfile));
                    var newProfile = profile.CreateAsset() as MixedRealityToolkitRootProfile;
                    MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                }

                EditorGUILayout.EndHorizontal();
            }

            // We don't call the CheckLock method so won't get a duplicate message.
            if (MixedRealityPreferences.LockProfiles && !ThisProfile.IsEditable)
            {
                GUI.enabled = false;
            }

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Camera System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraSystem);
            EditorGUILayout.PropertyField(cameraSystemType);
            changed |= RenderProfile(ThisProfile, cameraProfile);

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);
            EditorGUILayout.PropertyField(inputSystemType);
            changed |= RenderProfile(ThisProfile, inputSystemProfile);

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);
            EditorGUILayout.PropertyField(boundarySystemType);
            changed |= RenderProfile(ThisProfile, boundaryVisualizationProfile);

            // Teleport System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Teleport System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableTeleportSystem);
            EditorGUILayout.PropertyField(teleportSystemType);

            // Spatial Awareness System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Spatial Awareness System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableSpatialAwarenessSystem);
            EditorGUILayout.PropertyField(spatialAwarenessSystemType);
            changed |= RenderProfile(ThisProfile, spatialAwarenessProfile);

            // Networking System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Networking System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableNetworkingSystem);
            EditorGUILayout.PropertyField(networkingSystemType);
            changed |= RenderProfile(ThisProfile, networkingSystemProfile);

            // Diagnostics System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Diagnostics System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableDiagnosticsSystem);
            EditorGUILayout.PropertyField(diagnosticsSystemType);
            changed |= RenderProfile(ThisProfile, diagnosticsSystemProfile);

            // Native Library System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Native Library System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableNativeLibrarySystem);
            EditorGUILayout.PropertyField(nativeLibrarySystemType);
            changed |= RenderProfile(ThisProfile, nativeLibrarySystemProfile);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            changed |= RenderProfile(ThisProfile, registeredServiceProvidersProfile);

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed &&
                MixedRealityToolkit.IsInitialized &&
                MixedRealityToolkit.Instance.ActiveProfile == rootProfile)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(rootProfile);
            }
        }
    }
}