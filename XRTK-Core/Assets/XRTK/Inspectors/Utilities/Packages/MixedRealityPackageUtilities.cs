using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using XRTK.Utilities.Async;
using XRTK.Utilities.Editor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace XRTK.Inspectors.Utilities.Packages
{
    public static class MixedRealityPackageUtilities
    {
        /// <summary>
        /// The Mixed Reality Toolkit's upm package settings.
        /// </summary>
        private static MixedRealityPackageSettings PackageSettings
        {
            get
            {
                if (packageSettings == null)
                {
                    var path = $"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}\\Inspectors\\Utilities\\Packages\\MixedRealityPackageSettings.asset";

                    if (DebugEnabled)
                    {
                        Debug.Log(path);
                    }

                    packageSettings = AssetDatabase.LoadAssetAtPath<MixedRealityPackageSettings>(path);
                }

                return packageSettings;
            }
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

        private static Tuple<MixedRealityPackageInfo, bool, bool>[] currentPackages;

        /// <summary>
        /// Ensures the package manifest is up to date.
        /// </summary>
        public static async void CheckPackageManifest()
        {
            if (IsRunningCheck ||
                Application.isPlaying ||
                Application.isBatchMode ||
                PackageSettings == null)
            {
                return;
            }

            IsRunningCheck = true;

            if (DebugEnabled)
            {
                Debug.Log("Checking packages...");
            }

            Tuple<MixedRealityPackageInfo, bool, bool>[] installedPackages;

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

            if (DebugEnabled)
            {
                Debug.Log("Check complete!");
            }

            IsRunningCheck = false;
        }

        private static async void CheckPackage(Tuple<MixedRealityPackageInfo, bool, bool> installedPackage)
        {
            (MixedRealityPackageInfo package, bool isEnabled, bool isInstalled) = installedPackage;

            if (DebugEnabled)
            {
                Debug.Log($"{package.Name}_enabled == {isEnabled}");
            }

            if (package.IsRequiredPackage && !isInstalled ||
                (package.IsDefaultPackage && isEnabled && !isInstalled))
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
            else if (!isEnabled && isInstalled)
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
        internal static async Task<Tuple<MixedRealityPackageInfo, bool, bool>[]> GetCurrentMixedRealityPackagesAsync()
        {
            var packageCount = PackageSettings.MixedRealityPackages.Length;
            currentPackages = new Tuple<MixedRealityPackageInfo, bool, bool>[packageCount];
            var installedPackages = new List<PackageInfo>();
            var validationFiles = AssetDatabase.FindAssets("PackageValidation");
            var validatedPackages = new List<MixedRealityPackageValidation>(5);
            var upmPackageListRequest = Client.List(true);

            await upmPackageListRequest.WaitUntil(request => request.IsCompleted, timeout: 30);

            foreach (var guid in validationFiles)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (!path.EndsWith(".asset")) { continue; }

                if (DebugEnabled)
                {
                    Debug.Log($"Attempting to load validation at {path}");
                }

                var validation = AssetDatabase.LoadAssetAtPath<MixedRealityPackageValidation>(path);

                if (validation != null)
                {
                    validatedPackages.Add(validation);
                }
            }

            foreach (var package in PackageSettings.MixedRealityPackages)
            {
                var upmPackage = upmPackageListRequest.Result?.FirstOrDefault(packageInfo => packageInfo.name.Equals(package.Name));
                var validPackages = validatedPackages.Where(validation => validation.PackageName.Equals(package.Name)).ToList();
                var validationCount = validPackages.Count;

                if (DebugEnabled)
                {
                    Debug.Log($"{package.Name} | validation count: {validationCount} | is upm package? {upmPackage != null}");
                }

                if (validationCount > 0)
                {
                    EditorPreferences.Set($"{package.Name}_enabled", true);

                    if (validPackages.Count == 1 &&
                        !validPackages[0].IsMainProjectAsset)
                    {
                        continue;
                    }

                    try
                    {
                        await RemovePackageAsync(package);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"{e.Message}\n{e.StackTrace}");
                    }

                    continue;
                }

                if (upmPackage != null)
                {
                    installedPackages.Add(upmPackage);
                }
            }

            for (var i = 0; i < PackageSettings.MixedRealityPackages.Length; i++)
            {
                var package = PackageSettings.MixedRealityPackages[i];
                var isInstalled = installedPackages.Any(UpmCheck) || validatedPackages.Any(ValidationCheck);

                currentPackages[i] = new Tuple<MixedRealityPackageInfo, bool, bool>(
                    package,
                    EditorPreferences.Get($"{package.Name}_enabled", true) || package.IsRequiredPackage,
                    isInstalled);

                bool ValidationCheck(MixedRealityPackageValidation validation) => validation.PackageName.Equals(package.Name);
                bool UpmCheck(PackageInfo packageInfo) => packageInfo != null && packageInfo.name.Equals(package.Name);
            }

            return currentPackages;
        }

        private static async Task AddPackageAsync(MixedRealityPackageInfo packageInfo)
        {
            var addRequest = Client.Add($"{packageInfo.Name}@{packageInfo.Uri}");
            await addRequest.WaitUntil(request => request.IsCompleted, timeout: 30);

            if (addRequest.Status == StatusCode.Success)
            {
                if (DebugEnabled)
                {
                    Debug.Log($"successfully added {packageInfo.Name}@{addRequest.Result.packageId}");
                }

                packageInfo.PackageInfo = addRequest.Result;
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

        public static string GetRevisionHash(this MixedRealityPackageInfo packageInfo)
        {
            return GetRevisionHash(packageInfo.PackageInfo != null ? packageInfo.PackageInfo.resolvedPath : string.Empty);
        }

        private static string GetRevisionHash(string resolvedPath)
        {
            var match = Regex.Match(resolvedPath, "@([^@]+)$");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
