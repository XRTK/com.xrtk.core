using System;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Inspectors
{
    /// <summary>
    /// Popup window to help facilitate fixing broken serialized <see cref="Type"/> references.
    /// </summary>
    public class SystemTypeRepairWindow : EditorWindow
    {
        public static bool WindowOpen => window != null;

        private static Type[] repairedTypeOptions;
        private static SerializedProperty property;
        private static SystemTypeRepairWindow window;
        private static readonly GUIContent WindowTitleContent = new GUIContent("Select repaired type");

        /// <summary>
        /// Displays a list of available types to use to repair the reference.
        /// </summary>
        /// <param name="typeOptions">The types that the user can choose from to repair the reference.</param>
        /// <param name="serializedTypeProperty">The serialized property that's missing the reference.</param>
        public static void Display(Type[] typeOptions, SerializedProperty serializedTypeProperty)
        {
            if (window != null) { window.Close(); }

            repairedTypeOptions = typeOptions;
            property = serializedTypeProperty;

            window = CreateInstance(typeof(SystemTypeRepairWindow)) as SystemTypeRepairWindow;
            Debug.Assert(window != null);
            window.titleContent = WindowTitleContent;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            foreach (var type in repairedTypeOptions)
            {
                if (GUILayout.Button(type.FullName, EditorStyles.miniButton))
                {
                    property.stringValue = SystemType.GetReference(type);
                    property.serializedObject.ApplyModifiedProperties();
                    Close();
                }
            }
        }

        private void OnDisable() => window = null;

        private void OnInspectorUpdate() => Repaint();
    }
}