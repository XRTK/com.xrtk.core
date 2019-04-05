using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Utilities;
using XRTK.Utilities.Editor;

namespace XRTK.Inspectors
{
    public class MixedRealityPackagesWindow : EditorWindow
    {
        private static MixedRealityPackagesWindow window;
        private static Tuple<MixedRealityPackageInfo, bool, bool>[] packages;
        private static bool[] isPackageEnabled;
        private static bool[] isPackageInstalled;

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
            window.ShowUtility();

            packages = await MixedRealityPackageUtilities.GetCurrentMixedRealityPackagesAsync();

            isPackageEnabled = new bool[packages.Length];
            isPackageInstalled = new bool[packages.Length];

            for (var i = 0; i < packages.Length; i++)
            {
                isPackageEnabled[i] = packages[i].Item2;
                isPackageInstalled[i] = packages[i].Item3;
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
                EditorGUILayout.LabelField("Gathering Package data...");
                EditorGUILayout.EndVertical();
                return;
            }

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 256;

            for (var i = 0; i < packages.Length; i++)
            {
                (MixedRealityPackageInfo package, bool packageEnabled, bool packageInstalled) = packages[i];

                bool CheckDependency(Tuple<MixedRealityPackageInfo, bool, bool> packageSetting)
                {
                    (MixedRealityPackageInfo packageInfo, bool isEnabled, bool isInstalled) = packageSetting;
                    return packageEnabled && packageInstalled && isEnabled && isInstalled && packageInfo.Dependencies.Contains(package.Name);
                }

                var hasDependency = packages.Any(CheckDependency);
                GUI.enabled = !package.IsRequiredPackage && !hasDependency;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                isPackageEnabled[i] = EditorGUILayout.Toggle(isPackageEnabled[i], GUILayout.Width(12));
                var packageName = package.DisplayName.Replace("XRTK.", package.IsRequiredPackage
                    ? " (Required) " : hasDependency
                        ? " (Dependency) " : "");
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
                    (MixedRealityPackageInfo package, bool _, bool _) = packages[i];

                    if (!isPackageEnabled[i] && isPackageInstalled[i])
                    {
                        EditorPreferences.Set($"{package.Name}_enabled", false);

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{package.Name}_enabled == false");
                        }
                    }

                    if (isPackageEnabled[i] && !isPackageInstalled[i])
                    {
                        EditorPreferences.Set($"{package.Name}_enabled", true);

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{package.Name}_enabled == true");
                        }
                    }
                }

                MixedRealityPackageUtilities.CheckPackageManifest();
                Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
