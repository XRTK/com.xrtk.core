﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Editor.Utilities
{
    public static class AssemblyDefinitionEditorExtension
    {
        private const string DLL = ".dll";
        private const string META = ".meta";
        private const string JSON = ".json";
        private const string ASMDEF = ".asmdef";

        #region Serialzied Data Objects

        [Serializable]
        internal class AsmDefSourceFiles
        {
            public string[] Files = null;
        }

        [Serializable]
        internal class AssemblyDefinition
        {
            public string name = null;
            public string version = null;
            public string[] references = null;
            public string[] optionalUnityReferences = null;
            public string[] includePlatforms = null;
            public string[] excludePlatforms = null;
            public bool allowUnsafeCode = false;

            public AsmDefSourceFiles Source { get; set; } = new AsmDefSourceFiles();

            public static AssemblyDefinition FromJson(string json)
            {
                var scriptAssemblyData = JsonUtility.FromJson<AssemblyDefinition>(json);
                if (scriptAssemblyData == null) { throw new Exception("Json file does not contain an assembly definition"); }
                if (string.IsNullOrEmpty(scriptAssemblyData.name)) { throw new Exception("Required property 'name' not set"); }
                if (scriptAssemblyData.excludePlatforms != null && scriptAssemblyData.excludePlatforms.Length > 0 &&
                   (scriptAssemblyData.includePlatforms != null && scriptAssemblyData.includePlatforms.Length > 0))
                {
                    throw new Exception("Both 'excludePlatforms' and 'includePlatforms' are set.");
                }

                return scriptAssemblyData;
            }

            public static string ToJson(AssemblyDefinition definition)
            {
                return JsonUtility.ToJson(definition, true);
            }
        }

        #endregion Serialzied Data Objects

        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Source with Assembly", true, 99)]
        public static bool ReplaceWithAssemblyValidation()
        {
            if (Selection.activeObject.IsNull()) { return false; }
            return !AssetDatabase.GetAssetPath(Selection.activeObject).GetAssetPathSiblings().Any(path => path.Contains(DLL));
        }

        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Source with Assembly", false, 99)]
        public static void ReplaceWithAssembly()
        {
            Debug.Assert(Selection.activeObject != null);

            EditorUtility.DisplayProgressBar("Replacing source with assembly", "Getting things ready...", 0);

            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var directoryPath = new FileInfo(assetPath).Directory?.FullName;
            var assemblyDefinitionText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath).text;
            var scriptAssemblyData = AssemblyDefinition.FromJson(assemblyDefinitionText);
            var fromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(scriptAssemblyData.name);

            Debug.Assert(!string.IsNullOrEmpty(scriptAssemblyData.name));
            Debug.Assert(fromAssemblyName == assetPath, "Failed to get the proper assembly name!");

            if (CompilationPipeline.GetAssemblies(AssembliesType.Editor).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath) ||
                CompilationPipeline.GetAssemblies(AssembliesType.Player).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath))
            {
                EditorUtility.DisplayProgressBar("Replacing source with assembly", "Saving source meta data for later...", 0.95f);
                File.WriteAllText(assetPath, AssemblyDefinition.ToJson(scriptAssemblyData));
                File.WriteAllText($"{Path.GetFullPath(assetPath).Hide()}{JSON}", JsonUtility.ToJson(scriptAssemblyData.Source, true));
            }
            else
            {
                Debug.LogError("Failed to replace source code with assembly!");
            }

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            EditorUtility.ClearProgressBar();
        }

        private static bool ReplaceSourceWithAssembly(this Assembly[] assemblies, ref AssemblyDefinition assemblyDefinition, string directoryPath)
        {
            EditorUtility.DisplayProgressBar("Replacing source with assembly", "Gathering assembly information...", 0.1f);

            for (var i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                EditorUtility.DisplayProgressBar("Replacing source with assembly", $"Processing assembly {assembly.name}", i / (float)assemblies.Length);

                if (assembly.name != assemblyDefinition.name) { continue; }

                Debug.Assert(assembly.sourceFiles != null);
                Debug.Assert(assembly.sourceFiles.Length > 0);
                Debug.Assert(File.Exists(assembly.outputPath));

                assemblyDefinition.Source.Files = assembly.sourceFiles;
                AssetDatabase.ReleaseCachedFileHandles();

                for (var j = 0; j < assembly.sourceFiles.Length; j++)
                {
                    var fullPath = Path.GetFullPath(assembly.sourceFiles[j]);
                    var newPath = fullPath.Hide();

                    EditorUtility.DisplayProgressBar("Replacing source with assembly", $"Processing file {Path.GetFileName(fullPath)}", j / (float)assembly.sourceFiles.Length);

                    File.Move(fullPath, newPath);
                    File.Move($"{fullPath}{META}", $"{newPath}{META}");
                }

                var assemblyPath = $"{directoryPath}{Path.DirectorySeparatorChar}{assembly.name}{DLL}";

                EditorUtility.DisplayProgressBar("Replacing source with assembly", "Copying assembly into project...", 0.5f);

                File.Copy(assembly.outputPath, assemblyPath);

                EditorUtility.DisplayProgressBar("Replacing source with assembly", "Importing plugin...", 0.625f);

                AssetDatabase.ImportAsset(assemblyPath.GetUnityProjectRelativePath());

                EditorUtility.DisplayProgressBar("Replacing source with assembly", "Updating plugin settings...", 0.75f);

                var importedAssembly = (PluginImporter)AssetImporter.GetAtPath(assemblyPath.GetUnityProjectRelativePath());

                if (importedAssembly == null)
                {
                    Debug.LogError("Failed to get plugin importer!");
                    return true;
                }

                if (assemblyDefinition.excludePlatforms != null && assemblyDefinition.excludePlatforms.Length > 0 &&
                    assemblyDefinition.includePlatforms != null && assemblyDefinition.includePlatforms.Length > 0)
                {
                    Selection.activeObject = importedAssembly;
                    Debug.LogError("Unable to update plugin import settings, as both exclude and include platforms have been enabled.");
                    return true;
                }

                BuildTarget buildTarget;
                importedAssembly.SetCompatibleWithAnyPlatform(assemblyDefinition.includePlatforms == null || assemblyDefinition.includePlatforms.Length == 0);

                if (assemblyDefinition.includePlatforms != null && assemblyDefinition.includePlatforms.Length > 0)
                {
                    importedAssembly.SetCompatibleWithEditor(assemblyDefinition.includePlatforms.Contains("Editor"));

                    for (int j = 0; j < assemblyDefinition.includePlatforms?.Length; j++)
                    {
                        if (assemblyDefinition.includePlatforms[j].TryGetBuildTarget(out buildTarget))
                        {
                            importedAssembly.SetCompatibleWithPlatform(buildTarget, true);
                        }
                    }
                }

                if (assemblyDefinition.excludePlatforms != null && assemblyDefinition.excludePlatforms.Length > 0)
                {
                    importedAssembly.SetCompatibleWithEditor(!assemblyDefinition.excludePlatforms.Contains("Editor"));

                    for (int j = 0; j < assemblyDefinition.excludePlatforms?.Length; j++)
                    {
                        if (assemblyDefinition.excludePlatforms[j].TryGetBuildTarget(out buildTarget))
                        {
                            importedAssembly.SetExcludeFromAnyPlatform(buildTarget, true);
                        }
                    }
                }

                EditorUtility.DisplayProgressBar("Replacing source with assembly", "Saving and re-importing with updated settings...", 0.8f);

                importedAssembly.SaveAndReimport();
                Selection.activeObject = importedAssembly;

                return true;
            }

            return false;
        }

        [MenuItem("CONTEXT/PluginImporter/Replace Assembly with Source", true, 99)]
        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Assembly with Source", true, 99)]
        public static bool ReplaceWithSourceValidation()
        {
            if (Selection.activeObject.IsNull()) { return false; }

            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            return assetPath.GetAssetPathSiblings().Any(path => assetPath.Contains(DLL) && path.Contains(ASMDEF) || assetPath.Contains(ASMDEF) && path.Contains(DLL));
        }

        [MenuItem("CONTEXT/PluginImporter/Replace Assembly with Source", false, 99)]
        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Assembly with Source", false, 99)]
        public static void ReplaceWithSource()
        {
            Debug.Assert(Selection.activeObject != null);

            EditorUtility.DisplayProgressBar("Replacing assembly with source", "Getting things ready...", 0);

            var builtAssemblyPath = string.Empty;
            var assemblyDefinitionPath = string.Empty;
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (assetPath.Contains(DLL))
            {
                builtAssemblyPath = assetPath;
                assemblyDefinitionPath = assetPath.FindSiblingFileByExtension(ASMDEF);
            }
            else if (assetPath.Contains(ASMDEF))
            {
                assemblyDefinitionPath = assetPath;
                builtAssemblyPath = assetPath.FindSiblingFileByExtension(DLL);
            }

            EditorUtility.DisplayProgressBar("Replacing assembly with source", "Getting source file data...", .25f);

            Debug.Assert(!string.IsNullOrEmpty(builtAssemblyPath), "No Assembly found for this Assembly Definition!");
            Debug.Assert(!string.IsNullOrEmpty(assemblyDefinitionPath), "No Assembly Definition found for this Assembly!");
            var assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);
            Debug.Assert(assemblyDefinitionAsset != null, $"Failed to load assembly def asset at {assemblyDefinitionPath}");
            var assemblyDefinitionText = assemblyDefinitionAsset.text;
            var scriptAssemblyData = AssemblyDefinition.FromJson(assemblyDefinitionText);
            var assemblySourcePath = $"{Path.GetFullPath(assemblyDefinitionPath).Hide()}{JSON}";
            Debug.Assert(File.Exists(assemblySourcePath), "Fatal Error: Missing meta data to re-import source files. You'll need to manually do it by removing the '.' in front of each file.");
            string sourceFilesText = File.ReadAllText(assemblySourcePath);
            File.Delete(assemblySourcePath);
            scriptAssemblyData.Source = JsonUtility.FromJson<AsmDefSourceFiles>(sourceFilesText);

            Debug.Assert(scriptAssemblyData != null);
            Debug.Assert(scriptAssemblyData.Source?.Files != null, "Fatal Error: Missing meta data to re-import source files. You'll need to manually do it by removing the '.' in front of each file.");

            for (var i = 0; i < scriptAssemblyData.Source.Files.Length; i++)
            {
                var sourceFile = scriptAssemblyData.Source.Files[i];
                var fullHiddenPath = Path.GetFullPath(sourceFile).Hide();

                if (!File.Exists(fullHiddenPath))
                {
                    Debug.LogError($"Failed to find source file for {scriptAssemblyData.name}.{Path.GetFileNameWithoutExtension(sourceFile)}");
                    continue;
                }

                var sourcePath = fullHiddenPath.UnHide();

                EditorUtility.DisplayProgressBar("Replacing assembly with source", $"Processing file {Path.GetFileName(fullHiddenPath).UnHide()}", i / (float)scriptAssemblyData.Source.Files.Length);

                File.Move(fullHiddenPath, sourcePath);
                File.Move($"{fullHiddenPath}{META}", $"{sourcePath}{META}");
            }

            EditorUtility.DisplayProgressBar("Replacing assembly with source", "Deleting assembly...", .75f);

            File.Delete(builtAssemblyPath);
            File.Delete($"{builtAssemblyPath}{META}");

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            EditorUtility.ClearProgressBar();
        }

        private static string Hide(this string path)
        {
            var index = path.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);

            if (index == 0)
            {
                index = path.LastIndexOf(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal);
            }

            if (index == 0)
            {
                return $".{path}";
            }

            return path.Insert(index + 1, ".");
        }

        private static string UnHide(this string path)
        {
            string fileName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(fileName)) { return path; }
            if (!fileName.Contains(".")) { return path; }

            if (fileName.IndexOf(".", StringComparison.Ordinal) == 0)
            {
                fileName = fileName.TrimStart('.');
            }

            return path.Replace($"{Path.DirectorySeparatorChar}.{fileName}", $"{Path.DirectorySeparatorChar}{fileName}");
        }

        private static string GetUnityProjectRelativePath(this string path)
        {
            return path.Replace(Path.GetFullPath(Application.dataPath), "Assets").Replace("\\", "/");
        }

        private static string[] GetAssetPathSiblings(this string assetPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(assetPath), $"Invalid asset path {assetPath}");
            string directoryPath = new FileInfo(assetPath).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath), $"Invalid root path {directoryPath}");
            return Directory.GetFiles(directoryPath);
        }

        private static string FindSiblingFileByExtension(this string path, string extension)
        {
            var directoryPath = new FileInfo(path).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath));
            var files = Directory.GetFiles(directoryPath);

            for (var i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) == extension)
                {
                    return files[i].GetUnityProjectRelativePath();
                }
            }

            return string.Empty;
        }

        private static bool TryGetBuildTarget(this string platform, out BuildTarget buildTarget)
        {
            switch (platform)
            {
                case "Editor":
                    buildTarget = BuildTarget.NoTarget;
                    return false;
                case "Android":
                    buildTarget = BuildTarget.Android;
                    return true;
                case "iOS":
                    buildTarget = BuildTarget.iOS;
                    return true;
                case "LinuxStandalone32":
                case "LinuxStandalone64":
                case "LinuxStandaloneUniversal":
                    buildTarget = BuildTarget.StandaloneLinux64;
                    return true;
                case "macOSStandalone":
                    buildTarget = BuildTarget.StandaloneOSX;
                    return true;
                case "PS4":
                    buildTarget = BuildTarget.PS4;
                    return true;
                case "Switch":
                    buildTarget = BuildTarget.Switch;
                    return true;
                case "tvOS":
                    buildTarget = BuildTarget.tvOS;
                    return true;
                case "WSA":
                    buildTarget = BuildTarget.WSAPlayer;
                    return true;
                case "WebGL":
                    buildTarget = BuildTarget.WebGL;
                    return true;
                case "WindowsStandalone32":
                    buildTarget = BuildTarget.StandaloneWindows;
                    return true;
                case "WindowsStandalone64":
                    buildTarget = BuildTarget.StandaloneWindows64;
                    return true;
                case "XboxOne":
                    buildTarget = BuildTarget.XboxOne;
                    return true;
                default:
                    // If unsupported then it needs to be added to the switch statement above.
                    Debug.LogError($"{platform} unsupported!");
                    buildTarget = BuildTarget.NoTarget;
                    return false;
            }
        }
    }
}
