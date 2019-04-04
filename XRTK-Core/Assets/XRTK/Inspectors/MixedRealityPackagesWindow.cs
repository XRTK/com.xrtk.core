using System;
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
        private static bool OpenPackagesWindowValidation()
        {
            return MixedRealityPackageUtilities.IsRunningCheck;
        }

        [MenuItem("Mixed Reality Toolkit/Packages...", false, 99)]
        private static async void OpenPackagesWindow()
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
                isPackageEnabled[i] = !packages[i].Item2;
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

            for (var i = 0; i < packages.Length; i++)
            {
                GUI.enabled = !packages[i].Item1.IsRequiredPackage;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                isPackageEnabled[i] = EditorGUILayout.Toggle(isPackageEnabled[i], GUILayout.Width(12));
                EditorGUILayout.LabelField(new GUIContent(packages[i].Item1.DisplayName.Replace("XRTK.", packages[i].Item1.IsRequiredPackage ? " (Required)" : "")));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            if (GUILayout.Button("Apply"))
            {
                for (var i = 0; i < packages.Length; i++)
                {
                    if (!isPackageEnabled[i] && isPackageInstalled[i])
                    {
                        EditorPreferences.Set($"{packages[i].Item1.Name}_disabled", true);

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{packages[i].Item1.Name}_disabled == true");
                        }
                    }

                    if (isPackageEnabled[i] && !isPackageInstalled[i])
                    {
                        EditorPreferences.Set($"{packages[i].Item1.Name}_disabled", false);

                        if (MixedRealityPackageUtilities.DebugEnabled)
                        {
                            Debug.LogWarning($"{packages[i].Item1.Name}_disabled == false");
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
