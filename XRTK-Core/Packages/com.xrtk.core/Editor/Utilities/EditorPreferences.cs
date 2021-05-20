// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// Convenience class for setting Editor Preferences with <see cref="Application.productName"/> as key prefix.
    /// </summary>
    [InitializeOnLoad]
    public static class EditorPreferences
    {
        static EditorPreferences()
        {
            applicationProductName = Application.productName;
            applicationDataPath = Application.dataPath;
        }

        private static string applicationDataPath;

        public static string ApplicationDataPath = applicationDataPath ?? (applicationDataPath = Application.dataPath);

        private static string applicationProductName = null;

        public static string ApplicationProductName => applicationProductName ?? (applicationProductName = Application.productName);

        /// <summary>
        /// Set the saved <see cref="string"/> from to <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetString($"{ApplicationProductName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="bool"/> from to <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, bool value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetBool($"{ApplicationProductName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="float"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, float value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetFloat($"{ApplicationProductName}_{key}", value);
        }

        /// <summary>
        /// Set the saved <see cref="int"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, int value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));
            EditorPrefs.SetInt($"{ApplicationProductName}_{key}", value);
        }

        /// <summary>
        /// Get the saved <see cref="string"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static string Get(string key, string defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{ApplicationProductName}_{key}"))
            {
                return EditorPrefs.GetString($"{ApplicationProductName}_{key}");
            }

            EditorPrefs.SetString($"{ApplicationProductName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="bool"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static bool Get(string key, bool defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{ApplicationProductName}_{key}"))
            {
                return EditorPrefs.GetBool($"{ApplicationProductName}_{key}");
            }

            EditorPrefs.SetBool($"{ApplicationProductName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="float"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static float Get(string key, float defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{ApplicationProductName}_{key}"))
            {
                return EditorPrefs.GetFloat($"{ApplicationProductName}_{key}");
            }

            EditorPrefs.SetFloat($"{ApplicationProductName}_{key}", defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Get the saved <see cref="int"/> from the <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static int Get(string key, int defaultValue)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(key));

            if (EditorPrefs.HasKey($"{ApplicationProductName}_{key}"))
            {
                return EditorPrefs.GetInt($"{ApplicationProductName}_{key}");
            }

            EditorPrefs.SetInt($"{ApplicationProductName}_{key}", defaultValue);
            return defaultValue;
        }
    }
}
