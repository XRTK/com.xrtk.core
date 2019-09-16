// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

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
        private const string ScopedRegistryEntry = @"{
  ""scopedRegistries"": [
    {
      ""name"": ""XRTK"",
      ""url"": ""http://upm.xrtk.io:4873/"",
      ""scopes"": [
        ""com.xrtk""
      ]
    }
  ],
";

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
                    var manifestFilePath = $"{Directory.GetParent(Application.dataPath)}\\Packages\\manifest.json";

                    if (File.Exists(manifestFilePath))
                    {
                        var text = File.ReadAllText(manifestFilePath);

                        if (!text.Contains("XRTK"))
                        {
                            text = text.TrimStart('{');
                            text = $"{ScopedRegistryEntry}{text}";
                        }

                        File.WriteAllText(manifestFilePath, text);

                        Client.Add("com.xrtk.sdk");
                        AssetDatabase.DeleteAsset("Assets/XRTK.Seed");
                    }
                    else
                    {
                        Debug.LogError("Failed to install XRTK, couldn't find the project manifest!");
                    }
                }
            }
        }
    }
}
