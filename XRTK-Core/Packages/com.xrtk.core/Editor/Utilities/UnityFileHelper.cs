// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    public static class UnityFileHelper
    {
        /// <summary>
        /// Managed list of native Unity asset file extensions
        /// </summary>
        private static readonly string[] UnityFileExtensions = {
            "*.meta",
            "*.mat",
            "*.anim",
            "*.prefab",
            "*.unity",
            "*.asset",
            "*.guiskin",
            "*.fontsettings",
            "*.controller",
            "*.json"
        };

        /// <summary>
        /// Utility to return all the unity recognized files, including meta files within a specific path
        /// </summary>
        /// <param name="assetsRootPath">Root folder from which to search from</param>
        public static List<string> GetUnityAssetsAtPath(string assetsRootPath)
        {
            // Get list of working files
            var filesPaths = new List<string>();

            assetsRootPath = Path.GetFullPath(assetsRootPath);

            foreach (var extension in UnityFileExtensions)
            {
                try
                {
                    filesPaths.AddRange(Directory.GetFiles(assetsRootPath, extension, SearchOption.AllDirectories));
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e}\n{assetsRootPath}");
                }
            }

            return filesPaths;
        }
    }
}
