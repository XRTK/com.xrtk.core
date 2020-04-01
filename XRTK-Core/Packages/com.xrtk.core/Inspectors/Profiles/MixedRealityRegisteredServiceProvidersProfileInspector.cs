// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;

namespace XRTK.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityRegisteredServiceProvidersProfile))]
    public class MixedRealityRegisteredServiceProvidersProfileInspector : MixedRealityServiceProfileInspector
    {
        public override void OnInspectorGUI()
        {
            RenderHeader();

            EditorGUILayout.LabelField("Registered Service Providers Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This profile defines any additional Services like systems, features, and managers to register with the Mixed Reality Toolkit.\n\n" +
                                    "Note: The order of the list determines the order these services get created.", MessageType.Info);

            ThisProfile.CheckProfileLock();

            base.OnInspectorGUI();
        }
    }
}