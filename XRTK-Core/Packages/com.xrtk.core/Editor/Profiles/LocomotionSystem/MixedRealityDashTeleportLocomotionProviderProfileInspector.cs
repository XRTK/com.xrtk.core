// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityDashTeleportLocomotionProviderProfile))]
    public class MixedRealityDashTeleportLocomotionProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty dashDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            dashDuration = serializedObject.FindProperty(nameof(dashDuration));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines behaviour for the dash teleport locomotion implementation of XRTK.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(dashDuration);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
