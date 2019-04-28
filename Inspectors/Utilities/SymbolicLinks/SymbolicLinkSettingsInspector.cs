// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace XRTK.Inspectors.Utilities.SymbolicLinks
{
    [CustomEditor(typeof(SymbolicLinkSettings))]
    public class SymbolicLinkSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}