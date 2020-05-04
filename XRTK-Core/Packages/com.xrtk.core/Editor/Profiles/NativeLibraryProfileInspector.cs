// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Services;

namespace XRTK.Editor.Profiles
{
    [CustomEditor(typeof(NativeLibrarySystemProfile))]
    public class NativeLibraryProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines any additional native data providers to register with the Mixed Reality Toolkit.");

            base.OnInspectorGUI();
        }
    }
}