// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Inspectors.Utilities.SymbolicLinks
{
    [InitializeOnLoad]
    public class SymbolicLinkerWindow : EditorWindow
    {
        private static SymbolicLinkerWindow window;
        private static string sourcePath = string.Empty;
        private static string targetPath = string.Empty;
        private static GUIStyle textFieldGUIStyle;

        static SymbolicLinkerWindow() => EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            var current = Event.current;
            if (current == null) { return; }
            if (current.type != EventType.KeyDown) { return; }

            switch (current.keyCode)
            {
                case KeyCode.Delete:
                    if (DisableSymbolicLinkValidation())
                    {
                        DisableSymbolicLink();
                        Event.current.Use();
                    }

                    break;
            }
        }

        [MenuItem("Assets/Symbolic Links/Create Link", false, 21)]
        private static void CreateSymbolicLink()
        {
            var path = string.Empty;
            sourcePath = string.Empty;
            var guids = Selection.assetGUIDs;
            window = GetWindow<SymbolicLinkerWindow>();

            if (guids != null && guids.Length == 1)
            {
                path = AssetDatabase.GUIDToAssetPath(guids[0]);
            }

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                targetPath = $"{Application.dataPath}/ThirdParty".ToBackSlashes();
            }
            else
            {
                targetPath = Path.GetFullPath(path).ToBackSlashes();
            }

            window.titleContent = new GUIContent("Import Module");
            window.minSize = new Vector2(256, 128);
            window.maxSize = new Vector2(1024, 128);
            window.Show();
        }

        [MenuItem("Assets/Symbolic Links/Disable Link", true, 22)]
        private static bool DisableSymbolicLinkValidation()
        {
            var guids = Selection.assetGUIDs;

            if (guids == null || guids.Length != 1) { return false; }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return false;
            }

            path = Path.GetFullPath(path).ToBackSlashes();

            if (SymbolicLinker.Settings == null) { return false; }

            var symbolicLink = SymbolicLinker.Settings.SymbolicLinks.Find(link => $"{SymbolicLinker.ProjectRoot}{link.TargetRelativePath}" == path);

            return symbolicLink != null && symbolicLink.IsActive;
        }

        [MenuItem("Assets/Symbolic Links/Disable Link", false, 22)]
        private static void DisableSymbolicLink()
        {
            var guids = Selection.assetGUIDs;

            if (guids == null || guids.Length != 1) { return; }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            path = Path.GetFullPath(path).ToBackSlashes();
            var symbolicLink = SymbolicLinker.Settings.SymbolicLinks.Find(link => $"{SymbolicLinker.ProjectRoot}{link.TargetRelativePath}" == path);

            if (symbolicLink == null) { return; }

            switch (EditorUtility.DisplayDialogComplex("Delete this Symbolically linked path?", path, "Disable Link", "Delete Link", "Cancel"))
            {
                case 0:
                    SymbolicLinker.DisableLink(symbolicLink.TargetRelativePath);
                    break;
                case 1:
                    SymbolicLinker.RemoveLink(symbolicLink.SourceRelativePath, symbolicLink.TargetRelativePath);
                    break;
            }

            EditorUtility.SetDirty(SymbolicLinker.Settings);
            AssetDatabase.SaveAssets();

            if (!EditorApplication.isUpdating)
            {
                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            textFieldGUIStyle = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleRight
            };

            GUILayout.Space(8);
            GUILayout.TextField(sourcePath, textFieldGUIStyle);

            if (GUILayout.Button("Choose Source Path"))
            {
                sourcePath = EditorUtility.OpenFolderPanel("Source Path", Application.dataPath, string.Empty).ToBackSlashes();
            }

            GUILayout.Space(10);
            GUILayout.TextField(targetPath, textFieldGUIStyle);

            if (GUILayout.Button("Choose Target Path"))
            {
                targetPath = EditorUtility.OpenFolderPanel("Target Path", targetPath, string.Empty).ToBackSlashes();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("Import Package"))
            {
                if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetPath))
                {
                    Debug.Log("Please specify a path");
                }
                else if (targetPath.Contains(sourcePath))
                {
                    Debug.LogWarning("Holy Infinite Loop Batman! You should probably consider choosing a better path.\n" +
                                     "Preferably one that doesn't create rips in the fabric of time and digital space.");
                }
                else
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    MixedRealityPreferences.AutoLoadSymbolicLinks = true;
                    SymbolicLinker.AddLink(sourcePath, targetPath);
                    EditorUtility.SetDirty(SymbolicLinker.Settings);
                    AssetDatabase.SaveAssets();

                    if (!EditorApplication.isUpdating)
                    {
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}