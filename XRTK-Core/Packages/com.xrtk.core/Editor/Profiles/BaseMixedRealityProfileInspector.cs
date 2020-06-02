// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Editor.Extensions;
using XRTK.Utilities.Async;

namespace XRTK.Editor.Profiles
{
    /// <summary>
    /// Base class for all <see cref="BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    [CustomEditor(typeof(BaseMixedRealityProfile), true, isFallback = true)]
    public class BaseMixedRealityProfileInspector : UnityEditor.Editor
    {
        protected static readonly string DefaultGuidString = default(Guid).ToString("N");

        private static SerializedObject targetProfile;
        private static BaseMixedRealityProfile currentlySelectedProfile;
        private static BaseMixedRealityProfile profileSource;

        /// <summary>
        /// The <see cref="Guid"/> string representation for this profile asset.
        /// </summary>
        protected string ThisProfileGuidString { get; private set; }

        /// <summary>
        /// The instanced reference of the currently rendered <see cref="BaseMixedRealityProfile"/>.
        /// </summary>
        protected BaseMixedRealityProfile ThisProfile { get; private set; }

        private bool isOverrideHeader = false;

        protected virtual void OnEnable()
        {
            targetProfile = serializedObject;
            currentlySelectedProfile = target as BaseMixedRealityProfile;
            Debug.Assert(!currentlySelectedProfile.IsNull());
            ThisProfile = currentlySelectedProfile;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(ThisProfile, out var guidHex, out long _);
            ThisProfileGuidString = guidHex;
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();
            DrawDefaultInspector();
        }

        protected void RenderHeader(string infoBoxText = "", Texture2D image = null)
        {
            if (!image.IsNull() ||
                !string.IsNullOrWhiteSpace(infoBoxText))
            {
                isOverrideHeader = true;
            }
            else
            {
                if (isOverrideHeader) { return; }
            }

            if (image.IsNull())
            {
                MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            }
            else
            {
                MixedRealityInspectorUtility.RenderInspectorHeader(image);
            }

            if (!ThisProfile.ParentProfile.IsNull() &&
                GUILayout.Button("Back to parent profile"))
            {
                Selection.activeObject = ThisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"{ThisProfile.name.ToProperCase()} Settings", EditorStyles.boldLabel);

            if (isOverrideHeader)
            {
                EditorGUILayout.HelpBox(infoBoxText, MessageType.Info);
            }

            EditorGUILayout.Space();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Create Clone from Profile Values", false, 0)]
        protected static async void CreateCloneProfile()
        {
            profileSource = currentlySelectedProfile;
            var newProfile = CreateInstance(currentlySelectedProfile.GetType().ToString());
            currentlySelectedProfile = newProfile.CreateAsset() as BaseMixedRealityProfile;
            Debug.Assert(!currentlySelectedProfile.IsNull());

            await new WaitUntil(() => profileSource != currentlySelectedProfile);

            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = currentlySelectedProfile;
            EditorGUIUtility.PingObject(currentlySelectedProfile);
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Copy Profile Values", false, 1)]
        private static void CopyProfileValues()
        {
            profileSource = currentlySelectedProfile;
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", true)]
        private static bool PasteProfileValuesValidation()
        {
            return !currentlySelectedProfile.IsNull() &&
                   targetProfile != null &&
                   !profileSource.IsNull() &&
                   currentlySelectedProfile.GetType() == profileSource.GetType();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", false, 2)]
        private static void PasteProfileValues()
        {
            currentlySelectedProfile.CopySerializedValues(profileSource);
        }
    }
}