// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Utilities
{
    public static class ValidateConfiguration
    {
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreControllerMappingsPrompts";

        /// <summary>
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="providerTypesToValidate">Array of Data Provider types to validate</param>
        /// <param name="providerDefaultConfiguration">Array of Data Provider default configurations to add if missing</param>
        /// <param name="prompt">Unit Test helper, to control whether the UI prompt is offered or not</param>
        /// <returns></returns>
        public static bool ValidateService<T>(this BaseMixedRealityServiceProfile<T> profile, Type[] providerTypesToValidate, MixedRealityServiceConfiguration<T>[] providerDefaultConfiguration, bool prompt = true) where T : IMixedRealityService
        {
#if UNITY_EDITOR
            if (Application.isPlaying || EditorPrefs.GetBool(IgnoreKey, false))
            {
                return false;
            }
#endif //UNITY_EDITOR

            if (MixedRealityToolkit.HasActiveProfile)
            {
                var errorsFound = false;

                if (profile == null)
                {
                    return false;
                }

                var registeredConfigurations = profile.RegisteredServiceConfigurations;

                if (providerTypesToValidate != null &&
                    providerTypesToValidate.Length > 0)
                {
                    var typesValidated = new bool[providerTypesToValidate.Length];

                    for (int i = 0; i < providerTypesToValidate.Length; i++)
                    {
                        if (providerTypesToValidate[i] == null) { continue; }

                        for (var j = 0; j < registeredConfigurations.Length; j++)
                        {
                            var subProfile = registeredConfigurations[j];

                            if (subProfile.InstancedType?.Type == providerTypesToValidate[i])
                            {
                                typesValidated[i] = true;
                            }
                        }
                    }

                    for (var i = 0; i < typesValidated.Length; i++)
                    {
                        if (!typesValidated[i])
                        {
                            errorsFound = true;
                        }
                    }

                    if (errorsFound)
                    {
                        var errorDescription = new StringBuilder();
                        errorDescription.AppendLine("The following Data Providers were not found in the current Mixed Reality Configuration profile:\n");

                        for (int i = 0; i < typesValidated.Length; i++)
                        {
                            if (!typesValidated[i])
                            {
                                errorDescription.AppendLine($" [{providerTypesToValidate[i]}]");
                            }
                        }

                        errorDescription.AppendLine($"\nYou can either add this manually in\nInput Profile ->  Controller Data providers\n or click 'App Provider' to add this automatically");
#if UNITY_EDITOR
                        if (prompt)
                        {
                            if (EditorUtility.DisplayDialog($"{providerTypesToValidate[0]} provider not found", errorDescription.ToString(), "Ignore", "Add Provider"))
                            {
                                EditorPrefs.SetBool(IgnoreKey, true);
                            }
                            else
                            {
                                for (int i = 0; i < providerTypesToValidate.Length; i++)
                                {
                                    if (!typesValidated[i])
                                    {
                                        profile.RegisteredServiceConfigurations = registeredConfigurations.AddItem(providerDefaultConfiguration[i]);
                                    }
                                }

                                return true;
                            }
                        }
                        else
                        {
                            Debug.LogWarning(errorDescription);
                        }
#endif //UNITY_EDITOR
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Controller Mapping function to test for a controller mappings 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="mappingTypesToValidate">Array of controller mappings to validate</param>
        /// <param name="prompt">Unit Test helper, to control whether the UI prompt is offered or not</param>
        /// <returns></returns>
        public static bool ValidateControllerProfiles(this MixedRealityControllerMappingProfiles profile, Type[] mappingTypesToValidate, bool prompt = true)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || EditorPrefs.GetBool(IgnoreKey, false))
            {
                return false;
            }
#endif //UNITY_EDITOR

            if (MixedRealityToolkit.HasActiveProfile)
            {
                var errorsFound = false;
                var mappingConfigurationSource = profile.MixedRealityControllerMappings;

                if (mappingConfigurationSource != null)
                {
                    if (mappingTypesToValidate != null && mappingTypesToValidate.Length > 0)
                    {
                        var typesValidated = new bool[mappingTypesToValidate.Length];

                        for (int i = 0; i < mappingTypesToValidate.Length; i++)
                        {
                            foreach (var mappingProfile in mappingConfigurationSource)
                            {
                                if (mappingProfile.ControllerType == null) { continue; }

                                if (mappingProfile.ControllerType == mappingTypesToValidate[i])
                                {
                                    typesValidated[i] = true;
                                }
                            }
                        }

                        for (var i = 0; i < typesValidated.Length; i++)
                        {
                            if (!typesValidated[i])
                            {
                                errorsFound = true;
                            }
                        }

                        if (errorsFound)
                        {
                            var errorDescription = new StringBuilder();
                            errorDescription.AppendLine("The following Controller Types were not found in the current Mixed Reality Configuration profile:\n");

                            for (int i = 0; i < typesValidated.Length; i++)
                            {
                                if (!typesValidated[i])
                                {
                                    errorDescription.AppendLine($" [{mappingTypesToValidate[i]}]");
                                }
                            }

                            errorDescription.AppendLine($"\nYou will need to either create a profile and add it to your Controller mappings, or assign the default from the 'DefaultProfiles' folder in the associated package");
#if UNITY_EDITOR
                            if (prompt)
                            {
                                if (EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ignore", "Later"))
                                {
                                    EditorPrefs.SetBool(IgnoreKey, true);
                                }
                            }
#endif //UNITY_EDITOR
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Controller Mapping function to test for a controller mappings for a specific hand
        /// </summary>
        /// <param name="mappingTypesToValidate">Array of controller mappings to validate</param>
        /// <param name="controllerHandedness">The <see cref="Handedness"/> of the controller to validate</param>
        /// <param name="prompt">Unit Test helper, to control whether the UI prompt is offered or not</param>
        /// <returns></returns>
        public static bool ValidateControllerMappings(Type[] mappingTypesToValidate, Handedness controllerHandedness, bool prompt = true)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || EditorPrefs.GetBool(IgnoreKey, false))
            {
                return false;
            }
#endif //UNITY_EDITOR

            if (MixedRealityToolkit.HasActiveProfile)
            {
                var errorsFound = false;
                var mappingConfigurationSource = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles.MixedRealityControllerMappings;

                if (mappingConfigurationSource != null)
                {
                    if (mappingTypesToValidate != null && mappingTypesToValidate.Length > 0)
                    {
                        var typesValidated = new bool[mappingTypesToValidate.Length];

                        for (int i = 0; i < mappingTypesToValidate.Length; i++)
                        {
                            //Check if there is a valid mapping for the selected controller type and hand.  Additionally, if the hand is Left, then automatically check the right hand.
                            typesValidated[i] = controllerHandedness != Handedness.Left ||
                                                mappingConfigurationSource.GetControllerInteractionMapping(mappingTypesToValidate[i], Handedness.Right, out _) ||
                                                !mappingConfigurationSource.GetControllerInteractionMapping(mappingTypesToValidate[i], controllerHandedness, out _);

                            if (!typesValidated[i])
                            {
                                errorsFound = true;
                            }
                        }

                        if (errorsFound)
                        {
                            var errorDescription = new StringBuilder();
                            errorDescription.AppendLine("The following Controller Types were not found in the current Mixed Reality Configuration profile:");

                            for (int i = 0; i < typesValidated.Length; i++)
                            {
                                if (!typesValidated[i])
                                {
                                    errorDescription.AppendLine($" [{mappingTypesToValidate[i]}]");
                                }
                            }

                            errorDescription.AppendLine($"\nYou will need to either create a profile and add it to your Controller mappings, or assign the default from the 'DefaultProfiles' folder in the associated package");
#if UNITY_EDITOR
                            if (prompt)
                            {
                                if (EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ignore", "Later"))
                                {
                                    EditorPrefs.SetBool(IgnoreKey, true);
                                }
                            }
#endif //UNITY_EDITOR
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}