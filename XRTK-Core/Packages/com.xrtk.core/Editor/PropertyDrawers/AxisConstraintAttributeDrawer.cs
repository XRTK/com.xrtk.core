// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.InputSystem;

namespace XRTK.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AxisConstraintAttribute))]
    public class AxisConstraintAttributeDrawer : PropertyDrawer
    {
        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var constraintAttribute = attribute as AxisConstraintAttribute;

            if (property.type == nameof(MixedRealityInputAction))
            {
                inputActionDropdown.OnGui(position, property, label, constraintAttribute.AxisConstraint);
            }
            else
            {
                var color = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, $"{nameof(AxisConstraintAttribute)} is not supported for {property.type}");
                GUI.color = color;
            }
        }
    }
}