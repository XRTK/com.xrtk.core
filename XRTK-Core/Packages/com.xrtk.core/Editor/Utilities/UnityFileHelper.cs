// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace XRTK.Editor.Utilities
{
    public static class UnityFileHelper
    {
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

        public static List<string> GetUnityFiles(string assetsRootPath)
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