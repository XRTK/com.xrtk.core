// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(MixedRealityRegisteredServiceProvidersProfile))]
    public class MixedRealityRegisteredServiceProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines any additional Services like systems, features, and managers to register with the Mixed Reality Toolkit.\n\nNote: The order of the list determines the order these services get created and the priority they get events such as Enable, Update, and Disable.");

            base.OnInspectorGUI();
        }
    }
}