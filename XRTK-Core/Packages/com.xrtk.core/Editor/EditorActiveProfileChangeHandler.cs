// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;

namespace XRTK.Editor
{
    [InitializeOnLoad]
    public static class EditorActiveProfileChangeHandler
    {
        static EditorActiveProfileChangeHandler()
        {
            EditorApplication.hierarchyChanged += EditorApplication_hierarchyChanged;
        }

        private static void EditorApplication_hierarchyChanged()
        {
            if (MixedRealityToolkit.HasActiveProfile)
            {
                if (MixedRealityToolkit.IsSystemEnabled<IMixedRealityInputSystem>() &&
                    Utilities.InputMappingAxisUtility.CheckUnityInputManagerMappings(Utilities.ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log("XRTK Input System was enabled, updated input axis mappings.");
                }
                else if (!MixedRealityToolkit.IsSystemEnabled<IMixedRealityInputSystem>() &&
                    Utilities.InputMappingAxisUtility.RemoveMappings(Utilities.ControllerMappingUtilities.UnityInputManagerAxes))
                {
                    Debug.Log("XRTK Input System was disabled, removed input axis mappings.");
                }
            }
        }
    }
}
