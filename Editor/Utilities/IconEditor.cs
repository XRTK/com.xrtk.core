// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Editor.Utilities
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DefaultAsset))]
    public class IconEditor : UnityEditor.Editor
    {
        private Texture2D icon;
        private string filter;
        private string[] filters;
        private bool filterFlag;
        private bool overwriteIcons;

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            icon = (Texture2D)EditorGUILayout.ObjectField("Icon Texture", icon, typeof(Texture2D), false);
            filter = EditorGUILayout.TextField(new GUIContent("Partial name filters", "Use comma separated values for each partial name search."), filter);
            filterFlag = EditorGUILayout.Toggle(filterFlag ? "Skipping filter results" : "Targeting filter results", filterFlag);

            EditorGUI.BeginChangeCheck();
            overwriteIcons = EditorGUILayout.Toggle("Overwrite Icon?", overwriteIcons);

            if (GUILayout.Button("Set Icons for child script assets"))
            {
                filters = !string.IsNullOrEmpty(filter) ? filter.Split(',') : null;

                Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

                for (int i = 0; i < selectedAsset.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Updating Icons...", $"{i} of {selectedAsset.Length} {selectedAsset[i].name}", i / (float)selectedAsset.Length);
                    var path = AssetDatabase.GetAssetPath(selectedAsset[i]);
                    if (!path.Contains(".cs")) { continue; }

                    if (filters != null)
                    {
                        bool matched = filterFlag;

                        for (int j = 0; j < filters.Length; j++)
                        {
                            if (selectedAsset[i].name.ToLower().Contains(filters[j].ToLower()))
                            {
                                matched = !filterFlag;
                            }
                        }

                        if (overwriteIcons && !matched ||
                           !overwriteIcons && matched)
                        {
                            continue;
                        }
                    }

                    try
                    {
                        SetIcon(path, icon, overwriteIcons);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorUtility.ClearProgressBar();
            }

            GUI.enabled = false;
        }

        private void SetIcon(string objectPath, Texture2D texture, bool overwrite)
        {
            var monoImporter = AssetImporter.GetAtPath(objectPath) as MonoImporter;
            var setIcon = monoImporter.GetIcon();

            if (setIcon != null && !overwrite)
            {
                return;
            }

            monoImporter.SetIcon(texture);
            monoImporter.SaveAndReimport();
        }
    }
}
