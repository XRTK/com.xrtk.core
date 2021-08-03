// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealityBlinkTeleportLocomotionProviderProfile))]
    public class MixedRealityBlinkTeleportLocomotionProviderProfileInspector : BaseTeleportLocomotionProviderProfileInspector
    {
        private SerializedProperty inputAction;
        private SerializedProperty fadeDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputAction = serializedObject.FindProperty(nameof(inputAction));
            fadeDuration = serializedObject.FindProperty(nameof(fadeDuration));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(inputAction);
            EditorGUILayout.PropertyField(fadeDuration);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
