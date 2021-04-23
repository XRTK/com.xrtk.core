// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using XRTK.Definitions.InputSystem;

namespace XRTK.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MixedRealityInputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private readonly MixedRealityInputActionDropdown inputActionDropdown = new MixedRealityInputActionDropdown();

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            inputActionDropdown.OnGui(rect, property, content);
        }
    }
}
