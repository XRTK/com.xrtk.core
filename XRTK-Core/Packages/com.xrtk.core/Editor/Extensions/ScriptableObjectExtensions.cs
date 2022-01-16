// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Extensions
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
        public static T CreateAsset<T>(this T scriptableObject, bool ping = true) where T : ScriptableObject
        {
            return CreateAsset(scriptableObject, null, ping);
        }

        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static T CreateAsset<T>(this T scriptableObject, string path, bool ping = true) where T : ScriptableObject
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
        /// <param name="unique">Is the new asset unique, or can we make copies?</param>
        public static T CreateAsset<T>(this T scriptableObject, string path, string fileName, bool ping, bool unique = true) where T : ScriptableObject
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{scriptableObject.GetType().Name}" : fileName;

            name = name.Replace(" ", string.Empty);

            if (string.IsNullOrWhiteSpace(path))
            {
                path = MixedRealityPreferences.ProfileGenerationPath;
            }

            path = path.Replace(".asset", string.Empty);

            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
            {
                var subtractedPath = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));
                path = path.Replace(subtractedPath, string.Empty);
            }

            path = path.Replace($"{Directory.GetParent(Application.dataPath).FullName}{Path.DirectorySeparatorChar}", string.Empty);

            if (!Directory.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(Path.GetFullPath(path));
            }

            path = $"{path}/{name}.asset";

            if (unique)
            {
                AssetDatabase.GenerateUniqueAssetPath(path);
            }

            if (File.Exists(Path.GetFullPath(path)))
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            AssetDatabase.CreateAsset(scriptableObject, path);
            AssetDatabase.SaveAssets();

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh();
            }

            scriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);

            if (ping)
            {
                EditorApplication.delayCall += () =>
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(scriptableObject);
                    Selection.activeObject = scriptableObject;
                };
            }

            Debug.Assert(scriptableObject != null);

            return scriptableObject;
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static T GetOrCreateAsset<T>(this T scriptableObject, bool ping = true) where T : ScriptableObject
        {
            return GetOrCreateAsset(scriptableObject, null, ping);
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static T GetOrCreateAsset<T>(this T scriptableObject, string path, bool ping = true) where T : ScriptableObject
        {
            return GetOrCreateAsset(scriptableObject, path, null, ping);
        }

        /// <summary>
        /// Attempts to find the asset associated to the instance of the <see cref="ScriptableObject"/>, if none is found a new asset is created.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want get or create an asset file for.</param>
        /// <param name="path">Optional path for the new asset.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        /// <param name="ping">The new asset should be selected and opened in the inspector.</param>
        public static T GetOrCreateAsset<T>(this T scriptableObject, string path, string fileName, bool ping) where T : ScriptableObject
        {
            return !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scriptableObject, out var guid, out long _)
                ? scriptableObject.CreateAsset(path, fileName, ping, false)
                : AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
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
