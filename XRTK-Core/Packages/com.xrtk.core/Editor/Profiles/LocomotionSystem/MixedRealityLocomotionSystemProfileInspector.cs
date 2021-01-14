// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityLocomotionSystemProfile))]
    public class MixedRealityLocomotionSystemProfileInspector : MixedRealityServiceProfileInspector
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("The locomotion system profile defines behaviour for the locomotion system and how your users will be able to move around in the application.");

            base.OnInspectorGUI();
        }
    }
}