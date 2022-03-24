﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XRTK.Editor.Utilities.SymbolicLinks;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.Utilities
{
    public static class GitUtilities
    {
        private static bool? isGitInstalled = null;

        public static bool IsGitInstalled
        {
            get
            {
                if (!isGitInstalled.HasValue)
                {
                    isGitInstalled = new Process().Run("git --version", out var message) && !message.Contains("'git' is not recognized");
                }

                if (!isGitInstalled.Value)
                {
                    Debug.LogWarning("Git installation not found on this machine! Please validate your git installation or environment variables.");
                }

                return isGitInstalled.Value;
            }
        }

        /// <summary>
        /// Returns the root directory of this repository.
        /// </summary>
        /// <remarks>
        /// If Git is not installed or cannot be found, then the project's root directory is returned.
        /// </remarks>
        public static string RepositoryRootDir
        {
            get
            {
                if (!string.IsNullOrEmpty(projectRootDir)) { return projectRootDir; }

                if (!IsGitInstalled)
                {
                    projectRootDir = Directory.GetParent(Directory.GetParent(EditorPreferences.ApplicationDataPath).FullName).FullName;
                }
                else
                {
                    if (new Process().Run($@"cd ""{Application.dataPath}"" && git rev-parse --show-toplevel", out var rootDir))
                    {
                        projectRootDir = rootDir.ForwardSlashes().Replace("\n", string.Empty);
                    }
                }

                return projectRootDir;
            }
        }

        private static string projectRootDir;

        internal static bool HasSubmodules => IsGitInstalled && File.Exists($"{RepositoryRootDir}/.gitmodules");

        [MenuItem("Assets/Submodules/Force update all submodules", true, 23)]
        public static bool ForceUpdateSubmodulesValidation() => HasSubmodules;

        [MenuItem("Assets/Submodules/Force update all submodules", false, 23)]
        public static void ForceUpdateSubmodules()
        {
            UpdateSubmodules();
            SymbolicLinker.RunSync(true);
        }

        /// <summary>
        /// Writes the specified path to the projects git ignore file located at the root of the repository.
        /// </summary>
        /// <param name="ignoredPath"></param>
        public static void WritePathToGitIgnore(string ignoredPath)
        {
            if (!IsGitInstalled)
            {
                return;
            }

            if (string.IsNullOrEmpty(ignoredPath))
            {
                Debug.LogError("You cannot pass null or empty parameter.");
                return;
            }

            var rootDir = RepositoryRootDir;
            ignoredPath = ignoredPath.Replace($"{rootDir.ForwardSlashes()}/", string.Empty);
            var directory = ignoredPath.Replace(Path.GetFileName(ignoredPath), Path.GetFileNameWithoutExtension(ignoredPath));
            directory = directory.Substring(0, directory.LastIndexOf("/", StringComparison.Ordinal));
            var gitIgnoreFilePath = $"{rootDir}{Path.DirectorySeparatorChar}.gitignore";

            if (File.Exists(gitIgnoreFilePath))
            {
                bool addPath = true;

                try
                {
                    // Create a new StreamReader, tell it which file to read and what encoding the file was saved as
                    // Immediately clean up the reader after this block of code is done.
                    using (var reader = new StreamReader(gitIgnoreFilePath))
                    {
                        // While there's lines left in the text file, do this:
                        string line;
                        do
                        {
                            line = reader.ReadLine();

                            if (!string.IsNullOrEmpty(line) &&
                                (line.Equals(ignoredPath) || line.Contains(directory)))
                            {
                                // Don't add the line if it already exists.
                                addPath = false;
                            }
                        }
                        while (line != null);

                        // Done reading, close the reader and return true to broadcast success.
                        reader.Close();
                    }
                }
                // If anything broke in the try block, we throw an exception with information on what didn't work.
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (addPath)
                {
                    File.AppendAllText(gitIgnoreFilePath, $"{ignoredPath}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            else
            {
                File.WriteAllText(gitIgnoreFilePath, $"{ignoredPath}{Environment.NewLine}", Encoding.UTF8);
            }
        }

        /// <summary>
        /// Manually update the git submodules for this project.
        /// </summary>
        /// <returns>True, if update was successful.</returns>
        internal static bool UpdateSubmodules()
        {
            if (HasSubmodules)
            {
                bool success = false;
                EditorUtility.DisplayProgressBar("Updating Submodules...", "Please wait...", 0.5f);

                try
                {
                    success = new Process().Run($@"cd ""{RepositoryRootDir}"" && git submodule update --init --recursive", out _);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                return success;
            }

            return true;
        }

        /// <summary>
        /// Gets all the tags from a remote repository.
        /// </summary>
        /// <param name="url">The address of the remote repository.</param>
        /// <returns>A list of tags from the remote repository.</returns>
        public static async Task<IEnumerable<string>> GetAllTagsFromRemoteAsync(string url)
        {
            if (!IsGitInstalled)
            {
                return new List<string>(0);
            }

            var result = await new Process().RunAsync($"git ls-remote --tags {url}");

            if (result.ExitCode != 0 || !(result.Output.Length > 0))
            {
                var messageBuilder = new StringBuilder("Failed to get remote tags:");

                for (int i = 0; i < result.Output.Length; i++)
                {
                    messageBuilder.Append($"\n{result.Output[i]}");
                }

                for (int i = 0; i < result.Errors.Length; i++)
                {
                    messageBuilder.Append($"\n{result.Errors[i]}");
                }

                throw new Exception(messageBuilder.ToString());
            }

            return from tag in result.Output
                   select Regex.Match(tag, "(\\d*\\.\\d*\\.\\d*)")
                   into match
                   where match.Success
                   select match.Value;
        }
    }
}
