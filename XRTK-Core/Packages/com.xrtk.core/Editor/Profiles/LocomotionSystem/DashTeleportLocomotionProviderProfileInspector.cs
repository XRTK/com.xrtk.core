﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(DashTeleportLocomotionProviderProfile))]
    public class DashTeleportLocomotionProviderProfileInspector : BaseTeleportLocomotionProviderProfileInspector
    {
        private SerializedProperty dashDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            dashDuration = serializedObject.FindProperty(nameof(dashDuration));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(dashDuration);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
