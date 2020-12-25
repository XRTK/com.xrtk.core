// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Extensions
{
    /// <summary>
    /// Extensions for <see cref="SerializedProperty"/> usage.
    /// </summary>
    public static class SerializedPropertyExtensions
    {

        /// <summary>
        /// Draws a foldout but with bold label text.
        /// </summary>
        /// <param name="property"><see cref="SerializedProperty"/> to check if expanded.</param>
        /// <param name="foldoutContent">Foldout label content.</param>
        /// <param name="showPropertyChildren">Should the property field render its children?</param>
        /// <param name="layoutOptions">Optional <see cref="GUILayoutOption"/> options.</param>
        /// <param name="propertyContent"><see cref="SerializedProperty"/> label content.</param>
        /// <returns>Returns true, if foldout unfolded.</returns>
        public static bool FoldoutWithBoldLabelPropertyField(this SerializedProperty property, GUIContent foldoutContent, GUIContent propertyContent, bool showPropertyChildren = true, params GUILayoutOption[] layoutOptions)
        {
            return property.FoldoutWithBoldLabelPropertyField(foldoutContent, true, propertyContent, showPropertyChildren, layoutOptions);
        }

        /// <summary>
        /// Draws a foldout but with bold label text.
        /// </summary>
        /// <param name="property"><see cref="SerializedProperty"/> to check if expanded.</param>
        /// <param name="foldoutContent">Foldout label content.</param>
        /// <param name="toggleOnLabelClick">Should the foldout toggle on label click?</param>
        /// <param name="showPropertyChildren">Should the property field render its children?</param>
        /// <param name="layoutOptions">Optional <see cref="GUILayoutOption"/> options.</param>
        /// <param name="propertyContent"><see cref="SerializedProperty"/> label content.</param>
        /// <returns>Returns true, if foldout unfolded.</returns>
        public static bool FoldoutWithBoldLabelPropertyField(this SerializedProperty property, GUIContent foldoutContent, bool toggleOnLabelClick = true, GUIContent propertyContent = null, bool showPropertyChildren = true, params GUILayoutOption[] layoutOptions)
        {
            var defaultStyle = EditorStyles.foldout;
            var previousStyle = defaultStyle.fontStyle;
            defaultStyle.fontStyle = FontStyle.Bold;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, foldoutContent, toggleOnLabelClick);
            defaultStyle.fontStyle = previousStyle;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property, propertyContent, showPropertyChildren, layoutOptions);
                EditorGUI.indentLevel--;
            }

            return property.isExpanded;
        }
    }
}