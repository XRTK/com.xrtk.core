// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// <para>
    /// Used to regenerate guids for Unity assets.
    /// </para>
    /// <para>
    /// Based on https://gist.github.com/ZimM-LostPolygon/7e2f8a3e5a1be183ac19
    /// </para>
    /// </summary>
    public static class GuidRegenerator
    {
        /// <summary>
        /// Regenerate the guids for assets located in the <see cref="assetsRootPath"/>.
        /// </summary>
        /// <param name="assetsRootPath">The root directory to search for assets to regenerate guids for.</param>
        /// <param name="refreshAssetDatabase">Should <see cref="AssetDatabase.Refresh(ImportAssetOptions)"/> be called after finishing regeneration? (Default is true)</param>
        public static void RegenerateGuids(string assetsRootPath, bool refreshAssetDatabase = true)
        {
            RegenerateGuids(new List<string> { assetsRootPath }, refreshAssetDatabase);
        }

        /// <summary>
        /// Regenerate the guids for assets located in the <see cref="assetsRootPath"/>.
        /// </summary>
        /// <param name="assetsRootPath">The root directory to search for assets to regenerate guids for.</param>
        /// <param name="refreshAssetDatabase">Should <see cref="AssetDatabase.Refresh(ImportAssetOptions)"/> be called after finishing regeneration? (Default is true)</param>
        public static void RegenerateGuids(List<string> assetsRootPath, bool refreshAssetDatabase = true)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                RegenerateGuidsInternal(assetsRootPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();

                if (refreshAssetDatabase)
                {
                    EditorApplication.delayCall += AssetDatabase.Refresh;
                }
            }
        }

        private static void RegenerateGuidsInternal(List<string> assetsRootPath)
        {
            // Get list of working files
            var filesPaths = new List<string>();

            for (int i = 0; i < assetsRootPath.Count; i++)
            {
                filesPaths.AddRange(UnityFileHelper.GetUnityAssetsAtPath(assetsRootPath[i]));
            }

            // Create dictionary to hold old-to-new GUID map
            var guidOldToNewMap = new Dictionary<string, string>();
            var guidsInFileMap = new Dictionary<string, List<string>>();

            // We must only replace GUIDs for Resources present in the path.
            // Otherwise built-in resources (shader, meshes etc) get overwritten.
            var ownGuids = new HashSet<string>();

            // Traverse all files, remember which GUIDs are in which files and generate new GUIDs
            var counter = 0;

            foreach (var filePath in filesPaths)
            {
                EditorUtility.DisplayProgressBar("Gathering asset info...", filePath, counter / (float)filesPaths.Count);

                var isFirstGuid = true;
                var guids = GetGuids(File.ReadAllText(filePath));

                foreach (var oldGuid in guids)
                {
                    // First GUID in .meta file is always the GUID of the asset itself
                    if (isFirstGuid && Path.GetExtension(filePath) == ".meta")
                    {
                        ownGuids.Add(oldGuid);
                        isFirstGuid = false;
                    }

                    // Generate and save new GUID if we haven't added it before
                    if (!guidOldToNewMap.ContainsKey(oldGuid))
                    {
                        var newGuid = Guid.NewGuid().ToString("N");
                        guidOldToNewMap.Add(oldGuid, newGuid);
                    }

                    if (!guidsInFileMap.ContainsKey(filePath))
                    {
                        guidsInFileMap[filePath] = new List<string>();
                    }

                    if (!guidsInFileMap[filePath].Contains(oldGuid))
                    {
                        guidsInFileMap[filePath].Add(oldGuid);
                    }
                }

                counter++;
            }

            // Traverse the files again and replace the old GUIDs
            counter = -1;
            var guidsInFileMapKeysCount = guidsInFileMap.Keys.Count;

            foreach (var filePath in guidsInFileMap.Keys)
            {
                EditorUtility.DisplayProgressBar("Regenerating GUIDs...", filePath, counter / (float)guidsInFileMapKeysCount);
                counter++;

                var contents = File.ReadAllText(filePath);

                foreach (var oldGuid in guidsInFileMap[filePath])
                {
                    if (!ownGuids.Contains(oldGuid)) { continue; }

                    var newGuid = guidOldToNewMap[oldGuid];

                    if (string.IsNullOrEmpty(newGuid))
                    {
                        throw new NullReferenceException("newGuid == null");
                    }

                    contents = contents.Replace($"guid: {oldGuid}", $"guid: {newGuid}");
                }

                try
                {
                    var attributes = File.GetAttributes(filePath);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes ^= FileAttributes.ReadOnly;
                    }

                    File.SetAttributes(filePath, attributes);
                    File.WriteAllText(filePath, contents);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static IEnumerable<string> GetGuids(string text)
        {
            const string guidStart = "guid: ";
            const int guidLength = 32;
            var textLength = text.Length;
            var guidStartLength = guidStart.Length;
            var guids = new List<string>();
            var index = 0;

            while (index + guidStartLength + guidLength < textLength)
            {
                index = text.IndexOf(guidStart, index, StringComparison.Ordinal);

                if (index == -1)
                {
                    break;
                }

                index += guidStartLength;
                var guid = text.Substring(index, guidLength);
                index += guidLength;

                if (IsGuid(guid))
                {
                    guids.Add(guid);
                }
            }

            return guids;
        }

        private static bool IsGuid(string text) => text.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'));
    }
}