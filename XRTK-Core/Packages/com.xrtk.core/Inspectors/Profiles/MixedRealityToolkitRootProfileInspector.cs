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

            if (!MixedRealityToolkit.IsInitialized) { return; }

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();

            // Camera System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraSystem);
            EditorGUILayout.PropertyField(cameraSystemType);
            EditorGUILayout.PropertyField(cameraSystemProfile);

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);
            EditorGUILayout.PropertyField(inputSystemType);
            EditorGUILayout.PropertyField(inputSystemProfile);

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);
            EditorGUILayout.PropertyField(boundarySystemType);
            EditorGUILayout.PropertyField(boundaryVisualizationProfile);

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
            EditorGUILayout.PropertyField(spatialAwarenessProfile);

            // Networking System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Networking System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableNetworkingSystem);
            EditorGUILayout.PropertyField(networkingSystemType);
            EditorGUILayout.PropertyField(networkingSystemProfile);

            // Diagnostics System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Diagnostics System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableDiagnosticsSystem);
            EditorGUILayout.PropertyField(diagnosticsSystemType);
            EditorGUILayout.PropertyField(diagnosticsSystemProfile);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(registeredServiceProvidersProfile);

            EditorGUIUtility.labelWidth = previousLabelWidth;
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