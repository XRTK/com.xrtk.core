// Copyright (c) XRTK. All rights reserved.
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
using XRTK.Extensions;
using XRTK.Inspectors.Utilities.SymbolicLinks;
using Debug = UnityEngine.Debug;

namespace XRTK.Inspectors.Utilities
{
    public static class GitUtilities
    {
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

                if (new Process().Run($@"cd ""{Application.dataPath}"" && git rev-parse --show-toplevel", out var rootDir))
                {
                    return projectRootDir = rootDir.ToBackSlashes().Replace("\n", string.Empty);
                }

                return projectRootDir = Directory.GetParent(Application.dataPath).FullName;
            }
        }

        private static string projectRootDir;

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
            if (string.IsNullOrEmpty(ignoredPath))
            {
                Debug.LogError("You cannot pass null or empty parameter.");
                return;
            }

            var rootDir = RepositoryRootDir;
            ignoredPath = ignoredPath.Replace($"{rootDir.ToBackSlashes()}/", string.Empty);
            var ignoreFilePath = $"{rootDir}\\.gitignore";

            if (File.Exists(ignoreFilePath))
            {
                bool addPath = true;

                try
                {
                    // Create a new StreamReader, tell it which file to read and what encoding the file was saved as
                    // Immediately clean up the reader after this block of code is done.
                    using (var reader = new StreamReader(ignoreFilePath))
                    {
                        // While there's lines left in the text file, do this:
                        string line;
                        do
                        {
                            line = reader.ReadLine();

                            if (!string.IsNullOrEmpty(line) && line.Equals(ignoredPath))
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
                    File.AppendAllText(ignoreFilePath, $"{ignoredPath}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            else
            {
                File.WriteAllText(ignoreFilePath, $"{ignoredPath}{Environment.NewLine}", Encoding.UTF8);
            }
        }

        /// <summary>
        /// Manually update the git submodules for this project.
        /// </summary>
        /// <returns>True, if update was successful.</returns>
        internal static bool UpdateSubmodules()
        {
            EditorUtility.DisplayProgressBar("Updating Submodules...", "Please wait...", 0.5f);

            var isGitInstalled = new Process().Run("git --version", out var message) && !message.Contains("'git' is not recognized");

            if (isGitInstalled)
            {
                var success = new Process().Run($@"cd ""{RepositoryRootDir}"" && git submodule update --init --all", out _);

                EditorUtility.ClearProgressBar();
                return success;
            }

            EditorUtility.ClearProgressBar();
            Debug.LogError(message);
            return true;
        }

        /// <summary>
        /// Gets all the tags from a remote repository.
        /// </summary>
        /// <param name="url">The address of the remote repository.</param>
        /// <returns>A list of tags from the remote repository.</returns>
        public static async Task<IEnumerable<string>> GetAllTagsFromRemoteAsync(string url)
        {
            var result = await new Process().RunAsync($"git ls-remote --tags {url}");

            if (result.ExitCode !=0 || !(result.Output.Length > 0))
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
