// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers.Simulation
{
    [CustomEditor(typeof(SimulatedControllerDataProviderProfile))]
    public class SimulatedControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private static readonly GUIContent AddControllerDataProviderContent = new GUIContent("+ Add a New Controller Data Provider");
        private static readonly GUIContent RemoveControllerDataProviderContent = new GUIContent("-", "Remove Controller Data Provider");
        private static readonly GUIContent ProfileContent = new GUIContent("Profile");

        private SerializedProperty simulatedControllerType;
        private SerializedProperty simulatedUpdateFrequency;
        private SerializedProperty controllerHideTimeout;

        private SerializedProperty defaultDistance;
        private SerializedProperty depthMultiplier;
        private SerializedProperty jitterAmount;

        private SerializedProperty toggleLeftPersistentKey;
        private SerializedProperty leftControllerTrackedKey;
        private SerializedProperty toggleRightPersistentKey;
        private SerializedProperty rightControllerTrackedKey;

        private SerializedProperty rotationSpeed;
        private SerializedProperty registeredControllerDataProviders;
        private bool[] foldouts = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            simulatedControllerType = serializedObject.FindProperty(nameof(simulatedControllerType));
            simulatedUpdateFrequency = serializedObject.FindProperty(nameof(simulatedUpdateFrequency));
            controllerHideTimeout = serializedObject.FindProperty(nameof(controllerHideTimeout));

            defaultDistance = serializedObject.FindProperty(nameof(defaultDistance));
            depthMultiplier = serializedObject.FindProperty(nameof(depthMultiplier));
            jitterAmount = serializedObject.FindProperty(nameof(jitterAmount));

            toggleLeftPersistentKey = serializedObject.FindProperty(nameof(toggleLeftPersistentKey));
            toggleRightPersistentKey = serializedObject.FindProperty(nameof(toggleRightPersistentKey));
            leftControllerTrackedKey = serializedObject.FindProperty(nameof(leftControllerTrackedKey));
            rightControllerTrackedKey = serializedObject.FindProperty(nameof(rightControllerTrackedKey));

            rotationSpeed = serializedObject.FindProperty(nameof(rotationSpeed));
            registeredControllerDataProviders = serializedObject.FindProperty(nameof(registeredControllerDataProviders));
            foldouts = new bool[registeredControllerDataProviders.arraySize];
        }

        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            thisProfile.CheckProfileLock();

            serializedObject.Update();

            EditorGUILayout.BeginVertical("Label");

            EditorGUILayout.PropertyField(simulatedControllerType);
            EditorGUILayout.PropertyField(simulatedUpdateFrequency);
            EditorGUILayout.PropertyField(controllerHideTimeout);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(defaultDistance);
            EditorGUILayout.PropertyField(depthMultiplier);
            EditorGUILayout.PropertyField(jitterAmount);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(toggleLeftPersistentKey);
            EditorGUILayout.PropertyField(leftControllerTrackedKey);
            EditorGUILayout.PropertyField(toggleRightPersistentKey);
            EditorGUILayout.PropertyField(rightControllerTrackedKey);
            EditorGUILayout.PropertyField(rotationSpeed);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();

            bool changed = false;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Simulated Controller Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Register data providers for specific types of simulated controllers here.", MessageType.Info);

            if (GUILayout.Button(AddControllerDataProviderContent, EditorStyles.miniButton))
            {
                registeredControllerDataProviders.arraySize += 1;
                var newConfiguration = registeredControllerDataProviders.GetArrayElementAtIndex(registeredControllerDataProviders.arraySize - 1);
                var dataProviderType = newConfiguration.FindPropertyRelative("dataProviderType");
                var dataProviderName = newConfiguration.FindPropertyRelative("dataProviderName");
                var priority = newConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = newConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = newConfiguration.FindPropertyRelative("profile");

                serializedObject.ApplyModifiedProperties();
                dataProviderType.FindPropertyRelative("reference").stringValue = string.Empty;
                dataProviderName.stringValue = "New Controller Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                profile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[registeredControllerDataProviders.arraySize];
                changed = true;
            }

            EditorGUILayout.Space();

            for (int i = 0; i < registeredControllerDataProviders.arraySize; i++)
            {
                var controllerConfiguration = registeredControllerDataProviders.GetArrayElementAtIndex(i);
                var dataProviderName = controllerConfiguration.FindPropertyRelative("dataProviderName");
                var dataProviderType = controllerConfiguration.FindPropertyRelative("dataProviderType");
                var priority = controllerConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = controllerConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = controllerConfiguration.FindPropertyRelative("profile");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], dataProviderName.stringValue, true);

                if (GUILayout.Button(RemoveControllerDataProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    registeredControllerDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[registeredControllerDataProviders.arraySize];
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(dataProviderType);
                    EditorGUILayout.PropertyField(dataProviderName);
                    EditorGUILayout.PropertyField(priority);
                    EditorGUILayout.PropertyField(runtimePlatform);
                    RenderProfile(thisProfile, profile, ProfileContent, false);

                    if (EditorGUI.EndChangeCheck())
                    {
                        changed = true;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}