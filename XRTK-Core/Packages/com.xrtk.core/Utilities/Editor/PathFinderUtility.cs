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
        private const string CorePathFinder = "/Utilities/Editor/CorePathFinder.cs";
        private const string SdkPathFinder = "/Inspectors/SdkPathFinder.cs";

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

        private static Type ResolvedCorePathFinder
        {
            get
            {
                if (resolvedCorePathFinder == null)
                {
                    foreach (var type in GetAllPathFinders)
                    {
                        if (type.Name == nameof(CorePathFinder))
                        {
                            resolvedCorePathFinder = type;
                            break;
                        }
                    }
                }

                return resolvedCorePathFinder;
            }
        }

        private static Type resolvedCorePathFinder = null;

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
                if (string.IsNullOrEmpty(coreRelativeFolderPath) &&
                    ResolvedCorePathFinder != null)
                {
                    coreRelativeFolderPath =
                        AssetDatabase.GetAssetPath(
                                MonoScript.FromScriptableObject(
                                    ScriptableObject.CreateInstance(ResolvedCorePathFinder)))
                            .Replace(CorePathFinder, string.Empty);
                }

                return coreRelativeFolderPath;
            }
        }

        private static string coreRelativeFolderPath = string.Empty;

        private static Type ResolvedSdkPathFinder
        {
            get
            {
                if (resolvedSdkPathFinder == null)
                {
                    foreach (var type in GetAllPathFinders)
                    {
                        if (type.Name == nameof(SdkPathFinder))
                        {
                            resolvedSdkPathFinder = type;
                            break;
                        }
                    }
                }

                return resolvedSdkPathFinder;
            }
        }

        private static Type resolvedSdkPathFinder = null;


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
                if (string.IsNullOrEmpty(sdkRelativeFolderPath) &&
                    ResolvedSdkPathFinder != null)
                {
                    sdkRelativeFolderPath =
                        AssetDatabase.GetAssetPath(
                                MonoScript.FromScriptableObject(
                                    ScriptableObject.CreateInstance(ResolvedSdkPathFinder)))
                            .Replace(SdkPathFinder, string.Empty);
                }

                return sdkRelativeFolderPath;
            }
        }

        private static string sdkRelativeFolderPath = string.Empty;
    }
}