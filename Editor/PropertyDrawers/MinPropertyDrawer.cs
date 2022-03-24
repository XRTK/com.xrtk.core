// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="MinAttribute"/> decorated <see cref="float"/> or <see cref="int"/> values rendered in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(MinAttribute))]
    public class MinPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minAttribute = (MinAttribute)attribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(position, property);

                    if (EditorGUI.EndChangeCheck() &&
                        property.floatValue < minAttribute.min)
                    {
                        property.floatValue = minAttribute.min;
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(position, property);

                    if (EditorGUI.EndChangeCheck() &&
                        property.intValue < minAttribute.min)
                    {
                        property.intValue = (int)minAttribute.min;
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    break;
                default:
                    EditorGUI.LabelField(position, label.text, $"Use {nameof(MinAttribute)} with float or int fields only.");
                    break;
            }
        }
    }
}