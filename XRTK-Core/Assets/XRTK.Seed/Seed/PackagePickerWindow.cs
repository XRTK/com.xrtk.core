// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace XRTK.Seed
{
    public class PackagePickerWindow : EditorWindow
    {
        private const float ITEM_HEIGHT = 24f;
        private const float MIN_VERTICAL_SIZE = 152f;
        private const float MIN_HORIZONTAL_SIZE = 272f;

        private static readonly List<Tuple<PackageInfo, bool>> Packages = new List<Tuple<PackageInfo, bool>>();

        private static bool isNew = false;
        private static PackagePickerWindow window;

        private static Texture2D logo;
        private bool preview = false;

        private static Texture2D Logo => logo != null ? logo : logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/XRTK.Seed/XRTK_Logo.png", typeof(Texture2D));

        internal static void ShowPackageWindow(List<PackageInfo> xrtkPackages)
        {
            if (window != null)
            {
                window.Close();
            }

            if (xrtkPackages == null ||
                xrtkPackages.Count == 0)
            {
                MixedRealityPackageSeed.ListXrtkPackages();
                return;
            }

            Packages.Clear();

            foreach (var package in xrtkPackages)
            {
                switch (package.name)
                {
                    case string _ when package.name.Equals("com.xrtk.sdk"):
                        Packages.Insert(0, new Tuple<PackageInfo, bool>(package, true));
                        break;
                    default:
                        Packages.Add(new Tuple<PackageInfo, bool>(package, true));
                        break;
                }
            }

            var height = MIN_VERTICAL_SIZE + ITEM_HEIGHT * xrtkPackages.Count;

            isNew = true;
            window = CreateInstance<PackagePickerWindow>();
            window.minSize = new Vector2(MIN_HORIZONTAL_SIZE, height);
            window.maxSize = new Vector2(MIN_HORIZONTAL_SIZE, height);
            window.position = new Rect(0f, 0f, MIN_HORIZONTAL_SIZE, height);
            window.titleContent = new GUIContent("XRTK Packages");
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (isNew)
            {
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x - position.width / 2, mousePos.y, position.width, position.height);
                isNew = false;
            }

            if (Packages.Count == 0)
            {
                Debug.LogWarning($"{nameof(PackagePickerWindow)}.{nameof(Packages)} is empty!");
                Close();
            }

            GUILayout.BeginVertical();
            GUILayout.Label(Logo, new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            }, GUILayout.Height(80f), GUILayout.Width(position.width));
            GUILayout.Label("The Mixed Reality Toolkit", new GUIStyle
            {
                normal = new GUIStyleState { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            }, GUILayout.Height(16f));
            EditorGUILayout.Space();
            GUILayout.Label("Available Packages:", new GUIStyle
            {
                normal = new GUIStyleState { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            }, GUILayout.Height(16f));
            preview = GUILayout.Toggle(preview, "  Show preview packages", GUILayout.Height(16f));
            GUILayout.Space(4f);

            for (var i = 0; i < Packages.Count; i++)
            {
                var (package, enabled) = Packages[i];

                if (package.name.Equals("com.xrtk.core")) { continue; }

                EditorGUI.BeginChangeCheck();
                GUILayout.Space(2f);
                GUILayout.BeginHorizontal();
                GUILayout.Space(12f);
                enabled = GUILayout.Toggle(enabled, new GUIContent($"  {package.displayName.Replace("XRTK.", string.Empty)} @ {(preview ? package.versions.latest : package.version)}", package.description));
                GUILayout.EndHorizontal();
                GUILayout.Space(2f);

                if (EditorGUI.EndChangeCheck())
                {
                    Packages[i] = new Tuple<PackageInfo, bool>(package, enabled);
                }
            }

            GUILayout.FlexibleSpace();

            GUI.enabled = !EditorPrefs.HasKey($"{Application.productName}_XRTK"); // Check if the XRTK is already installed in project

            if (GUILayout.Button("Add selected packages", EditorStyles.miniButton, GUILayout.Height(16f)))
            {
                var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(PackageManifest.ManifestFilePath));

                foreach (var item in Packages)
                {
                    var (package, enabled) = item;

                    if (enabled && !manifest.Dependencies.ContainsKey(package.name))
                    {
                        manifest.Dependencies.Add(package.name, preview ? package.versions.latest : package.version);
                    }
                }

                File.WriteAllText(PackageManifest.ManifestFilePath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
                AssetDatabase.DeleteAsset("Assets/XRTK.Seed");
                EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Close();
            }

            GUI.enabled = true;
            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }
    }
}
