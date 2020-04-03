// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Inspectors.Utilities;
using XRTK.Services;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(NativeLibrarySystemProfile))]
    public class NativeLibraryProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Native Data Providers Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This profile defines any additional native data providers to register with the Mixed Reality Toolkit.\n\n" +
                                    "Note: The order of the list determines the order these services get created.", MessageType.Info);
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}