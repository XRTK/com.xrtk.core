// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using Debug = UnityEngine.Debug;

namespace XRTK.Seed
{
    /// <summary>
    /// Sets up the Mixed Reality Toolkit package then self destructs.
    /// </summary>
    /// <remarks>
    /// To be used in the export of the traditional .unitypackge
    /// </remarks>
    [InitializeOnLoad]
    public class MixedRealityPackageSeed
    {
        private const string Repository = "https://github.com/XRTK/XRTK-Core.git";

        static MixedRealityPackageSeed()
        {
            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            Assembly assembly = null;

            try
            {
                assembly = Assembly.Load("XRTK");
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                if (assembly == null)
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Normal,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            FileName = "cmd.exe",
                            Arguments = $"/C git ls-remote --tags {Repository}"
                        }
                    };

                    var tag = "0.1.3";

                    try
                    {
                        if (process.Start())
                        {
                            var error = process.StandardError.ReadToEnd();

                            if (string.IsNullOrEmpty(error))
                            {
                                var output = process.StandardOutput.ReadToEnd();

                                tag = (from t in output.Split('\n')
                                       select Regex.Match(t, "(\\d*\\.\\d*\\.\\d*)")
                                        into match
                                       where match.Success
                                       select match.Value)
                                    .LastOrDefault();
                            }
                            else
                            {
                                Debug.LogError(error);
                            }

                            process.WaitForExit();
                            process.Close();
                            process.Dispose();
                        }
                        else
                        {
                            Debug.LogError("Failed to get remote tags for the XRTK package!");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }

                    Client.Add($"com.xrtk.core@{Repository}#{tag}");
                    EditorUtility.DisplayProgressBar("Installing the Mixed Reality Toolkit", "Resolving packages...", 0.1f);
                    AssetDatabase.DeleteAsset("Assets/XRTK.Seed");
                }
            }
        }
    }
}
