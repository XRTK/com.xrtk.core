// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
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
        private SerializedProperty boundarySystemProfile;

        // Teleport system properties
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;
        private SerializedProperty teleportSystemProfile;

        // Locomotion system properties
        private SerializedProperty enableLocomotionSystem;
        private SerializedProperty locomotionSystemType;
        private SerializedProperty locomotionSystemProfile;

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
        private bool didPromptToConfigure = false;

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
                        didPromptToConfigure = true;
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
            boundarySystemProfile = serializedObject.FindProperty(nameof(boundarySystemProfile));

            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty(nameof(enableTeleportSystem));
            teleportSystemType = serializedObject.FindProperty(nameof(teleportSystemType));
            teleportSystemProfile = serializedObject.FindProperty(nameof(teleportSystemProfile));

            // Locomotion system configuration
            enableLocomotionSystem = serializedObject.FindProperty(nameof(enableLocomotionSystem));
            locomotionSystemType = serializedObject.FindProperty(nameof(locomotionSystemType));
            locomotionSystemProfile = serializedObject.FindProperty(nameof(locomotionSystemProfile));

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
            profileLabel.tooltip = boundarySystemProfile.tooltip;
            EditorGUILayout.PropertyField(boundarySystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Teleport System configuration
            EditorGUILayout.Space();
            enableTeleportSystem.boolValue = EditorGUILayout.ToggleLeft("Teleport System (deprecated)", enableTeleportSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = teleportSystemType.tooltip;
            EditorGUILayout.PropertyField(teleportSystemType, typeLabel);
            profileLabel.tooltip = teleportSystemProfile.tooltip;
            EditorGUILayout.PropertyField(teleportSystemProfile, profileLabel);
            EditorGUI.indentLevel--;

            // Locomotion System configuration
            EditorGUILayout.Space();
            enableLocomotionSystem.boolValue = EditorGUILayout.ToggleLeft("Locomotion System", enableLocomotionSystem.boolValue, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            typeLabel.tooltip = locomotionSystemType.tooltip;
            EditorGUILayout.PropertyField(locomotionSystemType, typeLabel);
            profileLabel.tooltip = locomotionSystemProfile.tooltip;
            EditorGUILayout.PropertyField(locomotionSystemProfile, profileLabel);
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
