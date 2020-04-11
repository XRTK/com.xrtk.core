// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.InputSystem;

namespace XRTK.Inspectors.Profiles.InputSystem
{
    [CustomEditor(typeof(MixedRealityInputDataProvidersProfile))]
    public class MixedRealityInputDataProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Controller Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the input sources your application can get input data from.", MessageType.Info);
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}