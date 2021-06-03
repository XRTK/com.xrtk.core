// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(BaseLocomotionProviderProfile))]
    public class MixedRealityLocomotionProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty startupBehaviour;

        protected override void OnEnable()
        {
            base.OnEnable();

            startupBehaviour = serializedObject.FindProperty(nameof(startupBehaviour));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines behaviour for the locomotion provider.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(startupBehaviour);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
