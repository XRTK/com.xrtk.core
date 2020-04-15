// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using UnityEditor;
using XRTK.Definitions.CameraSystem;

namespace XRTK.Inspectors.Profiles.CameraSystem
{
    [CustomEditor(typeof(MixedRealityCameraDataProvidersProfile))]
    public class MixedRealityCameraDataProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("Use this profile to define and control camera sources in your application.\n\nBelow is a list of registered Camera Data Providers for each platform.");

            base.OnInspectorGUI();
        }
    }
}