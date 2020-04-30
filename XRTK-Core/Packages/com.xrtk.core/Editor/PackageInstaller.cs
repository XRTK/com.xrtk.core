// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Editor
{
    public static class PackageInstaller
    {
        private const string META_SUFFIX = ".meta";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static bool TryInstallProfiles(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                return true;
            }

            var profilePaths = Directory.EnumerateFiles(Path.GetFullPath(sourcePath), "*.asset", SearchOption.AllDirectories);
            var anyFail = false;

            foreach (var profilePath in profilePaths)
            {
                try
                {
                    CopyAsset(sourcePath, profilePath, destinationPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    anyFail = true;
                }
            }

            EditorApplication.delayCall += AssetDatabase.Refresh;

            return !anyFail;
        }

        private static void CopyAsset(this string rootPath, string sourceAssetPath, string destinationPath)
        {
            sourceAssetPath = sourceAssetPath.ToForwardSlashes();
            destinationPath = $"{destinationPath}{sourceAssetPath.Replace(Path.GetFullPath(rootPath), string.Empty)}".ToForwardSlashes();
            destinationPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, destinationPath);

            Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);

            File.Copy(sourceAssetPath, destinationPath);
            File.Copy($"{sourceAssetPath}{META_SUFFIX}", $"{destinationPath}{META_SUFFIX}");
        }
    }
}
