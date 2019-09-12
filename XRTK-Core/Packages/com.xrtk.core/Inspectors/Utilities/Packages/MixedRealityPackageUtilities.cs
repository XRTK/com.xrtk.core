// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Extensions.EditorClassExtensions;
using XRTK.Utilities.Async;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace XRTK.Inspectors.Utilities.Packages
{
    public static class MixedRealityPackageUtilities
    {
        /// <summary>
        /// The Mixed Reality Toolkit's upm package settings.
        /// </summary>
        public static MixedRealityPackageSettings PackageSettings
        {
            get
            {
                if (EditorApplication.isUpdating)
                {
                    return packageSettings;
                }

                if (packageSettings == null &&
                    !string.IsNullOrEmpty(MixedRealityPreferences.PackageSettingsPath))
                {
                    packageSettings = AssetDatabase.LoadAssetAtPath<MixedRealityPackageSettings>(MixedRealityPreferences.PackageSettingsPath);
                }

                if (packageSettings == null)
                {
                    packageSettings = ScriptableObject.CreateInstance(nameof(MixedRealityPackageSettings)) as MixedRealityPackageSettings;
                    packageSettings.CreateAsset("Assets/XRTK.Generated/CustomProfiles");
                }

                return packageSettings;
            }
            internal set => packageSettings = value;
        }

        private static MixedRealityPackageSettings packageSettings;

        /// <summary>
        /// Is the package utility running a check?
        /// </summary>
        public static bool IsRunningCheck { get; private set; }

        /// <summary>
        /// Debug the package utility.
        /// </summary>
        public static bool DebugEnabled
        {
            get => MixedRealityPreferences.DebugPackageInfo;
            set => MixedRealityPreferences.DebugPackageInfo = value;
        }

        private static Tuple<MixedRealityPackageInfo, bool>[] currentPackages;

        /// <summary>
        /// Ensures the package manifest is up to date.
        /// </summary>
        public static async void CheckPackageManifest()
        {
            if (IsRunningCheck ||
                EditorApplication.isPlayingOrWillChangePlaymode ||
                Application.isBatchMode)
            {
                return;
            }

            if (PackageSettings == null)
            {
                if (EditorApplication.isUpdating)
                {
                    EditorApplication.delayCall += CheckPackageManifest;
                    return;
                }

                Debug.LogError("Failed to find Package Settings!");
                return;
            }

            IsRunningCheck = true;

            if (DebugEnabled)
            {
                Debug.Log("Checking packages...");
            }

            if (PackageSettings != null)
            {
                Tuple<MixedRealityPackageInfo, bool>[] installedPackages;

                try
                {
                    installedPackages = await GetCurrentMixedRealityPackagesAsync();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                    IsRunningCheck = false;
                    return;
                }

                foreach (var installedPackage in installedPackages)
                {
                    CheckPackage(installedPackage);
                }
            }
            else
            {
                Debug.LogWarning("Failed to find package settings.");
            }

            if (DebugEnabled)
            {
                Debug.Log("Check complete!");
            }

            IsRunningCheck = false;
        }

        private static async void CheckPackage(Tuple<MixedRealityPackageInfo, bool> installedPackage)
        {
            (MixedRealityPackageInfo package, bool isInstalled) = installedPackage;

            if (DebugEnabled)
            {
                Debug.Log($"{package.Name} | Enabled? {package.IsEnabled} | Installed? {isInstalled}");
            }

            if (package.IsRequiredPackage && !isInstalled ||
                (package.IsDefaultPackage && package.IsEnabled && !isInstalled))
            {
                try
                {
                    await AddPackageAsync(package);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                }
            }
            else if (!package.IsEnabled && isInstalled)
            {
                try
                {
                    await RemovePackageAsync(package);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Validates the currently installed upm xrtk packages.
        /// </summary>
        internal static async void ValidatePackages()
        {
            try
            {
                await GetCurrentMixedRealityPackagesAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Returns the currently installed upm xrtk packages.
        /// </summary>
        /// <exception cref="TimeoutException">A <see cref="TimeoutException"/> can occur if the packages are not returned in 10 seconds.</exception>
        internal static async Task<Tuple<MixedRealityPackageInfo, bool>[]> GetCurrentMixedRealityPackagesAsync()
        {
            var packageCount = PackageSettings.MixedRealityPackages.Length;
            currentPackages = new Tuple<MixedRealityPackageInfo, bool>[packageCount];
            var upmPackageListRequest = Client.List(true);

            await upmPackageListRequest.WaitUntil(request => request.IsCompleted, timeout: 30);

            var installedPackages = PackageSettings.MixedRealityPackages
                .Select(package => upmPackageListRequest.Result?
                    .FirstOrDefault(packageInfo => packageInfo.name.Equals(package.Name)))
                .Where(upmPackage => upmPackage != null)
                .ToList();

            for (var i = 0; i < PackageSettings.MixedRealityPackages.Length; i++)
            {
                var package = PackageSettings.MixedRealityPackages[i];
                var isInstalled = installedPackages.Any(UpmCheck);

                currentPackages[i] = new Tuple<MixedRealityPackageInfo, bool>(package, isInstalled);

                bool UpmCheck(PackageInfo packageInfo) => packageInfo != null && packageInfo.name.Equals(package.Name);
            }

            return currentPackages;
        }

        private static async Task AddPackageAsync(MixedRealityPackageInfo packageInfo)
        {
            var versionSeparator = new[] { '.' };
            var tag = (await GitUtilities.GetAllTagsFromRemoteAsync(packageInfo.Uri))
                .OrderBy(value => int.Parse(value.Split(versionSeparator)[0])) // Major
                .ThenBy(value => int.Parse(value.Split(versionSeparator)[1]))  // Minor
                .ThenBy(value => int.Parse(value.Split(versionSeparator)[2]))  // Revision
                .LastOrDefault();
            var addRequest = Client.Add($"{packageInfo.Name}@{packageInfo.Uri}#{tag}");

            await addRequest.WaitUntil(request => request.IsCompleted, timeout: 30);

            if (addRequest.Status == StatusCode.Success)
            {
                if (DebugEnabled)
                {
                    Debug.Log($"successfully added {packageInfo.Name}@{addRequest.Result.packageId}");
                }
            }
            else
            {
                Debug.LogError($"Package Error({addRequest.Error?.errorCode}): {addRequest.Error?.message}");
            }
        }

        private static async Task RemovePackageAsync(MixedRealityPackageInfo packageInfo)
        {
            var removeRequest = Client.Remove($"{packageInfo.Name}");
            await removeRequest.WaitUntil(request => request.IsCompleted, timeout: 30);

            if (removeRequest.Status == StatusCode.Success)
            {
                if (DebugEnabled)
                {
                    Debug.Log($"successfully removed {packageInfo.Name}");
                }
            }
            else if (removeRequest.Error?.errorCode != ErrorCode.NotFound)
            {
                Debug.LogError($"Package Error({removeRequest.Error?.errorCode}): {removeRequest.Error?.message}");
            }
        }
    }
}
