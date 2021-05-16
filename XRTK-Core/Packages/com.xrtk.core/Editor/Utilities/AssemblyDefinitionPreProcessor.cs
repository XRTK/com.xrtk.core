// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    public class AssemblyDefinitionPreProcessor : AssetPostprocessor
    {
        private const string VersionRegexPattern = "\\[assembly: AssemblyVersion\\(\"(.*)\"\\)\\]";

        private void OnPreprocessAsset()
        {
            if (assetPath.Contains("package.json") && !Application.isBatchMode)
            {
                var text = File.ReadAllText(assetPath);
                var asmdef = AssemblyDefinitionEditorExtension.AssemblyDefinition.FromJson(text);

                if (!asmdef.name.Contains("com.xrtk"))
                {
                    return;
                }

                var newVersion = $"[assembly: AssemblyVersion(\"{asmdef.version}\")]";
                var assemblyPath = assetPath.Replace("package.json", "Runtime/AssemblyInfo.cs");
                var editorAssemblyPath = assetPath.Replace("package.json", "Editor/AssemblyInfo.cs");

                Debug.Assert(File.Exists(assemblyPath));
                Debug.Assert(File.Exists(editorAssemblyPath));

                File.WriteAllText(assemblyPath, Regex.Replace(File.ReadAllText(assemblyPath), VersionRegexPattern, newVersion));
                File.WriteAllText(editorAssemblyPath, Regex.Replace(File.ReadAllText(editorAssemblyPath), VersionRegexPattern, newVersion));
            }
        }
    }
}
