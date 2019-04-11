using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using XRTK.Extensions;
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

                if (new Process().Run($@"/C cd {Application.dataPath} && git rev-parse --show-toplevel", out string rootDir))
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
            // SymbolicLinkUtilities.RunSync(true);
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
            var success = new Process().Run($"/C cd \"{RepositoryRootDir}\" && git submodule update --init --recursive", out _);
            EditorUtility.ClearProgressBar();
            // TODO we need to ensure that we return true if git isn't installed.
            return success;
        }
    }
}
