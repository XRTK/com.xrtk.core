// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Utilities.Editor
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Mixed Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged
    {
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";

        /// <summary>
        /// Is this the first session of the xrtk?
        /// </summary>
        public static bool IsFirstSession => SessionState.GetBool(SessionKey, true);

        private static string Project_AbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(project_AbsolutePath))
                {
                    project_AbsolutePath = Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName;
                }

                return project_AbsolutePath;
            }
        }

        private static string project_AbsolutePath = string.Empty;

        /// <summary>
        /// The absolute folder path to the Mixed Reality Toolkit in your project.
        /// </summary>
        public static string MixedRealityToolkit_AbsoluteFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(mixedRealityToolkit_AbsoluteFolderPath))
                {
                    return mixedRealityToolkit_AbsoluteFolderPath;
                }

                FindDirectory(Project_AbsolutePath, "Packages/com.xrtk.core", out mixedRealityToolkit_AbsoluteFolderPath);

                return mixedRealityToolkit_AbsoluteFolderPath;
            }
        }

        private static string mixedRealityToolkit_AbsoluteFolderPath = string.Empty;

        /// <summary>
        /// The relative folder path to the Mixed Reality Toolkit in relation to the "Assets" or "Packages" folders.
        /// </summary>
        public static string MixedRealityToolkit_RelativeFolderPath =>
            !string.IsNullOrEmpty(MixedRealityToolkit_AbsoluteFolderPath)
                ? MixedRealityToolkit_AbsoluteFolderPath.Replace($"{Project_AbsolutePath}\\", string.Empty)
                : "Packages/com.xrtk.core";

        /// <summary>
        /// Constructor.
        /// </summary>
        static MixedRealityEditorSettings()
        {
            if (!Application.isBatchMode)
            {
                EditorApplication.delayCall += CheckSettings;
            }
        }

        /// <summary>
        /// Check the Mixed Reality Toolkit's settings.
        /// </summary>
        public static void CheckSettings()
        {
            if (Application.isPlaying ||
                EditorPrefs.GetBool(IgnoreKey, false) ||
                !IsFirstSession)
            {
                return;
            }

            SessionState.SetBool(SessionKey, false);

            bool refresh = false;
            bool restart = false;

            var message = "The Mixed Reality Toolkit needs to apply the following settings to your project:\n\n";

            var forceTextSerialization = EditorSettings.serializationMode == SerializationMode.ForceText;

            if (!forceTextSerialization)
            {
                message += "- Force Text Serialization\n";
            }

            var visibleMetaFiles = EditorSettings.externalVersionControl.Equals("Visible Meta Files");

            if (!visibleMetaFiles)
            {
                message += "- Visible meta files\n";
            }

            var il2Cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;

            if (!il2Cpp)
            {
                message += "- Change the Scripting Backend to use IL2CPP\n";
            }

            if (!PlayerSettings.virtualRealitySupported)
            {
                message += "- Enable XR Settings for your current platform\n";
            }

            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
            {
                message += "- Enable Shared Depth Buffer in the XR SDK Settings\n";
            }

            message += "\nWould you like to make these changes?\n\n";

            if (!forceTextSerialization || !il2Cpp || !visibleMetaFiles || !PlayerSettings.virtualRealitySupported)
            {
                var choice = EditorUtility.DisplayDialogComplex("Apply Mixed Reality Toolkit Default Settings?", message, "Apply", "Ignore", "Later");

                switch (choice)
                {
                    case 0:
                        EditorSettings.serializationMode = SerializationMode.ForceText;
                        EditorSettings.externalVersionControl = "Visible Meta Files";
                        PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                        PlayerSettings.virtualRealitySupported = true;

                        var projectSettingsObject = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/ProjectSettings.asset");
                        Debug.Assert(projectSettingsObject != null);
                        var projectSettings = new SerializedObject(projectSettingsObject);
                        var vrSettingsProperty = projectSettings.FindProperty("vrSettings");
                        Debug.Assert(vrSettingsProperty != null);

                        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer)
                        {
                            var holoLensProperty = vrSettingsProperty.FindPropertyRelative("hololens");
                            Debug.Assert(holoLensProperty != null);
                            var depthBufferSettingsProperty = holoLensProperty.FindPropertyRelative("depthBufferSharingEnabled");
                            Debug.Assert(depthBufferSettingsProperty != null);
                            depthBufferSettingsProperty.boolValue = true;
                            depthBufferSettingsProperty.serializedObject.ApplyModifiedProperties();
                        }

                        refresh = true;
                        break;
                    case 1:
                        EditorPrefs.SetBool(IgnoreKey, true);
                        break;
                    case 2:
                        break;
                }
            }

            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                restart = true;
            }

            if (refresh || restart)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            if (restart)
            {
                EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).ToString());
            }
        }

        private static bool FindDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            path = string.Empty;
            string[] directories;

            try
            {
                directories = Directory.GetDirectories(directoryPathToSearch);
            }
            catch (Exception)
            {
                return false;
            }

            foreach (var directory in directories)
            {
                var name = Path.GetFileName(directory);

                if (name != null &&
                    name.Contains(directoryName))
                {
                    path = directory;
                    return true;
                }

                if (FindDirectory(directory, directoryName, out path))
                {
                    return true;
                }
            }

            return false;
        }

        #region IActiveBuildTargetChanged Implementation

        /// <inheritdoc />
        public int callbackOrder => 0;

        /// <inheritdoc />
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            SessionState.SetBool(SessionKey, true);
            CheckSettings();
        }

        #endregion IActiveBuildTargetChanged Implementation
    }
}
