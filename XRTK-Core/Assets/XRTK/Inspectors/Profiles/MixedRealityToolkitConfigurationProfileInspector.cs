// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Extensions.EditorClassExtensions;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityProfileInspector
    {
        // Camera properties
        private SerializedProperty enableCameraProfile;
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

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private MixedRealityToolkitConfigurationProfile configurationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurationProfile = target as MixedRealityToolkitConfigurationProfile;

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Toolkit in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later"))
                    {
                        var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
                        Debug.Assert(playspace != null);
                        MixedRealityToolkit.Instance.ActiveProfile = configurationProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Toolkit in your scene.");
                        return;
                    }
                }
            }

            if (!MixedRealityToolkit.ConfirmInitialized() ||
                !MixedRealityToolkit.HasActiveProfile)
            {
                return;
            }

            // Camera configuration
            enableCameraProfile = serializedObject.FindProperty("enableCameraProfile");
            cameraProfile = serializedObject.FindProperty("cameraProfile");
            // Input system configuration
            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputSystemProfile = serializedObject.FindProperty("inputSystemProfile");
            // Boundary system configuration
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
            boundarySystemType = serializedObject.FindProperty("boundarySystemType");
            boundaryVisualizationProfile = serializedObject.FindProperty("boundaryVisualizationProfile");
            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty("enableTeleportSystem");
            teleportSystemType = serializedObject.FindProperty("teleportSystemType");
            // Spatial Awareness system configuration
            enableSpatialAwarenessSystem = serializedObject.FindProperty("enableSpatialAwarenessSystem");
            spatialAwarenessSystemType = serializedObject.FindProperty("spatialAwarenessSystemType");
            spatialAwarenessProfile = serializedObject.FindProperty("spatialAwarenessProfile");
            // Networking system configuration
            enableNetworkingSystem = serializedObject.FindProperty("enableNetworkingSystem");
            networkingSystemType = serializedObject.FindProperty("networkingSystemType");
            networkingSystemProfile = serializedObject.FindProperty("networkingSystemProfile");
            // Diagnostics system configuration
            enableDiagnosticsSystem = serializedObject.FindProperty("enableDiagnosticsSystem");
            diagnosticsSystemType = serializedObject.FindProperty("diagnosticsSystemType");
            diagnosticsSystemProfile = serializedObject.FindProperty("diagnosticsSystemProfile");

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty("registeredServiceProvidersProfile");
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            serializedObject.Update();

            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("Unable to find Mixed Reality Toolkit!", MessageType.Error);
                return;
            }

            if (!configurationProfile.IsCustomProfile)
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
                    ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                    var newProfile = profile.CreateAsset("Assets/XRTK.Generated/CustomProfiles") as MixedRealityToolkitConfigurationProfile;
                    MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                    Selection.activeObject = newProfile;
                }

                EditorGUILayout.EndHorizontal();
            }

            // We don't call the CheckLock method so won't get a duplicate message.
            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Camera Profile configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProfile);

            changed |= RenderProfile(cameraProfile);

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);
            EditorGUILayout.PropertyField(inputSystemType);
            changed |= RenderProfile(inputSystemProfile);

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);
            EditorGUILayout.PropertyField(boundarySystemType);
            changed |= RenderProfile(boundaryVisualizationProfile);

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
            changed |= RenderProfile(spatialAwarenessProfile);

            // Networking System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Networking System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableNetworkingSystem);
            EditorGUILayout.PropertyField(networkingSystemType);
            changed |= RenderProfile(networkingSystemProfile);

            // Diagnostics System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Diagnostics System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableDiagnosticsSystem);
            EditorGUILayout.PropertyField(diagnosticsSystemType);
            changed |= RenderProfile(diagnosticsSystemProfile);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            changed |= RenderProfile(registeredServiceProvidersProfile);

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(configurationProfile);
            }
        }
    }
}