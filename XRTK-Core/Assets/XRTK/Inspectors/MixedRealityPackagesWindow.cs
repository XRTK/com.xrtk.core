using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Inspectors.Utilities;
using XRTK.Utilities.Editor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace XRTK.Inspectors
{
    public class MixedRealityPackagesWindow : EditorWindow
    {
        private static MixedRealityPackagesWindow window;

        private static List<PackageInfo> installedPackages = new List<PackageInfo>();
        private static bool[] isPackageEnabled = null;
        private static bool[] isPackageInstalled = null;

        [MenuItem("Mixed Reality Toolkit/Packages...", false, 99)]
        public static async void OpenPackagesWindow()
        {
            if (window != null) { window.Close(); }

            installedPackages.Clear();

            window = CreateInstance(typeof(MixedRealityPackagesWindow)) as MixedRealityPackagesWindow;
            Debug.Assert(window != null);
            window.titleContent = new GUIContent("XRTK upm Packages");
            window.minSize = new Vector2(288, 320);
            window.ShowUtility();

            installedPackages = await MixedRealityPackageUtilities.GetCurrentMixedRealityPackagesAsync();

            if (installedPackages != null && installedPackages.Count > 0)
            {
                var packages = MixedRealityPackageUtilities.PackageSettings.MixedRealityPackages;
                Debug.Assert(packages != null);
                var packageCount = packages.Length;
                isPackageEnabled = new bool[packageCount];
                isPackageInstalled = new bool[packageCount];

                for (var i = 0; i < packages.Length; i++)
                {
                    isPackageEnabled[i] = isPackageInstalled[i] = installedPackages.Any(Predicate);

                    bool Predicate(PackageInfo packageInfo)
                    {
                        return packageInfo != null && packageInfo.name.Equals(packages[i].Name);
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (window == null) { Close(); }

            EditorGUILayout.BeginVertical();
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Mixed Reality Toolkit Unity Package Manager");

            if (installedPackages == null)
            {
                EditorGUILayout.LabelField("No Packages found!");
                EditorGUILayout.EndVertical();
                return;
            }

            if (installedPackages.Count == 0)
            {
                EditorGUILayout.LabelField("Gathering Package data...");
                EditorGUILayout.EndVertical();
                return;
            }

            var packages = MixedRealityPackageUtilities.PackageSettings.MixedRealityPackages;

            for (var i = 0; i < packages.Length; i++)
            {
                if (packages[i].DisplayName.Equals("XRTK.UpmExtensions")) { continue; }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                isPackageEnabled[i] = EditorGUILayout.Toggle(isPackageEnabled[i], GUILayout.Width(12));
                EditorGUILayout.LabelField(new GUIContent(packages[i].DisplayName.Replace("XRTK.", "")));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Apply"))
            {
                MixedRealityPackageUtilities.UpdatePackages(isPackageEnabled, isPackageInstalled);
                Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
