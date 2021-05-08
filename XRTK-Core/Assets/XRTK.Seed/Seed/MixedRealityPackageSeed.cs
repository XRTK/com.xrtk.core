// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

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
        private static readonly ScopedRegistry XRTKRegistry = new ScopedRegistry
        {
            Name = "XRTK",
            Url = "http://upm.xrtk.io:4873/",
            Scopes = new List<string> { "com.xrtk" }
        };

        static MixedRealityPackageSeed()
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            Assembly assembly = null;

            try
            {
                assembly = Assembly.Load("XRTK");
            }
            catch
            {
                // ignored
            }
            finally
            {
                if (assembly == null)
                {
                    AddRegistry();
                }
            }
        }

        private static void AddRegistry()
        {
            try
            {
                if (File.Exists(PackageManifest.ManifestFilePath))
                {
                    var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(PackageManifest.ManifestFilePath));

                    if (manifest.ScopedRegistries == null)
                    {
                        manifest.ScopedRegistries = new List<ScopedRegistry>();
                    }

                    bool install = true;

                    foreach (var scopedRegistry in manifest.ScopedRegistries)
                    {
                        if (scopedRegistry.Name == "XRTK")
                        {
                            install = false;
                        }
                    }

                    if (install)
                    {
                        manifest.ScopedRegistries.Add(XRTKRegistry);
                        File.WriteAllText(PackageManifest.ManifestFilePath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
                    }

                    ListXrtkPackages();
                }
                else
                {
                    Debug.LogError("Failed to install XRTK, couldn't find the project manifest!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [MenuItem("Mixed Reality Toolkit/List Packages")]
        internal static void ListXrtkPackages()
        {
            var progress = 0f;
            EditorUtility.DisplayProgressBar("Searching upm packages...", "Searching...", progress++ / 100f);
            var request = Client.SearchAll(false);
            var xrtkPackages = new List<PackageInfo>();

            EditorApplication.update += Progress;

            void Progress()
            {
                switch (request.Status)
                {
                    case StatusCode.Success:
                        xrtkPackages.AddRange(request.Result.Where(package => package.name.Contains("com.xrtk.")));
                        break;

                    case StatusCode.InProgress:
                        EditorUtility.DisplayProgressBar("Searching upm packages...", "Searching...", progress += 0.0025f);
                        if (progress > 1f) { progress = 0f; }
                        break;

                    default: // case StatusCode.Failure:
                        if (request.Status >= StatusCode.Failure) { Debug.Log(request.Error.message); }
                        break;
                }

                if (request.IsCompleted)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update -= Progress;
                    PackagePickerWindow.ShowPackageWindow(xrtkPackages);
                }
            }
        }
    }
}
