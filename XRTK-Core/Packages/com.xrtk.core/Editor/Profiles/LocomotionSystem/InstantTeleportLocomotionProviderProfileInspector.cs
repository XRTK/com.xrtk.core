// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(InstantTeleportLocomotionProviderProfile))]
    public class InstantTeleportLocomotionProviderProfileInspector : BaseTeleportLocomotionProviderProfileInspector
    {
        private SerializedProperty inputAction;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputAction = serializedObject.FindProperty(nameof(inputAction));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(inputAction);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
