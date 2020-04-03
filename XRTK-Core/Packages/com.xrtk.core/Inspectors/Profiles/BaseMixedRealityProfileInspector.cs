// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Extensions;
using XRTK.Inspectors.PropertyDrawers;
using XRTK.Inspectors.Utilities;
using XRTK.Services;
using XRTK.Utilities.Async;

namespace XRTK.Inspectors.Profiles
{
    /// <summary>
    /// Base class for all <see cref="BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityProfileInspector : Editor
    {
        private const string IsEditableProfileProperty = "isEditable";

        private static SerializedObject targetProfile;
        private static BaseMixedRealityProfile currentlySelectedProfile;
        private static BaseMixedRealityProfile profileSource;

        protected BaseMixedRealityProfile ThisProfile { get; private set; }

        protected virtual void OnEnable()
        {
            targetProfile = serializedObject;
            currentlySelectedProfile = target as BaseMixedRealityProfile;
            Debug.Assert(currentlySelectedProfile != null);
            ThisProfile = currentlySelectedProfile;
        }

        protected void RenderHeader()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (ThisProfile.ParentProfile != null &&
                GUILayout.Button("Back to parent profile"))
            {
                Selection.activeObject = ThisProfile.ParentProfile;
            }

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Creates a new <see cref="BaseMixedRealityProfile"/> instance and sets it to the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="parentProfile"></param>
        /// <param name="property"></param>
        /// <param name="profileType"></param>
        protected static BaseMixedRealityProfile CreateNewProfileInstance(BaseMixedRealityProfile parentProfile, SerializedProperty property, Type profileType = null)
        {
            ScriptableObject instance;

            if (profileType == null)
            {
                if (!string.IsNullOrWhiteSpace(property.type))
                {
                    var profileTypeName = property.type?.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    instance = CreateInstance(profileTypeName);
                }
                else
                {
                    Debug.LogError("No property type found!");
                    return null;
                }
            }
            else
            {
                instance = CreateInstance(profileType);
            }

            Debug.Assert(Selection.activeObject != null);
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var newProfile = instance.CreateAsset(assetPath) as BaseMixedRealityProfile;
            Debug.Assert(newProfile != null);
            newProfile.ParentProfile = parentProfile;
            property.objectReferenceValue = newProfile;
            return newProfile;
        }

        /// <summary>
        /// Clones an instance of a <see cref="BaseMixedRealityProfile"/> and sets it to the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="parentProfile"></param>
        /// <param name="property"></param>
        /// <param name="renderedProfile"></param>
        protected static void CloneProfileInstance(BaseMixedRealityProfile parentProfile, SerializedProperty property, BaseMixedRealityProfile renderedProfile)
        {
            profileSource = renderedProfile;
            var newProfile = CreateNewProfileInstance(parentProfile, property, renderedProfile.GetType());
            PasteProfileValuesDelay(newProfile);
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Create Clone from Profile Values", false, 0)]
        protected static async void CreateCloneProfile()
        {
            profileSource = currentlySelectedProfile;
            var newProfile = CreateInstance(currentlySelectedProfile.GetType().ToString());
            currentlySelectedProfile = newProfile.CreateAsset() as BaseMixedRealityProfile;
            Debug.Assert(currentlySelectedProfile != null);

            await new WaitUntil(() => profileSource != currentlySelectedProfile);

            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = currentlySelectedProfile;
            EditorGUIUtility.PingObject(currentlySelectedProfile);

            if (!profileSource.IsEditable)
            {
                // For now we only replace it if it's the master settings profile.
                // Sub-profiles are easy to update in the master settings inspector.
                if (currentlySelectedProfile is MixedRealityToolkitRootProfile rootProfile)
                {
                    MixedRealityToolkit.Instance.ActiveProfile = rootProfile;
                }
            }
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Copy Profile Values", false, 1)]
        private static void CopyProfileValues()
        {
            profileSource = currentlySelectedProfile;
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", true)]
        private static bool PasteProfileValuesValidation()
        {
            return currentlySelectedProfile != null &&
                   targetProfile != null &&
                   profileSource != null &&
                   targetProfile.FindProperty(IsEditableProfileProperty).boolValue &&
                   currentlySelectedProfile.GetType() == profileSource.GetType();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", false, 2)]
        private static void PasteProfileValues()
        {
            currentlySelectedProfile.CopySerializedValues(profileSource);
        }

        private static async void PasteProfileValuesDelay(BaseMixedRealityProfile newProfile)
        {
            await new WaitUntil(() => currentlySelectedProfile == newProfile);
            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = newProfile;
        }
    }
}
