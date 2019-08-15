// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Inspectors.Profiles;
using XRTK.Inspectors.Utilities;

namespace XRTK.Definitions.Controllers.OpenVR.Inspectors.Profiles
{
    [CustomEditor(typeof(HandControllerDataProviderProfile))]
    public class HandControllerDataProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        public override void OnInspectorGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (thisProfile.ParentProfile != null &&
                GUILayout.Button("Back to Controller Data Providers"))
            {
                Selection.activeObject = thisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hand Controller Data Provider Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This profile aids in configuring additional platform settings for the registered controller data provider.", MessageType.Info);

            thisProfile.CheckProfileLock();
        }
    }
}