// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Inspectors.Extensions;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(BaseMixedRealityControllerDataProviderProfile))]
    public class BaseMixedRealityControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty hasSetupDefaults;
        private SerializedProperty controllerMappingProfiles;

        protected override void OnEnable()
        {
            base.OnEnable();

            serializedObject.Update();

            hasSetupDefaults = serializedObject.FindProperty(nameof(hasSetupDefaults));
            controllerMappingProfiles = serializedObject.FindProperty(nameof(controllerMappingProfiles));

            var dataProviderProfile = (BaseMixedRealityControllerDataProviderProfile)serializedObject.targetObject;

            if (!hasSetupDefaults.boolValue)
            {
                var defaultControllerOptions = dataProviderProfile.GetDefaultControllerOptions();

                Debug.Assert(defaultControllerOptions != null, $"Missing default controller definitions for {dataProviderProfile.name}");

                var defaultProfiles = new MixedRealityControllerMappingProfile[defaultControllerOptions.Length];

                controllerMappingProfiles.ClearArray();

                for (int i = 0; i < defaultProfiles.Length; i++)
                {
                    var instance = CreateInstance(nameof(MixedRealityControllerMappingProfile)).CreateAsset() as MixedRealityControllerMappingProfile;
                    Debug.Assert(instance != null);
                    instance.ControllerType = defaultControllerOptions[i].ControllerType;
                    instance.Handedness = defaultControllerOptions[i].Handedness;
                    instance.UseCustomInteractions = defaultControllerOptions[i].UseCustomInteractions;
                    instance.SetDefaultInteractionMapping();
                    defaultProfiles[i] = instance;
                    controllerMappingProfiles.InsertArrayElementAtIndex(i);
                    var mappingProfile = controllerMappingProfiles.GetArrayElementAtIndex(i);
                    mappingProfile.objectReferenceValue = instance;
                }

                hasSetupDefaults.boolValue = true;

                serializedObject.ApplyModifiedProperties();

                for (int i = 0; i < defaultControllerOptions.Length; i++)
                {
                    Debug.Log(defaultControllerOptions[i].ControllerType.Type.Name);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(controllerMappingProfiles, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}