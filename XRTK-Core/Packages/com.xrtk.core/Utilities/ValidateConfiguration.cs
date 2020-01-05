// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Utilities.Editor
{
    public static class ValidateConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingTypesToValidate"></param>
        /// <returns></returns>
        public static bool ValidateControllerMappings(Type[] mappingTypesToValidate, Handedness controllerHandedness)
        {
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
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ok");
#endif //UNITY_EDITOR
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayDialog("No missing interactions", "No missing interactions", "Ok");
#endif //UNITY_EDITOR
                        return true;
                    }
                }
            }
            return false;
        }

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
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayDialog("Controller Mappings not found", errorDescription.ToString(), "Ok");
#endif //UNITY_EDITOR
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayDialog("No Controller Profiles missing", "No Controller Profiles missing", "Ok");
#endif //UNITY_EDITOR
                        return true;
                    }

                }
            }
            return false;
        }
    }
}