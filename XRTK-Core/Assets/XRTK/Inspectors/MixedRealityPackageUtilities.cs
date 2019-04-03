using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Utilities.Async;
using XRTK.Utilities.Editor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace XRTK.Inspectors.Utilities
{
    [InitializeOnLoad]
    public static class MixedRealityPackageUtilities
    {
        private const string SessionKey = "_XRTK_Editor_CheckedPackages";

        static MixedRealityPackageUtilities()
        {
            EditorApplication.delayCall += CheckPackageManifest;
        }

        private static async void CheckPackageManifest()
        {
            if (Application.isPlaying ||
                !SessionState.GetBool(SessionKey, true))
            {
                return;
            }

            SessionState.SetBool(SessionKey, false);
            await CheckPackageManifestAsync();
        }

        /// <summary>
        /// The Mixed Reality Toolkit's upm package settings.
        /// </summary>
        public static MixedRealityPackageSettings PackageSettings
        {
            get
            {
                if (packageSettings == null)
                {
                    packageSettings = AssetDatabase
                        .FindAssets($"t:{typeof(MixedRealityPackageSettings).Name}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .OrderBy(x => x)
                        .Select(AssetDatabase.LoadAssetAtPath<MixedRealityPackageSettings>)
                        .FirstOrDefault();
                }

                return packageSettings;
            }
        }

        private static MixedRealityPackageSettings packageSettings;

        private static async Task CheckPackageManifestAsync()
        {
            foreach (var packageInfo in PackageSettings.MixedRealityPackages)
            {
                if (packageInfo.IsDefaultPackage && !EditorPreferences.Get($"{packageInfo.Name}_disabled", false))
                {
                    var searchResult = Client.Search(packageInfo.DisplayName);

                    await new WaitUntil(() => searchResult.Status != StatusCode.InProgress);

                    if (searchResult.Result == null)
                    {
                        Client.Add($"{packageInfo.Name}@{packageInfo.Uri}");
                    }
                }
            }
        }

        /// <summary>
        /// Returns the currently installed upm xrtk packages.
        /// </summary>
        public static async Task<List<PackageInfo>> GetCurrentMixedRealityPackagesAsync()
        {
            if (isUpdatingPackages) { return null; }

            var packageList = new List<PackageInfo>();
            var upmPackageListRequest = Client.List(true);

            await new WaitUntil(() => upmPackageListRequest.Status != StatusCode.InProgress);

            if (upmPackageListRequest.Result == null)
            {
                return null;
            }

            foreach (var mrtkPackageInfo in PackageSettings.MixedRealityPackages)
            {
                packageList.Add(
                    upmPackageListRequest.Result.FirstOrDefault(
                        packageInfo => packageInfo.name.Equals(mrtkPackageInfo.Name)));
            }

            return packageList.Count == 0 ? null : packageList;
        }

        private static bool isUpdatingPackages = false;

        public static async void UpdatePackages(bool[] isPackageEnabled, bool[] isPackageInstalled)
        {
            if (isUpdatingPackages) { return; }
            EditorAssemblyReloadManager.LockReloadAssemblies = true;
            isUpdatingPackages = true;
            var packages = PackageSettings.MixedRealityPackages;

            for (var i = 0; i < packages.Length; i++)
            {
                if (!isPackageEnabled[i] && isPackageInstalled[i])
                {
                    EditorPreferences.Set($"{packages[i].Name}_disabled", true);
                    var removeRequest = Client.Remove($"{packages[i].Name}");
                    await new WaitUntil(() => removeRequest.Status != StatusCode.InProgress);
                }

                if (isPackageEnabled[i] && !isPackageInstalled[i])
                {
                    EditorPreferences.Set($"{packages[i].Name}_disabled", false);
                    var addRequest = Client.Add($"{packages[i].Name}@{packages[i].Uri}");
                    await new WaitUntil(() => addRequest.Status != StatusCode.InProgress);
                }
            }

            isUpdatingPackages = false;
            EditorAssemblyReloadManager.LockReloadAssemblies = false;
        }
    }
}
