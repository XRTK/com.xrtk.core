// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.Utilities.SymbolicLinks
{
    [InitializeOnLoad]
    public static class SymbolicLinker
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        static SymbolicLinker()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGui;
            RunSync(Application.isBatchMode);
        }

        private const string LINK_ICON_TEXT = "<=link=>";

        // Style used to draw the symlink indicator in the project view.
        private static GUIStyle SymlinkMarkerStyle => symbolicLinkMarkerStyle ?? (symbolicLinkMarkerStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = new Color(.2f, .8f, .2f, .8f) },
            alignment = TextAnchor.MiddleRight
        });

        private static GUIStyle symbolicLinkMarkerStyle;

        internal static string ProjectRoot => GitUtilities.RepositoryRootDir;

        /// <summary>
        /// Is the sync task running?
        /// </summary>
        public static bool IsSyncing { get; private set; }

        /// <summary>
        /// Debug the symbolic linker utility.
        /// </summary>
        private static bool DebugEnabled
        {
            get => MixedRealityPreferences.DebugSymbolicInfo;
            set => MixedRealityPreferences.DebugSymbolicInfo = value;
        }

        /// <summary>
        /// The current settings for the symbolic links.
        /// </summary>
        public static SymbolicLinkSettings Settings
        {
            get
            {
                if (settings == null &&
                    !string.IsNullOrEmpty(MixedRealityPreferences.SymbolicLinkSettingsPath))
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Loading symlink settings from :{MixedRealityPreferences.SymbolicLinkSettingsPath}");
                    }

                    settings = AssetDatabase.LoadAssetAtPath<SymbolicLinkSettings>(MixedRealityPreferences.SymbolicLinkSettingsPath);
                }

                return settings;
            }
            internal set => settings = value;
        }

        private static SymbolicLinkSettings settings;

        [DidReloadScripts]
        private static void OnScriptsReloaded() => RunSync();

        /// <summary>
        /// Synchronizes the project with any symbolic links defined in the settings.
        /// </summary>
        /// <param name="forceUpdate">Bypass the auto load check and force the packages to be updated, even if they're up to date.</param>
        public static void RunSync(bool forceUpdate = false)
        {
            if (DebugEnabled)
            {
                Debug.Log($"{nameof(RunSync)}({nameof(forceUpdate)} = {forceUpdate})");
            }

            if (IsSyncing || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (!MixedRealityPreferences.AutoLoadSymbolicLinks && !forceUpdate)
            {
                return;
            }

            if (Settings == null)
            {
                if (EditorApplication.isUpdating)
                {
                    EditorApplication.delayCall += () => RunSync(forceUpdate);
                    return;
                }
            }

            if (Settings == null)
            {
                if (!string.IsNullOrEmpty(MixedRealityPreferences.SymbolicLinkSettingsPath))
                {
                    Debug.LogWarning("Symbolic link settings not found! Auto load links has been turned off.\nYou can enable this again in the xrtk preferences.");
                }

                MixedRealityPreferences.AutoLoadSymbolicLinks = false;
                return;
            }

            IsSyncing = true;
            AssetDatabase.ReleaseCachedFileHandles();
            EditorApplication.LockReloadAssemblies();

            if (DebugEnabled)
            {
                Debug.Log("Verifying project's symbolic links...");
            }

            var needsUpdate = false;
            var symbolicLinks = new List<SymbolicLink>(Settings.SymbolicLinks);

            foreach (var link in symbolicLinks)
            {
                if (string.IsNullOrEmpty(link.SourceRelativePath)) { continue; }

                if (link.Validate())
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Is Link Active? {link.IsActive} : {link.TargetRelativePath}");
                    }

                    // If we already have the directory in our project, then skip.
                    if (link.IsActive) { continue; }

                    // Check to see if there are any directories that don't belong and remove them.
                    needsUpdate |= link.Disable();
                    continue;
                }

                if (!link.IsActive) { continue; }

                if (Directory.Exists(link.SourceAbsolutePath))
                {
                    if (link.IsActive)
                    {
                        needsUpdate |= link.Add();
                    }

                    continue;
                }

                // Make sure all of our Submodules are initialized and updated.
                // if they didn't get updated then we probably have some pending changes so we will skip.
                if (!GitUtilities.UpdateSubmodules()) { continue; }

                if (Directory.Exists(link.SourceAbsolutePath))
                {
                    if (link.IsActive)
                    {
                        needsUpdate |= link.Add();
                    }

                    continue;
                }

                Debug.LogError($"Unable to find symbolic link source path: {link.SourceAbsolutePath}");
                needsUpdate |= link.Remove();
            }

            if (DebugEnabled)
            {
                Debug.Log("Project symbolic links verified.");
            }

            if (needsUpdate)
            {
                EditorUtility.SetDirty(Settings);
                AssetDatabase.SaveAssets();

                if (!EditorApplication.isUpdating)
                {
                    EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }

            EditorApplication.UnlockReloadAssemblies();
            IsSyncing = false;
        }

        internal static bool Add(this SymbolicLink newLink)
        {
            if (string.IsNullOrEmpty(newLink.SourceAbsolutePath) || string.IsNullOrEmpty(newLink.TargetAbsolutePath))
            {
                Debug.LogError("Unable to write to the symbolic link settings with null or empty parameters.");
                return false;
            }

            newLink.TargetAbsolutePath = AddSubfolderPathToTarget(newLink.SourceAbsolutePath, newLink.TargetAbsolutePath);

            if (!CreateSymbolicLink(newLink))
            {
                return false;
            }

            SymbolicLink symbolicLink;

            // Check if the symbolic link is already registered.
            if (!Settings.SymbolicLinks.Exists(link => link.TargetRelativePath == newLink.TargetRelativePath))
            {
                symbolicLink = new SymbolicLink(newLink.SourceRelativePath, newLink.TargetRelativePath)
                {
                    IsActive = true
                };

                Settings.SymbolicLinks.Add(symbolicLink);
            }
            else // Overwrite the registered symbolic link
            {
                symbolicLink = Settings.SymbolicLinks.Find(link => link.TargetRelativePath == newLink.TargetRelativePath);

                if (symbolicLink != null)
                {
                    symbolicLink.IsActive = true;
                }
            }

            return true;
        }

        /// <summary>
        /// Disables a symbolic link in the settings.
        /// </summary>
        internal static bool Disable(this SymbolicLink oldLink)
        {
            var symbolicLink = Settings.SymbolicLinks.Find(link => link.TargetRelativePath == oldLink.TargetRelativePath);

            if (symbolicLink != null)
            {
                if (DeleteSymbolicPath(symbolicLink.TargetAbsolutePath))
                {
                    Debug.Log($"Disabled symbolic link to \"{symbolicLink.SourceRelativePath}\" in project.");
                    symbolicLink.IsActive = false;
                    return true;
                }
            }
            else
            {
                Debug.LogError($"Unable to find the specified symbolic link at: {oldLink.TargetRelativePath}");
            }

            return false;
        }

        /// <summary>
        /// Remove a symbolic link in the settings.
        /// </summary>
        internal static bool Remove(this SymbolicLink oldLink)
        {
            if (!oldLink.Disable())
            {
                return false;
            }

            var symbolicLink = Settings.SymbolicLinks.Find(link => link.SourceRelativePath == oldLink.SourceRelativePath);

            if (symbolicLink != null)
            {
                Debug.Log($"Removed symbolic link to \"{symbolicLink.SourceRelativePath}\" from project.");
                Settings.SymbolicLinks.Remove(symbolicLink);
                return true;
            }

            Debug.LogError($"Unable to find the specified symbolic link source at: {oldLink.SourceRelativePath}");
            return false;
        }

        private static void OnProjectWindowItemGui(string guid, Rect rect)
        {
            try
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrEmpty(path) &&
                    ValidateSymbolicPath(Path.GetFullPath(path).ToBackSlashes()))
                {
                    GUI.Label(rect, LINK_ICON_TEXT, SymlinkMarkerStyle);
                }
            }
            catch (Exception e)
            {
                if (DebugEnabled)
                {
                    Debug.LogError(e);
                }
            }
        }

        private static bool IsSymbolicPath(string path)
        {
            try
            {
                if (!Directory.Exists(path) || File.Exists(path))
                {
                    return false;
                }

                var attributes = File.GetAttributes(path);

                if (attributes == (FileAttributes)(-1))
                {
                    Debug.LogError($"Invalid file attributes found for {path}!");

                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path);
                    }

                    return false;
                }

                if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    return false;
                }

                return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        internal static bool Validate(this SymbolicLink link)
        {
            return ValidateSymbolicPath(link.TargetAbsolutePath, link.SourceAbsolutePath);
        }

        private static bool ValidateSymbolicPath(string targetAbsolutePath, string sourceAbsolutePath = null)
        {
            try
            {
                if (!IsSymbolicPath(targetAbsolutePath))
                {
                    return false;
                }

                if (DebugEnabled)
                {
                    Debug.Log($"Attempting to validate {targetAbsolutePath}");
                }

                if (!Directory.Exists(targetAbsolutePath))
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Validated disabled link: {targetAbsolutePath}");
                    }

                    return false;
                }

                if (string.IsNullOrWhiteSpace(sourceAbsolutePath) && Settings != null)
                {
                    foreach (var link in Settings.SymbolicLinks)
                    {
                        if (targetAbsolutePath.Contains(link.TargetRelativePath))
                        {
                            sourceAbsolutePath = $"{ProjectRoot}{link.SourceRelativePath}";
                            break;
                        }
                    }
                }

                var isValid = !string.IsNullOrWhiteSpace(sourceAbsolutePath) &&
                               Directory.Exists(sourceAbsolutePath);

                if (!isValid)
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Removing invalid link for {targetAbsolutePath}");
                    }

                    DeleteSymbolicPath(targetAbsolutePath);
                }

                return isValid && Directory.Exists(targetAbsolutePath);
            }
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        private static bool CreateSymbolicLink(SymbolicLink link)
        {
            if (string.IsNullOrEmpty(link.SourceAbsolutePath) || string.IsNullOrEmpty(link.TargetAbsolutePath))
            {
                Debug.LogError("Unable to create symbolic link with null or empty args");
                return false;
            }

            if (!link.SourceAbsolutePath.Contains(ProjectRoot))
            {
                Debug.LogError($"The symbolic link you're importing needs to be in your project's git repository.\n{link.SourceAbsolutePath}\n{ProjectRoot}");
                return false;
            }

            // If pulling from git we may have "fake" symlinks that need to be cleaned up.
            if (File.Exists(link.TargetAbsolutePath))
            {
                File.Delete(link.TargetAbsolutePath);
            }

            // Check if the directory exists to ensure the parent directory paths are also setup
            // then delete the target directory as the mklink command will create it for us.
            if (!Directory.Exists(link.TargetAbsolutePath))
            {
                Directory.CreateDirectory(link.TargetAbsolutePath);
            }

            if (Directory.Exists(link.TargetAbsolutePath))
            {
                Directory.Delete(link.TargetAbsolutePath);
            }

            if (!link.TargetAbsolutePath.TryMakeRelativePath(link.SourceAbsolutePath, out var relativePath))
            {
                return false;
            }

            var symlinkCommand =
#if UNITY_EDITOR_WIN
                //mklink /D "C:\path\to\symlink" "..\relative\path\to\source"
                $"mklink /D \"{link.TargetAbsolutePath}\" \"{relativePath}\"";
#else
                //ln -s "../relative/path/to/source" "/path/to/symlink"
                $"ln -s \"{relativePath}\" \"{link.TargetAbsolutePath}\"";
#endif

            if (!new Process().Run(symlinkCommand, out var error))
            {
                Debug.LogError($"{error}\n{link.TargetRelativePath} <=> {link.SourceAbsolutePath}");
                return false;
            }

            Debug.Log($"Successfully created symbolic link to {link.SourceAbsolutePath}");

            if (!EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            return true;
        }

        private static bool DeleteSymbolicPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Unable to find the linked folder at: {path}");
                return false;
            }

            var symlinkCommand =
#if UNITY_EDITOR_WIN
                $"rmdir /q \"{path}\"";
#else
                $"rm \"{path}\"";
#endif

            var success = new Process().Run(symlinkCommand, out _);

            if (success)
            {
                if (File.Exists($"{path}.meta"))
                {
                    File.Delete($"{path}.meta");
                }

                if (!EditorApplication.isUpdating)
                {
                    EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }

            return true;
        }

        private static string AddSubfolderPathToTarget(string sourcePath, string targetPath)
        {
            var subFolder = sourcePath.Substring(sourcePath.LastIndexOf("/", StringComparison.Ordinal) + 1).Replace("~", string.Empty);

            // Check to see if our target path already has the sub folder reference.
            if (!targetPath.Substring(targetPath.LastIndexOf("/", StringComparison.Ordinal) + 1).Equals(subFolder))
            {
                targetPath = $"{targetPath}/{subFolder}";
            }

            return targetPath;
        }
    }
}
