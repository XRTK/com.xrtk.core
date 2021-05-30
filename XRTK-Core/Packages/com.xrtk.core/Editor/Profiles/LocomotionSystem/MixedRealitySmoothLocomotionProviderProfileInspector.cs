// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Editor.Profiles.LocomotionSystem
{
    [CustomEditor(typeof(MixedRealitySmoothLocomotionProviderProfile))]
    public class MixedRealitySmoothLocomotionProviderProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty speed;

        private static readonly GUIContent speedLabel = new GUIContent("Speed (m/s)");

        protected override void OnEnable()
        {
            base.OnEnable();

            speed = serializedObject.FindProperty(nameof(speed));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader("This profile defines behaviour for the smooth locomotion implementation of XRTK.");

            serializedObject.Update();

            EditorGUILayout.PropertyField(speed, speedLabel);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
