// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
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
        private SerializedProperty cameraSystemProfile;

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

        private MixedRealityToolkitRootProfile rootProfile;

        private readonly GUIContent enabledLabel = new GUIContent("Enabled");
        private readonly GUIContent typeLabel = new GUIContent("Type");
        private readonly GUIContent profileLabel = new GUIContent("Profile");

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
                            var playspace = MixedRealityToolkit.CameraSystem.MainCameraRig.PlayspaceTransform;
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
            cameraSystemProfile = serializedObject.FindProperty(nameof(cameraSystemProfile));

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

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty(nameof(registeredServiceProvidersProfile));
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Camera System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableCameraSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableCameraSystem.boolValue);
            EditorGUILayout.PropertyField(cameraSystemType, typeLabel);
            EditorGUILayout.PropertyField(cameraSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableInputSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableInputSystem.boolValue);
            EditorGUILayout.PropertyField(inputSystemType, typeLabel);
            EditorGUILayout.PropertyField(inputSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableBoundarySystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableBoundarySystem.boolValue);
            EditorGUILayout.PropertyField(boundarySystemType, typeLabel);
            EditorGUILayout.PropertyField(boundaryVisualizationProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Teleport System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Teleport System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableTeleportSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableTeleportSystem.boolValue);
            EditorGUILayout.PropertyField(teleportSystemType, typeLabel);
            EditorGUI.indentLevel--;

            // Spatial Awareness System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Spatial Awareness System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableSpatialAwarenessSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableSpatialAwarenessSystem.boolValue);
            EditorGUILayout.PropertyField(spatialAwarenessSystemType, typeLabel);
            EditorGUILayout.PropertyField(spatialAwarenessProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Networking System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Networking System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableNetworkingSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableNetworkingSystem.boolValue);
            EditorGUILayout.PropertyField(networkingSystemType, typeLabel);
            EditorGUILayout.PropertyField(networkingSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Diagnostics System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Diagnostics System Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            enableDiagnosticsSystem.boolValue = EditorGUILayout.ToggleLeft(enabledLabel, enableDiagnosticsSystem.boolValue);
            EditorGUILayout.PropertyField(diagnosticsSystemType, typeLabel);
            EditorGUILayout.PropertyField(diagnosticsSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
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
    }
}