// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XRTK.Utilities.Editor
{
    public interface IPathFinder { }

    public static class PathFinderUtility
    {
        private const string CORE_PATH_FINDER = "/Utilities/Editor/CorePathFinder.cs";
        private const string SDK_PATH_FINDER = "/Editor/SdkPathFinder.cs";

        private static readonly Dictionary<string, string> ResolvedFinderCache = new Dictionary<string, string>();

        private static List<Type> GetAllPathFinders
        {
            get
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => typeof(IPathFinder).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    .OrderBy(type => type.Name)
                    .ToList();
            }
        }

        private static string ResolvePath(string finderPath)
        {
            if (!ResolvedFinderCache.TryGetValue(finderPath, out var resolvedPath))
            {
                foreach (var type in GetAllPathFinders)
                {
                    if (type.Name == Path.GetFileNameWithoutExtension(finderPath))
                    {
                        resolvedPath = AssetDatabase.GetAssetPath(
                            MonoScript.FromScriptableObject(
                                ScriptableObject.CreateInstance(type)))
                                    .Replace(finderPath, string.Empty);
                        ResolvedFinderCache.Add(finderPath, resolvedPath);
                        break;
                    }
                }
            }

            return resolvedPath;
        }

        /// <summary>
        /// The absolute folder path to the Mixed Reality Toolkit in your project.
        /// </summary>
        public static string XRTK_Core_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(coreAbsoluteFolderPath))
                {
                    coreAbsoluteFolderPath = Path.GetFullPath(XRTK_Core_RelativeFolderPath).Replace('\\', '/');
                }

                return coreAbsoluteFolderPath;
            }
        }

        private static string coreAbsoluteFolderPath = string.Empty;

        /// <summary>
        /// The relative folder path to the Mixed Reality Toolkit "com.xrtk.core" folder in relation to either the "Assets" or "Packages" folders.
        /// </summary>
        public static string XRTK_Core_RelativeFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(coreRelativeFolderPath))
                {
                    coreRelativeFolderPath = ResolvePath(CORE_PATH_FINDER);
                    Debug.Assert(!string.IsNullOrWhiteSpace(coreRelativeFolderPath));
                }

                return coreRelativeFolderPath;
            }
        }

        private static string coreRelativeFolderPath = string.Empty;

        /// <summary>
        /// The absolute folder path to the Mixed Reality Toolkit's SDK in your project.
        /// </summary>
        public static string XRTK_SDK_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sdkAbsoluteFolderPath))
                {
                    sdkAbsoluteFolderPath = Path.GetFullPath(XRTK_SDK_RelativeFolderPath).Replace('\\', '/');
                }

                return sdkAbsoluteFolderPath;
            }
        }

        private static string sdkAbsoluteFolderPath = string.Empty;

        /// <summary>
        /// The relative folder path to the Mixed Reality Toolkit "com.xrtk.sdk" folder in relation to either the "Assets" or "Packages" folders.
        /// </summary>
        public static string XRTK_SDK_RelativeFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(sdkRelativeFolderPath))
                {
                    sdkRelativeFolderPath = ResolvePath(SDK_PATH_FINDER);
                }

                return sdkRelativeFolderPath;
            }
        }

        private static string sdkRelativeFolderPath = string.Empty;
    }
}