// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Utilities;
using XRTK.Services;

namespace XRTK.Editor.Profiles
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

        // Native Library system properties
        private SerializedProperty enableNativeLibrarySystem;
        private SerializedProperty nativeLibrarySystemType;
        private SerializedProperty nativeLibrarySystemProfile;

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private MixedRealityToolkitRootProfile rootProfile;

        private readonly GUIContent typeLabel = new GUIContent("Instanced Type", "The class type to instantiate at runtime for this system.");
        private readonly GUIContent profileLabel = new GUIContent("Profile");
        private GUIStyle headerStyle;

        private GUIStyle HeaderStyle
        {
            get
            {
                if (headerStyle == null)
                {
                    var editorStyle = EditorGUIUtility.isProSkin ? EditorStyles.whiteLargeLabel : EditorStyles.largeLabel;

                    if (editorStyle != null)
                    {
                        headerStyle = new GUIStyle(editorStyle)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 18,
                            padding = new RectOffset(0, 0, -8, -8)
                        };
                    }
                }

                return headerStyle;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            headerStyle = null;
            rootProfile = target as MixedRealityToolkitRootProfile;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized && prefabStage == null)
            {
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
            EditorGUILayout.LabelField("The Mixed Reality Toolkit", HeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            RenderSystemFields();
        }

        internal void RenderSystemFields()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Camera System configuration
            enableCameraSystem.boolValue = EditorGUILayout.ToggleLeft("Camera System", enableCameraSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = cameraSystemType.tooltip;
            EditorGUILayout.PropertyField(cameraSystemType, typeLabel);
            profileLabel.tooltip = cameraSystemProfile.tooltip;
            EditorGUILayout.PropertyField(cameraSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Input System configuration
            EditorGUILayout.Space();
            enableInputSystem.boolValue = EditorGUILayout.ToggleLeft("Input System", enableInputSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = inputSystemType.tooltip;
            EditorGUILayout.PropertyField(inputSystemType, typeLabel);
            profileLabel.tooltip = inputSystemProfile.tooltip;
            EditorGUILayout.PropertyField(inputSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Boundary System configuration
            EditorGUILayout.Space();
            enableBoundarySystem.boolValue = EditorGUILayout.ToggleLeft("Boundary System", enableBoundarySystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = boundarySystemType.tooltip;
            EditorGUILayout.PropertyField(boundarySystemType, typeLabel);
            profileLabel.tooltip = boundaryVisualizationProfile.tooltip;
            EditorGUILayout.PropertyField(boundaryVisualizationProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Teleport System configuration
            EditorGUILayout.Space();
            enableTeleportSystem.boolValue = EditorGUILayout.ToggleLeft("Teleport System", enableTeleportSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = teleportSystemType.tooltip;
            EditorGUILayout.PropertyField(teleportSystemType, typeLabel);
            EditorGUI.indentLevel--;

            // Spatial Awareness System configuration
            EditorGUILayout.Space();
            enableSpatialAwarenessSystem.boolValue = EditorGUILayout.ToggleLeft("Spatial Awareness System", enableSpatialAwarenessSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = spatialAwarenessSystemType.tooltip;
            EditorGUILayout.PropertyField(spatialAwarenessSystemType, typeLabel);
            profileLabel.tooltip = spatialAwarenessProfile.tooltip;
            EditorGUILayout.PropertyField(spatialAwarenessProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Networking System configuration
            EditorGUILayout.Space();
            enableNetworkingSystem.boolValue = EditorGUILayout.ToggleLeft("Networking System", enableNetworkingSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = networkingSystemType.tooltip;
            EditorGUILayout.PropertyField(networkingSystemType, typeLabel);
            profileLabel.tooltip = networkingSystemProfile.tooltip;
            EditorGUILayout.PropertyField(networkingSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Diagnostics System configuration
            EditorGUILayout.Space();
            enableDiagnosticsSystem.boolValue = EditorGUILayout.ToggleLeft("Diagnostics System", enableDiagnosticsSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = diagnosticsSystemType.tooltip;
            EditorGUILayout.PropertyField(diagnosticsSystemType, typeLabel);
            profileLabel.tooltip = diagnosticsSystemProfile.tooltip;
            EditorGUILayout.PropertyField(diagnosticsSystemProfile, profileLabel);
            EditorGUI.indentLevel--;


            // Native Library System configuration
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Native Library System Settings", EditorStyles.boldLabel);
            enableNativeLibrarySystem.boolValue = EditorGUILayout.ToggleLeft("Native Library System", enableNativeLibrarySystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = nativeLibrarySystemType.tooltip;
            EditorGUILayout.PropertyField(nativeLibrarySystemType, typeLabel);
            profileLabel.tooltip = nativeLibrarySystemProfile.tooltip;
            EditorGUILayout.PropertyField(nativeLibrarySystemProfile, profileLabel);
            EditorGUI.indentLevel--;


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
    }
}