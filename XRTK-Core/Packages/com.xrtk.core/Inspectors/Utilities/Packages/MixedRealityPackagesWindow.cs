// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Utilities.Editor;

namespace XRTK.Inspectors.Utilities.Packages
{
    public class MixedRealityPackagesWindow : EditorWindow
    {
        private static MixedRealityPackagesWindow window;
        private static Tuple<MixedRealityPackageInfo, bool>[] packages;
        private static bool[] isPackageEnabled;
        private static bool isError;

        [MenuItem("Mixed Reality Toolkit/Packages...", true, 99)]
        static bool OpenPackagesWindowValidation()
        {
            return !MixedRealityPackageUtilities.IsRunningCheck;
        }

        [MenuItem("Mixed Reality Toolkit/Packages...", false, 99)]
        public static async void OpenPackagesWindow()
        {
            if (window != null) { window.Close(); }

            window = CreateInstance(typeof(MixedRealityPackagesWindow)) as MixedRealityPackagesWindow;
            Debug.Assert(window != null);
            window.titleContent = new GUIContent("XRTK UPM Packages");
            window.minSize = new Vector2(288, 320);
            isError = false;
            window.ShowUtility();

            try
            {
                packages = await MixedRealityPackageUtilities.GetCurrentMixedRealityPackagesAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                isError = true;
                return;
            }

            isPackageEnabled = new bool[packages.Length];

            for (var i = 0; i < packages.Length; i++)
            {
                isPackageEnabled[i] = packages[i].Item1.IsEnabled;
            }
        }

        private void OnGUI()
        {
            if (window == null) { Close(); }

            EditorGUILayout.BeginVertical();
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Mixed Reality Toolkit Unity Package Manager");

            if (packages == null)
            {
                EditorGUILayout.LabelField(isError ? "Failed to find packages!" : "Gathering Package data...");
                EditorGUILayout.EndVertical();
                return;
            }

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 256;

            for (var i = 0; i < packages.Length; i++)
            {
                (MixedRealityPackageInfo package, bool _) = packages[i];

                GUI.enabled = !package.IsRequiredPackage;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                isPackageEnabled[i] = EditorGUILayout.Toggle(isPackageEnabled[i], GUILayout.Width(12));
                var packageName = package.DisplayName.Replace("XRTK.", package.IsRequiredPackage ? " (Required) " : string.Empty);
                EditorGUILayout.LabelField(new GUIContent(packageName));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (GUILayout.Button("Apply"))
            {
                for (var i = 0; i < packages.Length; i++)
                {
                    (MixedRealityPackageInfo package, bool isInstalled) = packages[i];

                    if (!isPackageEnabled[i] && isInstalled)
                    {
                        package.IsEnabled = false;

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{package.Name} isEnabled == false");
                        }
                    }

                    if (isPackageEnabled[i] && !isInstalled)
                    {
                        package.IsEnabled = true;

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{package.Name} isEnabled == true");
                        }
                    }

                    packages[i] = new Tuple<MixedRealityPackageInfo, bool>(package, isInstalled);
                }

                var newPackages = packages.Select(tuple => tuple.Item1).ToArray();
                MixedRealityPackageUtilities.PackageSettings.MixedRealityPackages = newPackages;
                MixedRealityPackageUtilities.CheckPackageManifest();
                Close();
            }

            if (GUILayout.Button("Restore Default Packages"))
            {
                for (int i = 0; i < packages.Length; i++)
                {
                    (MixedRealityPackageInfo package, bool isInstalled) = packages[i];

                    if (package.IsDefaultPackage)
                    {
                        package.IsEnabled = true;
                    }

                    packages[i] = new Tuple<MixedRealityPackageInfo, bool>(package, isInstalled);
                }

                var newPackages = packages.Select(tuple => tuple.Item1).ToArray();
                MixedRealityPackageUtilities.PackageSettings.MixedRealityPackages = newPackages;
                MixedRealityPackageUtilities.CheckPackageManifest();
                Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
