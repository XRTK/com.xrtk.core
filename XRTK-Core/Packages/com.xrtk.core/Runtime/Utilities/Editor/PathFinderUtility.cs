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
    /// <summary>
    /// Interface to implement on a <see cref="ScriptableObject"/> to make it easier to find relative/absolute folder paths using the <see cref="PathFinderUtility"/>.
    /// </summary>
    /// <remarks>
    /// Required to be a standalone class in a separate file or else <see cref="MonoScript.FromScriptableObject"/> returns an empty string path.
    /// </remarks>
    public interface IPathFinder
    {
        /// <summary>
        /// The relative path to this <see cref="IPathFinder"/> class from either the Assets or Packages folder.
        /// </summary>
        string Location { get; }
    }

    public static class PathFinderUtility
    {
        private const string CORE_PATH_FINDER = "/Runtime/Utilities/Editor/CorePathFinder.cs";
        private const string SDK_PATH_FINDER = "/Editor/SdkPathFinder.cs";

        private static readonly Dictionary<Type, string> PathFinderCache = new Dictionary<Type, string>();
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
        /// Resolves the path to the provided <see cref="IPathFinder"/>.<see cref="T:Type"/>
        /// </summary>
        /// <typeparam name="T"><see cref="IPathFinder"/> constraint.</typeparam>
        /// <param name="pathFinderType">The <see cref="T:Type"/> of <see cref="IPathFinder"/> to resolve the path for.</param>
        /// <returns>If found, the relative path to the root folder this <see cref="IPathFinder"/> references.</returns>
        public static string ResolvePath<T>(Type pathFinderType) where T : IPathFinder
        {
            if (pathFinderType is null)
            {
                Debug.LogError($"{nameof(pathFinderType)} is null!");
                return null;
            }

            if (!typeof(T).IsAssignableFrom(pathFinderType))
            {
                Debug.LogError($"{pathFinderType.Name} must implement {nameof(IPathFinder)}");
                return null;
            }

            if (!typeof(ScriptableObject).IsAssignableFrom(pathFinderType))
            {
                Debug.LogError($"{pathFinderType.Name} must derive from {nameof(ScriptableObject)}");
                return null;
            }

            if (!PathFinderCache.TryGetValue(pathFinderType, out var resolvedPath))
            {
                var pathFinder = ScriptableObject.CreateInstance(pathFinderType) as IPathFinder;
                Debug.Assert(pathFinder != null, $"{nameof(pathFinder)} != null");
                resolvedPath = AssetDatabase.GetAssetPath(
                    MonoScript.FromScriptableObject((ScriptableObject)pathFinder))
                        .Replace(pathFinder.Location, string.Empty);
                PathFinderCache.Add(pathFinderType, resolvedPath);
            }

            return resolvedPath;
        }

        #region Core Paths

        /// <summary>
        /// The absolute folder path to the Mixed Reality Toolkit in your project.
        /// </summary>
        public static string XRTK_Core_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(coreAbsoluteFolderPath))
                {
                    coreAbsoluteFolderPath = Path.GetFullPath(XRTK_Core_RelativeFolderPath);
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

        #endregion Core Paths

        #region SDK Paths

        /// <summary>
        /// The absolute folder path to the Mixed Reality Toolkit's SDK in your project.
        /// </summary>
        public static string XRTK_SDK_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sdkAbsoluteFolderPath))
                {
                    sdkAbsoluteFolderPath = Path.GetFullPath(XRTK_SDK_RelativeFolderPath);
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

        #endregion SDK Paths
    }
}