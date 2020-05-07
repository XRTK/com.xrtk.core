// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

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
        };

        /// <summary>
        /// Utility to return all the unity recognized files, including meta files within a specific path
        /// </summary>
        /// <param name="assetsRootPath">Root folder from which to search from</param>
        /// <returns></returns>
        public static List<string> GetUnityAssetsAtPath(string assetsRootPath)
        {
            // Get list of working files
            var filesPaths = new List<string>();

            foreach (var extension in UnityFileExtensions)
            {
                filesPaths.AddRange(Directory.GetFiles(assetsRootPath, extension, SearchOption.AllDirectories));
            }

            return filesPaths;
        }
    }
}
