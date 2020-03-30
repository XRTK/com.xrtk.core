// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Extensions;
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
        private const string IsDefaultProfileProperty = "isEditable";

        protected static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        protected static readonly GUIContent CloneProfileContent = new GUIContent("Clone", "Replace with a copy of the default profile.");

        private static SerializedObject targetProfile;
        private static BaseMixedRealityProfile profile;
        private static BaseMixedRealityProfile profileToCopy;

        protected BaseMixedRealityProfile ThisProfile { get; private set; }

        protected virtual void OnEnable()
        {
            targetProfile = serializedObject;
            profile = target as BaseMixedRealityProfile;
            Debug.Assert(profile != null);
            ThisProfile = profile;
        }

        protected void RenderHeader()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            if (ThisProfile.ParentProfile != null &&
                GUILayout.Button("Back to parent profile"))
            {
                Selection.activeObject = ThisProfile.ParentProfile;
            }
        }

        /// <summary>
        /// Renders a <see cref="BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="parentProfile">The <see cref="BaseMixedRealityProfile"/> parent of the profile being rendered.</param>
        /// <param name="property">the <see cref="BaseMixedRealityProfile"/> property.</param>
        /// <param name="guiContent">The GUIContent for the field.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(BaseMixedRealityProfile parentProfile, SerializedProperty property, GUIContent guiContent, bool showAddButton = true)
        {
            return RenderProfileInternal(parentProfile, property, guiContent, showAddButton);
        }

        /// <summary>
        /// Renders a <see cref="BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="parentProfile">The <see cref="BaseMixedRealityProfile"/> parent of the profile being rendered.</param>
        /// <param name="property">the <see cref="BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">Optional flag to hide the create button.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(BaseMixedRealityProfile parentProfile, SerializedProperty property, bool showAddButton = true)
        {
            return RenderProfileInternal(parentProfile, property, null, showAddButton);
        }

        private static bool RenderProfileInternal(BaseMixedRealityProfile parentProfile, SerializedProperty property, GUIContent guiContent, bool showAddButton)
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal();

            var oldObject = property.objectReferenceValue;

            if (guiContent == null)
            {
                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.PropertyField(property, guiContent);
            }

            if (property.objectReferenceValue == null)
            {
                if (showAddButton &&
                    GUILayout.Button(NewProfileContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                {
                    CreateNewProfileInstance(parentProfile, property);
                    changed = true;
                }
            }
            else
            {
                var renderedProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                Debug.Assert(renderedProfile != null);
                Debug.Assert(profile != null, "No profile was set in OnEnable. Did you forget to call base.OnEnable in a derived profile class?");

                if (profile.IsEditable &&
                    !renderedProfile.IsEditable &&
                    GUILayout.Button(CloneProfileContent, EditorStyles.miniButton, GUILayout.Width(42f)))
                {
                    CloneProfileInstance(parentProfile, property, renderedProfile);
                    changed = true;
                }
            }

            if (property.objectReferenceValue != null)
            {
                var renderedProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                Debug.Assert(renderedProfile != null);

                if (!(renderedProfile is MixedRealityToolkitConfigurationProfile) &&
                     (renderedProfile.ParentProfile == null ||
                      renderedProfile.ParentProfile != parentProfile))
                {
                    renderedProfile.ParentProfile = parentProfile;
                }
            }

            if (oldObject != property.objectReferenceValue)
            {
                changed = true;
            }

            EditorGUILayout.EndHorizontal();
            return changed;
        }

        /// <summary>
        /// Creates a new <see cref="BaseMixedRealityProfile"/> instance and sets it to the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="parentProfile"></param>
        /// <param name="property"></param>
        /// <param name="profileType"></param>
        protected static void CreateNewProfileInstance(BaseMixedRealityProfile parentProfile, SerializedProperty property, Type profileType = null)
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
                    return;
                }
            }
            else
            {
                instance = CreateInstance(profileType);
            }

            var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
            Debug.Assert(newProfile != null);
            newProfile.ParentProfile = parentProfile;
            property.objectReferenceValue = newProfile;
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Clones an instance of a <see cref="BaseMixedRealityProfile"/> and sets it to the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="parentProfile"></param>
        /// <param name="property"></param>
        /// <param name="renderedProfile"></param>
        protected static void CloneProfileInstance(BaseMixedRealityProfile parentProfile, SerializedProperty property, BaseMixedRealityProfile renderedProfile)
        {
            profileToCopy = renderedProfile;
            var instance = CreateInstance(renderedProfile.GetType());
            var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
            Debug.Assert(newProfile != null);
            newProfile.ParentProfile = parentProfile;
            property.objectReferenceValue = newProfile;
            property.serializedObject.ApplyModifiedProperties();
            PasteProfileValuesDelay(newProfile);
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Create Clone from Profile Values", false, 0)]
        protected static async void CreateCloneProfile()
        {
            profileToCopy = profile;
            var newProfile = CreateInstance(profile.GetType().ToString());
            profile = newProfile.CreateAsset() as BaseMixedRealityProfile;
            Debug.Assert(profile != null);

            await new WaitUntil(() => profileToCopy != profile);

            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = profile;
            EditorGUIUtility.PingObject(profile);

            if (!profileToCopy.IsEditable)
            {
                // For now we only replace it if it's the master configuration profile.
                // Sub-profiles are easy to update in the master configuration inspector.
                if (MixedRealityToolkit.Instance.ActiveProfile.GetType() == profile.GetType())
                {
                    MixedRealityToolkit.Instance.ActiveProfile = profile as MixedRealityToolkitConfigurationProfile;
                }
            }
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Copy Profile Values", false, 1)]
        private static void CopyProfileValues()
        {
            profileToCopy = profile;
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", true)]
        private static bool PasteProfileValuesValidation()
        {
            return profile != null &&
                   targetProfile != null &&
                   profileToCopy != null &&
                   targetProfile.FindProperty(IsDefaultProfileProperty).boolValue &&
                   profile.GetType() == profileToCopy.GetType();
        }

        [MenuItem("CONTEXT/BaseMixedRealityProfile/Paste Profile Values", false, 2)]
        private static void PasteProfileValues()
        {
            Undo.RecordObject(profile, "Paste Profile Values");
            var targetIsCustom = targetProfile.FindProperty(IsDefaultProfileProperty).boolValue;
            var originalName = targetProfile.targetObject.name;
            EditorUtility.CopySerialized(profileToCopy, targetProfile.targetObject);
            targetProfile.Update();
            targetProfile.FindProperty(IsDefaultProfileProperty).boolValue = targetIsCustom;
            targetProfile.ApplyModifiedProperties();
            targetProfile.targetObject.name = originalName;
            Debug.Assert(targetProfile.FindProperty(IsDefaultProfileProperty).boolValue == targetIsCustom);
            AssetDatabase.SaveAssets();
        }

        private static async void PasteProfileValuesDelay(BaseMixedRealityProfile newProfile)
        {
            await new WaitUntil(() => profile == newProfile);
            Selection.activeObject = null;
            PasteProfileValues();
            Selection.activeObject = newProfile;
        }
    }
}
