// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEditor;
using XRTK.Definitions.Utilities;

namespace XRTK.Editor.Data
{
    internal class ConfigurationProperty
    {
        private readonly SerializedProperty configuration;

        public bool IsExpanded
        {
            get => configuration.isExpanded;
            set => configuration.isExpanded = value;
        }

        private readonly SerializedProperty name;

        public string Name
        {
            get => name.stringValue;
            set => name.stringValue = value;
        }

        private readonly SerializedProperty priority;

        public uint Priority
        {
            get => (uint)priority.intValue;
            set => priority.intValue = (int)value;
        }

        private readonly SerializedProperty instancedType;
        private readonly SerializedProperty reference;

        public Type InstancedType
        {
            get => new SystemType(instancedType);
            set => reference.stringValue = value == null ? string.Empty : value.GUID.ToString();
        }

        private readonly SerializedProperty platformEntries;
        private readonly SerializedProperty runtimePlatforms;

        private readonly SerializedProperty profile;

        public UnityEngine.Object Profile
        {
            get => profile.objectReferenceValue;
            set => profile.objectReferenceValue = value;
        }

        public ConfigurationProperty(SerializedProperty property, bool clearPlatforms = false)
        {
            configuration = property;
            name = property.FindPropertyRelative(nameof(name));
            priority = property.FindPropertyRelative(nameof(priority));
            instancedType = property.FindPropertyRelative(nameof(instancedType));
            reference = instancedType.FindPropertyRelative(nameof(reference));
            platformEntries = property.FindPropertyRelative(nameof(platformEntries));
            runtimePlatforms = platformEntries.FindPropertyRelative(nameof(runtimePlatforms));

            if (clearPlatforms)
            {
                runtimePlatforms.ClearArray();
            }

            profile = property.FindPropertyRelative(nameof(profile));
        }

        public void ApplyModifiedProperties()
        {
            configuration.serializedObject.ApplyModifiedProperties();
        }
    }
}
