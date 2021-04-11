// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            EditorApplication.projectChanged += OnProjectChanged;
            RunSync(Application.isBatchMode);
        }

        private static bool hasProjectUpdated = false;

        private static void OnProjectChanged()
        {
            if (hasProjectUpdated) { return; }
            hasProjectUpdated = true;

            void VerifyLinks(object state)
            {
                while (hasProjectUpdated)
                {
                    Task.Delay(2500);

                    var anyInvalid = false;

                    foreach (var link in Settings.SymbolicLinks)
                    {
                        var path = $"{ProjectRoot}{link.TargetRelativePath}";
                        var attributes = File.GetAttributes(path);
                        var isValid = VerifySymbolicLink(path);

                        if (DebugEnabled)
                        {
                            Debug.Log($"Checking {path}\nIsValid?{isValid} | {attributes}");
                        }

                        if (!isValid)
                        {
                            DeleteSymbolicLink(path);
                            anyInvalid = true;
                        }
                    }

                    if (anyInvalid)
                    {
                        EditorApplication.delayCall += () => RunSync(true);
                    }
                }
            }

            ThreadPool.QueueUserWorkItem(VerifyLinks);
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

            bool needsUpdate = false;
            var symbolicLinks = new List<SymbolicLink>(Settings.SymbolicLinks);

            foreach (var link in symbolicLinks)
            {
                if (string.IsNullOrEmpty(link.SourceRelativePath)) { continue; }

                var targetAbsolutePath = $"{ProjectRoot}{link.TargetRelativePath}";
                var sourceAbsolutePath = $"{ProjectRoot}{link.SourceRelativePath}";

                if (VerifySymbolicLink(targetAbsolutePath))
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Is Link Active? {link.IsActive} : {link.TargetRelativePath}");
                    }

                    // If we already have the directory in our project, then skip.
                    if (link.IsActive) { continue; }

                    // Check to see if there are any directories that don't belong and remove them.
                    needsUpdate |= DisableLink(link.TargetRelativePath);
                    continue;
                }

                if (!link.IsActive) { continue; }

                if (Directory.Exists(sourceAbsolutePath))
                {
                    if (link.IsActive)
                    {
                        needsUpdate |= AddLink(sourceAbsolutePath, targetAbsolutePath);
                    }

                    continue;
                }

                // Make sure all of our Submodules are initialized and updated.
                // if they didn't get updated then we probably have some pending changes so we will skip.
                if (!GitUtilities.UpdateSubmodules()) { continue; }

                if (Directory.Exists(sourceAbsolutePath))
                {
                    if (link.IsActive)
                    {
                        needsUpdate |= AddLink(sourceAbsolutePath, targetAbsolutePath);
                    }

                    continue;
                }

                Debug.LogError($"Unable to find symbolic link source path: {sourceAbsolutePath}");
                needsUpdate |= RemoveLink(link.SourceRelativePath, link.TargetRelativePath);
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
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }

            EditorApplication.UnlockReloadAssemblies();
            IsSyncing = false;
        }

        /// <summary>
        /// Adds or Enables a symbolic link in the settings.
        /// </summary>
        /// <param name="sourceAbsolutePath"></param>
        /// <param name="targetAbsolutePath"></param>
        internal static bool AddLink(string sourceAbsolutePath, string targetAbsolutePath)
        {
            if (string.IsNullOrEmpty(sourceAbsolutePath) || string.IsNullOrEmpty(targetAbsolutePath))
            {
                Debug.LogError("Unable to write to the symbolic link settings with null or empty parameters.");
                return false;
            }

            targetAbsolutePath = AddSubfolderPathToTarget(sourceAbsolutePath, targetAbsolutePath);

            // Fix the directory character separator characters.
            var sourceRelativePath = sourceAbsolutePath.ToBackSlashes();
            var targetRelativePath = targetAbsolutePath.ToBackSlashes();

            // Strip URI for relative path.
            sourceRelativePath = sourceRelativePath.Replace(ProjectRoot, string.Empty);
            targetRelativePath = targetRelativePath.Replace(ProjectRoot, string.Empty);

            if (!CreateSymbolicLink(sourceAbsolutePath, targetAbsolutePath))
            {
                return false;
            }

            SymbolicLink symbolicLink;

            // Check if the symbolic link is already registered.
            if (!Settings.SymbolicLinks.Exists(link => link.TargetRelativePath == targetRelativePath))
            {
                symbolicLink = new SymbolicLink
                {
                    SourceRelativePath = sourceRelativePath,
                    TargetRelativePath = targetRelativePath,
                    IsActive = true
                };

                Settings.SymbolicLinks.Add(symbolicLink);
            }
            else // Overwrite the registered symbolic link
            {
                symbolicLink = Settings.SymbolicLinks.Find(link => link.TargetRelativePath == targetRelativePath);

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
        /// <param name="targetRelativePath"></param>
        internal static bool DisableLink(string targetRelativePath)
        {
            var symbolicLink = Settings.SymbolicLinks.Find(link => link.TargetRelativePath == targetRelativePath);

            if (symbolicLink != null)
            {
                if (DeleteSymbolicLink($"{ProjectRoot}{targetRelativePath}"))
                {
                    Debug.Log($"Disabled symbolic link to \"{symbolicLink.SourceRelativePath}\" in project.");
                    symbolicLink.IsActive = false;
                    return true;
                }
            }
            else
            {
                Debug.LogError($"Unable to find the specified symbolic link at: {targetRelativePath}");
            }

            return false;
        }

        /// <summary>
        /// Remove a symbolic link in the settings.
        /// </summary>
        /// <param name="sourceRelativePath"></param>
        /// <param name="targetRelativePath"></param>
        internal static bool RemoveLink(string sourceRelativePath, string targetRelativePath)
        {
            if (!DisableLink(targetRelativePath))
            {
                return false;
            }

            var symbolicLink = Settings.SymbolicLinks.Find(link => link.SourceRelativePath == sourceRelativePath);

            if (symbolicLink != null)
            {
                Debug.Log($"Removed symbolic link to \"{symbolicLink.SourceRelativePath}\" from project.");
                Settings.SymbolicLinks.Remove(symbolicLink);
                return true;
            }

            Debug.LogError($"Unable to find the specified symbolic link source at: {sourceRelativePath}");
            return false;
        }

        private static void OnProjectWindowItemGui(string guid, Rect rect)
        {
            try
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrEmpty(path) &&
                    IsSymbolicPath(path) &&
                    VerifySymbolicLink(path))
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
                if (!Directory.Exists(path))
                {
                    if (File.Exists(path))
                    {
                        return false;
                    }

                    if (DebugEnabled)
                    {
                        Debug.LogError($"Invalid path: {path}");
                    }

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

        private static bool VerifySymbolicLink(string targetAbsolutePath)
        {
            try
            {
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

                var isValid = IsSymbolicPath(targetAbsolutePath);

                if (!isValid)
                {
                    if (DebugEnabled)
                    {
                        Debug.Log($"Removing invalid link for {targetAbsolutePath}");
                    }

                    DeleteSymbolicLink(targetAbsolutePath);
                }
                else
                {
                    var tempFile = $"{targetAbsolutePath}/symlink_temp.txt";

                    try
                    {
                        if (!File.Exists(tempFile))
                        {
                            var stream = File.CreateText(tempFile);
                            stream.Dispose();
                            stream.Close();
                        }

                        if (File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                        }

                        if (File.Exists($"{tempFile}.meta"))
                        {
                            File.Delete($"{tempFile}.meta");
                        }
                    }
                    catch (Exception e)
                    {
                        switch (e)
                        {
                            case AccessViolationException _:
                            case IOException _:
                                return true;
                            default:
                                return false;
                        }
                    }
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

        private static bool CreateSymbolicLink(string sourceAbsolutePath, string targetAbsolutePath)
        {
            if (string.IsNullOrEmpty(targetAbsolutePath) || string.IsNullOrEmpty(sourceAbsolutePath))
            {
                Debug.LogError("Unable to create symbolic link with null or empty args");
                return false;
            }

            if (!sourceAbsolutePath.Contains(ProjectRoot))
            {
                Debug.LogError($"The symbolic link you're importing needs to be in your project's git repository.\n{sourceAbsolutePath}\n{ProjectRoot}");
                return false;
            }

            targetAbsolutePath = AddSubfolderPathToTarget(sourceAbsolutePath, targetAbsolutePath);
            var ignorePath = targetAbsolutePath;

            if (ignorePath.Contains("Assets/ThirdParty"))
            {
                // Check if we need to create the ThirdParty folder.
                if (!Directory.Exists($"{Application.dataPath}/ThirdParty"))
                {
                    Directory.CreateDirectory($"{Application.dataPath}/ThirdParty");
                }

                // Initialize gitIgnore if needed.
                GitUtilities.WritePathToGitIgnore("Assets/ThirdParty");
                GitUtilities.WritePathToGitIgnore("Assets/ThirdParty.meta");
            }
            else // If we're not importing into the ThirdParty Directory, we should add the path to the git ignore file.
            {
                GitUtilities.WritePathToGitIgnore($"{ignorePath}");

                // If the imported path target resides in the Assets folder, we should also ignore the path .meta file.
                if (ignorePath.Contains("Assets"))
                {
                    GitUtilities.WritePathToGitIgnore($"{ignorePath}.meta");
                }
            }

            // Check if the directory exists to ensure the parent directory paths are also setup
            // then delete the target directory as the mklink command will create it for us.
            if (!Directory.Exists(targetAbsolutePath))
            {
                Directory.CreateDirectory(targetAbsolutePath);
                Directory.Delete(targetAbsolutePath);
            }

#if UNITY_EDITOR_WIN
            // --------------------> mklink /D "C:\Link To Folder" "C:\Users\Name\Original Folder"
            if (!new Process().Run($"mklink /D \"{targetAbsolutePath}\" \"{sourceAbsolutePath}\"", out var error))
            {
                Debug.LogError(error);
                return false;
            }
#else
            // --------------------> ln -s /path/to/original /path/to/symlink
            if (!new Process().Run($"ln -s \"{sourceAbsolutePath}\" \"{targetAbsolutePath}\"", out var error))
            {
                Debug.LogError(error);
                return false;
            }
#endif

            Debug.Log($"Successfully created symbolic link to {sourceAbsolutePath}");

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            return true;
        }

        private static bool DeleteSymbolicLink(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Unable to find the linked folder at: {path}");
                return false;
            }

            bool success;

#if UNITY_EDITOR_WIN
            success = new Process().Run($"rmdir /q \"{path}\"", out _);
#else
            success = new Process().Run($"rm \"{path}\"", out _);
#endif

            if (success)
            {
                if (File.Exists($"{path}.meta"))
                {
                    File.Delete($"{path}.meta");
                }

                if (!EditorApplication.isUpdating)
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
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
