using System;
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
        static MixedRealityPackageUtilities()
        {
            EditorApplication.delayCall += CheckPackageManifest;
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

        /// <summary>
        /// Is the package utility running a check?
        /// </summary>
        public static bool IsRunningCheck { get; private set; } = false;

        /// <summary>
        /// Debug the package utility.
        /// </summary>
        public static bool DebugEnabled { get; set; } = false;

        private static Tuple<MixedRealityPackageInfo, bool, bool>[] currentPackages;

        /// <summary>
        /// Ensures the package manifest is up to date.
        /// </summary>
        public static async void CheckPackageManifest()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (IsRunningCheck) { return; }

            IsRunningCheck = true;

            if (DebugEnabled)
            {
                Debug.Log("Checking packages...");
            }

            var installedPackages = await GetCurrentMixedRealityPackagesAsync();

            foreach (var installedPackage in installedPackages)
            {
                (MixedRealityPackageInfo package, bool isEnabled, bool isInstalled) = installedPackage;

                if (DebugEnabled)
                {
                    Debug.Log($"{package.Name}_enabled == {isEnabled}");
                }

                if (package.IsRequiredPackage && !isInstalled ||
                    (package.IsDefaultPackage && isEnabled && !isInstalled))
                {
                    await AddPackage(package);
                }
                else if (!isEnabled && isInstalled)
                {
                    await RemovePackage(package);
                }
            }

            if (DebugEnabled)
            {
                Debug.Log("Check complete!");
            }

            IsRunningCheck = false;
        }

        /// <summary>
        /// Returns the currently installed upm xrtk packages.
        /// </summary>
        internal static async Task<Tuple<MixedRealityPackageInfo, bool, bool>[]> GetCurrentMixedRealityPackagesAsync()
        {
            var packageCount = PackageSettings.MixedRealityPackages.Length;
            currentPackages = new Tuple<MixedRealityPackageInfo, bool, bool>[packageCount];
            var installedPackages = new List<PackageInfo>();
            var upmPackageListRequest = Client.List(true);

            await new WaitUntil(() => upmPackageListRequest.Status != StatusCode.InProgress);

            if (upmPackageListRequest.Result == null)
            {
                return null;
            }

            foreach (var mrtkPackageInfo in PackageSettings.MixedRealityPackages)
            {
                installedPackages.Add(
                    upmPackageListRequest.Result.FirstOrDefault(
                        packageInfo => packageInfo.name.Equals(mrtkPackageInfo.Name)));
            }

            for (var i = 0; i < PackageSettings.MixedRealityPackages.Length; i++)
            {
                var package = PackageSettings.MixedRealityPackages[i];
                currentPackages[i] = new Tuple<MixedRealityPackageInfo, bool, bool>(
                    package,
                    EditorPreferences.Get($"{package.Name}_enabled", true) || package.IsRequiredPackage,
                    installedPackages.Any(Predicate));

                bool Predicate(PackageInfo packageInfo)
                {
                    return packageInfo != null && packageInfo.name.Equals(package.Name);
                }
            }

            return currentPackages;
        }

        private static async Task AddPackage(MixedRealityPackageInfo packageInfo)
        {
            var addRequest = Client.Add($"{packageInfo.Name}@{packageInfo.Uri}");
            await new WaitUntil(() => addRequest.Status != StatusCode.InProgress);

            if (addRequest.Status == StatusCode.Success && DebugEnabled)
            {
                Debug.Log($"successfully added {packageInfo.Name}");
            }
            else
            {
                Debug.LogError($"Package Error({addRequest.Error?.errorCode}): {addRequest.Error?.message}");
            }
        }

        private static async Task RemovePackage(MixedRealityPackageInfo packageInfo)
        {
            var removeRequest = Client.Remove($"{packageInfo.Name}");
            await new WaitUntil(() => removeRequest.Status != StatusCode.InProgress);

            if (removeRequest.Status == StatusCode.Success && DebugEnabled)
            {
                Debug.Log($"successfully removed {packageInfo.Name}");
            }
            else if (removeRequest.Error?.errorCode != ErrorCode.NotFound)
            {
                Debug.LogError($"Package Error({removeRequest.Error?.errorCode}): {removeRequest.Error?.message}");
            }
        }
    }
}
