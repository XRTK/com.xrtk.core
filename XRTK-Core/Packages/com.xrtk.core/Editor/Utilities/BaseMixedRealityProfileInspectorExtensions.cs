// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Extensions;
using XRTK.Extensions;

namespace XRTK.Editor.Utilities
{
    public static class BaseMixedRealityProfileInspectorExtensions
    {
        /// <summary>
        /// Creates a new <see cref="BaseMixedRealityProfile"/> instance and sets it to the <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="parentProfile"></param>
        /// <param name="property"></param>
        /// <param name="profileType"></param>
        /// <param name="clone"></param>
        public static BaseMixedRealityProfile CreateNewProfileInstance(this BaseMixedRealityProfile parentProfile, SerializedProperty property, Type profileType, bool clone = false)
        {
            ScriptableObject instance;

            if (profileType == null)
            {
                if (!string.IsNullOrWhiteSpace(property.type))
                {
                    var profileTypeName = property.type?.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    instance = ScriptableObject.CreateInstance(profileTypeName);
                }
                else
                {
                    Debug.LogError("No property type found!");
                    return null;
                }
            }
            else
            {
                instance = ScriptableObject.CreateInstance(profileType);
            }

            Debug.Assert(!instance.IsNull());

            var assetPath = !parentProfile.IsNull() ? AssetDatabase.GetAssetPath(parentProfile) : string.Empty;
            var newProfile = instance.CreateAsset(assetPath) as BaseMixedRealityProfile;
            Debug.Assert(!newProfile.IsNull());

            if (clone &&
                !property.objectReferenceValue.IsNull())
            {
                var oldProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                newProfile.CopySerializedValues(oldProfile);
            }

            newProfile.ParentProfile = parentProfile;
            property.objectReferenceValue = newProfile;
            return newProfile;
        }

        public static void CopySerializedValues(this BaseMixedRealityProfile target, BaseMixedRealityProfile source)
        {
            Debug.Assert(!target.IsNull());
            var serializedObject = new SerializedObject(target);
            Undo.RecordObject(target, "Paste Profile Values");
            var originalName = serializedObject.targetObject.name;
            EditorUtility.CopySerialized(source, serializedObject.targetObject);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            serializedObject.targetObject.name = originalName;
            AssetDatabase.SaveAssets();
        }
    }
}