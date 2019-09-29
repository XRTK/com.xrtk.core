// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XRTK.Inspectors.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ScriptableObject"/>s
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Creates, saves, and then optionally selects a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, bool ping = true)
        {
            return CreateAsset(scriptableObject, null, ping);
        }

        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, string path, bool ping = true)
        {
            return CreateAsset(scriptableObject, path, null, ping);
        }

        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static ScriptableObject CreateAsset(this ScriptableObject scriptableObject, string path, string fileName, bool ping)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{scriptableObject.GetType().Name}" : fileName;

            if (string.IsNullOrEmpty(path))
            {
                path = MixedRealityPreferences.ProfileGenerationPath;
            }

            if (Path.GetExtension(path) != string.Empty)
            {
                var subtractedPath = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal));
                path = path.Replace(subtractedPath, string.Empty);
            }

            if (!Directory.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(Path.GetFullPath(path));
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh();
            }

            if (ping)
            {
                EditorUtility.FocusProjectWindow();

                EditorApplication.delayCall += () =>
                {
                    EditorGUIUtility.PingObject(scriptableObject);
                    Selection.activeObject = scriptableObject;
                };
            }

            return scriptableObject;
        }

        /// <summary>
        /// Gets all the scriptable object instances in the project.
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="ScriptableObject"/> you're wanting to find instances of.</typeparam>
        /// <returns>An Array of instances for the type.</returns>
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            // FindAssets uses tags check documentation for more info
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var instances = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                instances[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            return instances;
        }
    }
}