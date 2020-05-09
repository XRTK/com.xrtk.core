// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Extensions;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.Providers;
using XRTK.Interfaces.Providers.SpatialObservers;
using XRTK.Services;

namespace XRTK.Editor
{
    public static class PackageInstaller
    {
        /// <summary>
        /// Attempt to copy any assets found in the source path into the project.
        /// </summary>
        /// <param name="sourcePath">The source path of the assets to be installed. This should typically be from a hidden upm package folder marked with a "~".</param>
        /// <param name="destinationPath">The destination path, typically inside the projects "Assets" directory.</param>
        /// <param name="regenerateGuids">Should the guids for the copied assets be regenerated?</param>
        /// <returns>Returns true if the profiles were successfully copies, installed, and added to the <see cref="MixedRealityToolkitRootProfile"/>.</returns>
        public static bool TryInstallAssets(string sourcePath, string destinationPath, bool regenerateGuids = true)
        {
            return TryInstallAssets(new Dictionary<string, string> { { sourcePath, destinationPath } }, regenerateGuids);
        }

        /// <summary>
        /// Attempt to copy any assets found in the source path into the project.
        /// </summary>
        /// <param name="installationPaths">The assets paths to be installed. Key is the source path of the assets to be installed. This should typically be from a hidden upm package folder marked with a "~". Value is the destination.</param>
        /// <param name="regenerateGuids">Should the guids for the copied assets be regenerated?</param>
        /// <returns>Returns true if the profiles were successfully copies, installed, and added to the <see cref="MixedRealityToolkitRootProfile"/>.</returns>
        public static bool TryInstallAssets(Dictionary<string, string> installationPaths, bool regenerateGuids = true)
        {
            var anyFail = false;
            var installedAssets = new List<string>();
            var installedDirectories = new List<string>();

            foreach (var installationPath in installationPaths)
            {
                var sourcePath = installationPath.Key;
                var destinationPath = installationPath.Value;
                installedDirectories.Add(destinationPath);

                Debug.Log($"{sourcePath} -> {destinationPath}");

                if (Directory.Exists(destinationPath))
                {
                    EditorUtility.DisplayProgressBar("Verifying assets...", $"{sourcePath} -> {destinationPath}", 0);

                    installedAssets.AddRange(UnityFileHelper.GetUnityAssetsAtPath(destinationPath));

                    for (int i = 0; i < installedAssets.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Verifying assets...", Path.GetFileNameWithoutExtension(installedAssets[i]), i / (float)installedAssets.Count);
                        installedAssets[i] = installedAssets[i].Replace($"{Directory.GetParent(Application.dataPath).FullName}\\", string.Empty).ToForwardSlashes();
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetFullPath(destinationPath));
                    EditorUtility.DisplayProgressBar("Copying assets...", $"{sourcePath} -> {destinationPath}", 0);

                    var copiedAssets = UnityFileHelper.GetUnityAssetsAtPath(sourcePath);

                    for (var i = 0; i < copiedAssets.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Copying assets...", Path.GetFileNameWithoutExtension(copiedAssets[i]), i / (float)copiedAssets.Count);

                        try
                        {
                            copiedAssets[i] = CopyAsset(sourcePath, copiedAssets[i], destinationPath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            anyFail = true;
                        }
                    }

                    if (!anyFail)
                    {
                        installedAssets.AddRange(copiedAssets);
                    }

                    EditorUtility.ClearProgressBar();
                }
            }

            if (anyFail)
            {
                foreach (var installedDirectory in installedDirectories)
                {
                    try
                    {
                        if (Directory.Exists(installedDirectory))
                        {
                            Directory.Delete(installedDirectory);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            if (regenerateGuids)
            {
                GuidRegenerator.RegenerateGuids(installedDirectories);
            }

            EditorApplication.delayCall += () => AddConfigurations(installedAssets);
            return true;
        }

        private static void AddConfigurations(List<string> profiles)
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            foreach (var profile in profiles.Where(x => x.EndsWith(".asset")))
            {
                var platformConfigurationProfile = AssetDatabase.LoadAssetAtPath<MixedRealityPlatformServiceConfigurationProfile>(profile);

                if (platformConfigurationProfile == null) { continue; }

                var rootProfile = MixedRealityToolkit.IsInitialized
                    ? MixedRealityToolkit.Instance.ActiveProfile
                    : ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitRootProfile>()[0];

                if (EditorUtility.DisplayDialog("We found a new Platform Configuration",
                    $"We found the {platformConfigurationProfile.name.ToProperCase()}. Would you like to add this platform configuration to your {rootProfile.name}?",
                    "Yes, Absolutely!",
                    "later"))
                {
                    InstallConfiguration(platformConfigurationProfile, rootProfile);
                }
                else
                {
                    EditorUtility.DisplayDialog("Attention!", "Each data provider will need to be manually registered in each service configuration.", "OK");
                }
            }
        }

        private static string CopyAsset(this string rootPath, string sourceAssetPath, string destinationPath)
        {
            sourceAssetPath = sourceAssetPath.ToForwardSlashes();
            destinationPath = $"{destinationPath}{sourceAssetPath.Replace(Path.GetFullPath(rootPath), string.Empty)}".ToForwardSlashes();
            destinationPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, destinationPath);

            if (!File.Exists(destinationPath))
            {
                Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);

                File.Copy(sourceAssetPath, destinationPath);
            }

            return destinationPath.Replace($"{Directory.GetParent(Application.dataPath).FullName}\\", string.Empty);
        }

        /// <summary>
        /// Installs the provided <see cref="MixedRealityServiceConfiguration"/> in the provided <see cref="MixedRealityToolkitRootProfile"/>.
        /// </summary>
        /// <param name="platformConfigurationProfile">The platform configuration to install.</param>
        /// <param name="rootProfile">The root profile to install the </param>
        public static void InstallConfiguration(MixedRealityPlatformServiceConfigurationProfile platformConfigurationProfile, MixedRealityToolkitRootProfile rootProfile)
        {
            foreach (var configuration in platformConfigurationProfile.Configurations)
            {
                var configurationType = configuration.InstancedType.Type;

                if (configurationType == null)
                {
                    Debug.LogError($"Failed to find a valid {nameof(configuration.InstancedType)} for {configuration.Name}!");
                    continue;
                }

                switch (configurationType)
                {
                    case Type _ when typeof(IMixedRealityCameraDataProvider).IsAssignableFrom(configurationType):
                        var cameraSystemProfile = rootProfile.CameraSystemProfile;
                        var cameraDataProviderConfiguration = new MixedRealityServiceConfiguration<IMixedRealityCameraDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                        if (cameraSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != cameraDataProviderConfiguration.InstancedType.Type))
                        {
                            Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                            cameraSystemProfile.RegisteredServiceConfigurations = cameraSystemProfile.RegisteredServiceConfigurations.AddItem(cameraDataProviderConfiguration);
                            EditorUtility.SetDirty(cameraSystemProfile);
                        }
                        break;

                    case Type _ when typeof(IMixedRealityInputDataProvider).IsAssignableFrom(configurationType):
                        var inputSystemProfile = rootProfile.InputSystemProfile;
                        var inputDataProviderConfiguration = new MixedRealityServiceConfiguration<IMixedRealityInputDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                        if (inputSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != inputDataProviderConfiguration.InstancedType.Type))
                        {
                            Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                            inputSystemProfile.RegisteredServiceConfigurations = inputSystemProfile.RegisteredServiceConfigurations.AddItem(inputDataProviderConfiguration);
                            EditorUtility.SetDirty(inputSystemProfile);
                        }
                        break;

                    case Type _ when typeof(IMixedRealitySpatialAwarenessDataProvider).IsAssignableFrom(configurationType):
                        var spatialAwarenessSystemProfile = rootProfile.SpatialAwarenessProfile;
                        var spatialObserverConfiguration = new MixedRealityServiceConfiguration<IMixedRealitySpatialAwarenessDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                        if (spatialAwarenessSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != spatialObserverConfiguration.InstancedType.Type))
                        {
                            Debug.Log($"Added {configuration.Name} to {rootProfile.name}");
                            spatialAwarenessSystemProfile.RegisteredServiceConfigurations = spatialAwarenessSystemProfile.RegisteredServiceConfigurations.AddItem(spatialObserverConfiguration);
                            EditorUtility.SetDirty(spatialAwarenessSystemProfile);
                        }
                        break;
                }
            }

            AssetDatabase.SaveAssets();
            EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
