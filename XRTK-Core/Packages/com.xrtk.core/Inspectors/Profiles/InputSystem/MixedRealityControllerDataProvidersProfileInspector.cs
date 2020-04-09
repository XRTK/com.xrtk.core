// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputDataProvidersProfile))]
    public class MixedRealityControllerDataProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("Below is a list of registered Input Data Providers for each platform that defines how Input devices, controllers, and other inputs are setup and work.");

            base.OnInspectorGUI();
        }
    }
}