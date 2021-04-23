// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Extensions
{
    /// <summary>
    /// Extensions for <see cref="EditorGUILayout"/> usage.
    /// </summary>
    public static class EditorGUILayoutExtensions
    {
        /// <summary>
        /// Draws a foldout but with bold label text.
        /// </summary>
        /// <param name="foldout">Foldout state.</param>
        /// <param name="content">Foldout label content.</param>
        /// <param name="toggleOnLabelClick">Should the foldout toggle on label click?</param>
        /// <returns>Returns true, if foldout unfolded.</returns>
        public static bool FoldoutWithBoldLabel(bool foldout, GUIContent content, bool toggleOnLabelClick = true)
        {
            GUIStyle defaultStyle = EditorStyles.foldout;
            FontStyle previousStyle = defaultStyle.fontStyle;
            defaultStyle.fontStyle = FontStyle.Bold;
            foldout = EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick);
            defaultStyle.fontStyle = previousStyle;

            return foldout;
        }
    }
}