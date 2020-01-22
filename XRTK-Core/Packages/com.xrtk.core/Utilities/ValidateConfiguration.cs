// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Utilities.Editor
{
    public static class ValidateConfiguration
    {
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreControllerMappingsPrompts";

        /// <summary>
        /// Controller Mapping function to test for a controller mappings for a specific hand
        /// </summary>
        /// <param name="mappingTypesToValidate">Array of controller mappings to validate</param>
        /// <param name="controllerHandedness">The <see cref="Handedness"/> of the controller to valiadte</param>
        /// <returns></returns>
        public static bool ValidateControllerMappings(Type[] mappingTypesToValidate, Handedness controllerHandedness)
        {
            if (Application.isPlaying || EditorPrefs.GetBool(IgnoreKey, false) )
            {
                return false;
            }

            if (MixedRealityToolkit.HasActiveProfile)
            {
                var mappingConfigurationSource = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles.MixedRealityControllerMappings;
                MixedRealityControllerMapping controllerMapping = MixedRealityControllerMapping.None;

                bool errorsFound = false;

                if (mappingTypesToValidate != null && mappingTypesToValidate.Length > 0)
                {
                    var typesValidated = new bool[mappingTypesToValidate.Length];
                    for (int i = 0; i < mappingTypesToValidate.Length; i++)
                    {
                        //Check if there is a valid mapping for the selected controller type and hand.  Additionally, if the hand is Left, then automatically check the right hand.
                        typesValidated[i] = mappingConfigurationSource.GetControllerInteractionMapping(mappingTypesToValidate[i], controllerHandedness, out controllerMapping) &&
                            controllerHandedness == Handedness.Left ? mappingConfigurationSource.GetControllerInteractionMapping(mappingTypesToValidate[i], Handedness.Right, out controllerMapping) : true;

                        if (!typesValidated[i])
                        {
                            errorsFound = true;
                        }
                    }

                    if (errorsFound)
                    {
                        StringBuilder errorDescription = new StringBuilder();
                        errorDescription.AppendLine("The following Controller Types were not found in the current Mixed Reality Configuration profile:");
                        for (int i = 0; i < typesValidated.Length; i++)
                        {
                            if (!typesValidated[i])
                            {
                                errorDescription.AppendLine($" - [{mappingTypesToValidate[i]}]");
                            }
                        }
                        errorDescription.AppendLine($"\nYou will need to either create a profile and add it to your Controller mappings, or assign the default from the 'DefaultProfiles' folder in the associated package");
#if UNITY_EDITOR
                        if (EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ignore", "Later"))
                        {
                            EditorPrefs.SetBool(IgnoreKey, true);
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
        /// <param name="mappingTypesToValidate">Array of controller mappings to validate</param>
        /// <returns></returns>
        public static bool ValidateControllerProfiles(Type[] mappingTypesToValidate)
        {
            if (MixedRealityToolkit.HasActiveProfile)
            {
                var mappingConfigurationSource = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles.MixedRealityControllerMappings;
                bool errorsFound = false;

                if (mappingTypesToValidate != null && mappingTypesToValidate.Length > 0)
                {
                    var typesValidated = new bool[mappingTypesToValidate.Length];
                    for (int i = 0; i < mappingTypesToValidate.Length; i++)
                    {
                        foreach (var profile in mappingConfigurationSource)
                        {
                            if (profile.ControllerType == null) continue;

                            if (profile.ControllerType == mappingTypesToValidate[i])
                            {
                                typesValidated[i] = true;
                            }
                        }
                    }

                    foreach (var test in typesValidated)
                    {
                        if (!test)
                        {
                            errorsFound = true;
                        }
                    }

                    if (errorsFound)
                    {
                        StringBuilder errorDescription = new StringBuilder();
                        errorDescription.AppendLine("The following Controller Types were not found in the current Mixed Reality Configuration profile:\n");
                        for (int i = 0; i < typesValidated.Length; i++)
                        {
                            if (!typesValidated[i])
                            {
                                errorDescription.AppendLine($" - [{mappingTypesToValidate[i]}]");
                            }
                        }
                        errorDescription.AppendLine($"\nYou will need to either create a profile and add it to your Controller mappings, or assign the default from the 'DefaultProfiles' folder in the associated package");
#if UNITY_EDITOR
                        if (EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ignore", "Later"))
                        {
                            EditorPrefs.SetBool(IgnoreKey, true);
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
    }
}