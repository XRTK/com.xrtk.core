// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Extensions;
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
            Debug.Log("CHANGED");
            if (!MixedRealityToolkit.Instance.IsNull())
            {
                if (MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
                {
                    Debug.Log("INPUT SYSTEM ENABLED");
                    // Make sure unity axis mappings are set.
                    Utilities.InputMappingAxisUtility.CheckUnityInputManagerMappings(Utilities.ControllerMappingUtilities.UnityInputManagerAxes);
                }
                else
                {
                    Debug.Log("INPUT SYSTEM DISABLED");
                    Utilities.InputMappingAxisUtility.RemoveMappings(Utilities.ControllerMappingUtilities.UnityInputManagerAxes);
                }
            }
        }
    }
}