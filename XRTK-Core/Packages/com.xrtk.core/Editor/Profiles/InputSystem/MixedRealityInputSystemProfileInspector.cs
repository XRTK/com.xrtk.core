// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.InputSystem;
using XRTK.Editor.Extensions;
using XRTK.Services;

namespace XRTK.Editor.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        private const string kDefaultInputActionsAssetPath = "Packages/com.unity.inputsystem/InputSystem/Plugins/PlayerInput/DefaultInputActions.inputactions";

        private static readonly GUIContent FocusProviderContent = new GUIContent("Focus Provider");
        private static readonly GUIContent GazeProviderContent = new GUIContent("Gaze Provider");
        private static readonly GUIContent GazeCursorPrefabContent = new GUIContent("Gaze Cursor Prefab");
        private static readonly GUIContent GlobalPointerSettingsContent = new GUIContent("Global Pointer Settings");
        private static readonly GUIContent GlobalHandSettingsContent = new GUIContent("Global Hand Settings");

        private SerializedProperty focusProviderType;
        private SerializedProperty gazeProviderType;
        private SerializedProperty gazeCursorPrefab;

        private SerializedProperty pointingExtent;
        private SerializedProperty pointerRaycastLayerMasks;
        private SerializedProperty drawDebugPointingRays;
        private SerializedProperty debugPointingRayColors;

        private SerializedProperty gripThreshold;
        private SerializedProperty renderingMode;
        private SerializedProperty handPhysicsEnabled;
        private SerializedProperty useTriggers;
        private SerializedProperty boundsMode;
        private SerializedProperty trackedPoses;

        private SerializedProperty inputActions;
        private SerializedProperty speechCommandsProfile;

        private bool showGlobalPointerOptions;
        private bool showGlobalHandOptions;
        private ReorderableList poseProfilesList;
        private int currentlySelectedPoseElement;

        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty(nameof(focusProviderType));
            gazeProviderType = serializedObject.FindProperty(nameof(gazeProviderType));
            gazeCursorPrefab = serializedObject.FindProperty(nameof(gazeCursorPrefab));

            pointingExtent = serializedObject.FindProperty(nameof(pointingExtent));
            pointerRaycastLayerMasks = serializedObject.FindProperty(nameof(pointerRaycastLayerMasks));
            drawDebugPointingRays = serializedObject.FindProperty(nameof(drawDebugPointingRays));
            debugPointingRayColors = serializedObject.FindProperty(nameof(debugPointingRayColors));

            gripThreshold = serializedObject.FindProperty(nameof(gripThreshold));
            renderingMode = serializedObject.FindProperty(nameof(renderingMode));
            handPhysicsEnabled = serializedObject.FindProperty(nameof(handPhysicsEnabled));
            useTriggers = serializedObject.FindProperty(nameof(useTriggers));
            boundsMode = serializedObject.FindProperty(nameof(boundsMode));
            trackedPoses = serializedObject.FindProperty(nameof(trackedPoses));

            inputActions = serializedObject.FindProperty(nameof(inputActions));
            speechCommandsProfile = serializedObject.FindProperty(nameof(speechCommandsProfile));

            poseProfilesList = new ReorderableList(serializedObject, trackedPoses, true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.5f
            };
            poseProfilesList.drawHeaderCallback += PoseProfilesList_DrawHeaderCallback;
            poseProfilesList.drawElementCallback += PoseProfilesList_DrawConfigurationOptionElement;
            poseProfilesList.onAddCallback += PoseProfilesList_OnConfigurationOptionAdded;
            poseProfilesList.onRemoveCallback += PoseProfilesList_OnConfigurationOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The Input System Profile helps developers configure input no matter what platform you're building for.");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(focusProviderType, FocusProviderContent);
            EditorGUILayout.PropertyField(gazeProviderType, GazeProviderContent);
            EditorGUILayout.PropertyField(gazeCursorPrefab, GazeCursorPrefabContent);

            EditorGUILayout.Space();

            showGlobalPointerOptions = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showGlobalPointerOptions, GlobalPointerSettingsContent, true);

            if (showGlobalPointerOptions)
            {
                EditorGUILayout.HelpBox("Global pointer options applied to all controllers that support pointers. You may override these globals per controller mapping profile.", MessageType.Info);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pointingExtent);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(pointerRaycastLayerMasks, true);
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel--;
                var newValue = EditorGUILayout.ToggleLeft(new GUIContent(drawDebugPointingRays.displayName, drawDebugPointingRays.tooltip), drawDebugPointingRays.boolValue);
                EditorGUI.indentLevel++;

                if (EditorGUI.EndChangeCheck())
                {
                    drawDebugPointingRays.boolValue = newValue;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(debugPointingRayColors, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            showGlobalHandOptions = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showGlobalHandOptions, GlobalHandSettingsContent, true);

            if (showGlobalHandOptions)
            {
                EditorGUILayout.HelpBox("Global hand tracking options applied to all platforms that support hand tracking. You may override these globals per platform in the platform's hand controller data provider profile.", MessageType.Info);
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Hand Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(gripThreshold);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hand Rendering Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(renderingMode);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hand Physics Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(handPhysicsEnabled);
                EditorGUILayout.PropertyField(useTriggers);
                EditorGUILayout.PropertyField(boundsMode);
                poseProfilesList.DoLayoutList();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(inputActions);
            DoHelpCreateAssetUI();
            EditorGUILayout.PropertyField(speechCommandsProfile);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetProfile(MixedRealityToolkit.Instance.ActiveProfile);
            }

            base.OnInspectorGUI();
        }

        private void PoseProfilesList_DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Tracked Hand Poses");
        }

        private void PoseProfilesList_DrawConfigurationOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedPoseElement = index;
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 3;
            var poseDataProperty = trackedPoses.GetArrayElementAtIndex(index);
            var selectedPoseData = EditorGUI.ObjectField(rect, poseDataProperty.objectReferenceValue, typeof(HandControllerPoseProfile), false) as HandControllerPoseProfile;

            if (selectedPoseData != null)
            {
                selectedPoseData.ParentProfile = ThisProfile;
            }

            poseDataProperty.objectReferenceValue = selectedPoseData;
        }

        private void PoseProfilesList_OnConfigurationOptionAdded(ReorderableList list)
        {
            trackedPoses.arraySize += 1;
            var index = trackedPoses.arraySize - 1;

            var mappingProfileProperty = trackedPoses.GetArrayElementAtIndex(index);
            mappingProfileProperty.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        }

        private void PoseProfilesList_OnConfigurationOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedPoseElement >= 0)
            {
                trackedPoses.DeleteArrayElementAtIndex(currentlySelectedPoseElement);
            }

            serializedObject.ApplyModifiedProperties();
        }

        // Since Unity's Input System doesn't have a public way to generate a clone of
        // the default input action maps, we just lifted this helper method from
        // UnityEngine.InputSystem.Editor.PlayerInputEditor.cs
        private void DoHelpCreateAssetUI()
        {
            if (inputActions.objectReferenceValue != null)
            {
                // All good. We already have an asset.
                return;
            }

            EditorGUILayout.HelpBox("There are no input actions associated with the input system yet. " +
                                    "Click the button below to create a new set of input actions or drag an " +
                                    "existing input actions asset into the field above.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Create Actions..."), EditorStyles.miniButton, GUILayout.MaxWidth(120)))
            {
                // Request save file location.
                var defaultFileName = Application.productName;
                var fileName = EditorUtility.SaveFilePanel("Create Input Actions Asset", "Assets", defaultFileName, InputActionAsset.Extension);

                // Create and import asset and open editor.
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!fileName.StartsWith(Application.dataPath))
                    {
                        Debug.LogError($"Path must be located in Assets/ folder (got: '{fileName}')");
                        EditorGUILayout.EndHorizontal();
                        return;
                    }

                    if (!fileName.EndsWith($".{InputActionAsset.Extension}"))
                    {
                        fileName += $".{InputActionAsset.Extension}";
                    }

                    // Load default actions and update all GUIDs.
                    var defaultActionsText = File.ReadAllText(kDefaultInputActionsAssetPath);
                    var newActions = InputActionAsset.FromJson(defaultActionsText);

                    foreach (var map in newActions.actionMaps)
                    {
                        Debug.Assert(map.id != Guid.Empty);

                        foreach (var action in map.actions)
                        {
                            Debug.Assert(action.id != Guid.Empty);
                        }
                    }

                    newActions.name = Path.GetFileNameWithoutExtension(fileName);
                    var newActionsText = newActions.ToJson();

                    // Write it out and tell the asset DB to pick it up.
                    File.WriteAllText(fileName, newActionsText);

                    // Import the new asset
                    var relativePath = $"Assets/{fileName.Substring(Application.dataPath.Length + 1)}";
                    AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceSynchronousImport);

                    // Load imported object.
                    var inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(relativePath);

                    // Set it on the PlayerInput component.
                    inputActions.objectReferenceValue = inputActionAsset;
                    serializedObject.ApplyModifiedProperties();

                    // Open the asset.
                    AssetDatabase.OpenAsset(inputActionAsset);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
    }
}
