// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.Controllers;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityControllerDataProvidersProfile))]
    public class MixedRealityControllerDataProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("Below is a list of registered Controller Data Providers for each platform that defines how controllers are setup and work.");

            base.OnInspectorGUI();
        }
    }
}