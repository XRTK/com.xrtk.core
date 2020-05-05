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
        private const string META_SUFFIX = ".meta";

        /// <summary>
        /// Attempt to copy any assets found in the source path into the project.
        /// </summary>
        /// <param name="sourcePath">The source path of the assets to be installed. This should typically be from a hidden upm package folder marked with a "~".</param>
        /// <param name="destinationPath">The destination path, typically inside the projects "Assets" directory.</param>
        /// <param name="regenerateGuids">Should the guids for the copied assets be regenerated?</param>
        /// <returns>Returns true if the profiles were successfully copies, installed, and added to the <see cref="MixedRealityToolkitRootProfile"/>.</returns>
        public static bool TryInstallAssets(string sourcePath, string destinationPath, string extension = "asset", bool regenerateGuids = true)
        {
            if (Directory.Exists(destinationPath))
            {
                var installedAssets = Directory.EnumerateFiles(Path.GetFullPath(destinationPath), $"*.{extension}", SearchOption.AllDirectories).ToList();

                for (int i = 0; i < installedAssets.Count; i++)
                {
                    installedAssets[i] = installedAssets[i].Replace($"{Directory.GetParent(Application.dataPath).FullName}\\", string.Empty).ToForwardSlashes();
                }

                EditorApplication.delayCall += () => AddConfigurations(installedAssets);
                return true;
            }
            else
            {
                Directory.CreateDirectory(Path.GetFullPath(destinationPath));
            }

            EditorUtility.DisplayProgressBar("Copying assets...", $"{sourcePath} -> {destinationPath}", 0);
            var assetPaths = Directory.EnumerateFiles(Path.GetFullPath(sourcePath), $"*.{extension}", SearchOption.AllDirectories).ToList();

            var anyFail = false;

            for (var i = 0; i < assetPaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Copying assets...", Path.GetFileNameWithoutExtension(assetPaths[i]), i / (float)assetPaths.Count);

                try
                {
                    assetPaths[i] = CopyAsset(sourcePath, assetPaths[i], destinationPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    anyFail = true;
                }
            }

            EditorUtility.ClearProgressBar();

            if (anyFail)
            {
                return false;
            }

            if (regenerateGuids)
            {
                GuidRegenerator.RegenerateGuids(Path.GetFullPath(destinationPath), false);
            }

            EditorApplication.delayCall += () => AddConfigurations(assetPaths);
            return true;
        }

        private static void AddConfigurations(List<string> profiles)
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            foreach (var profile in profiles)
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
                File.Copy($"{sourceAssetPath}{META_SUFFIX}", $"{destinationPath}{META_SUFFIX}");
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

                    case Type _ when typeof(IMixedRealitySpatialObserverDataProvider).IsAssignableFrom(configurationType):
                        var spatialAwarenessSystemProfile = rootProfile.SpatialAwarenessProfile;
                        var spatialObserverConfiguration = new MixedRealityServiceConfiguration<IMixedRealitySpatialObserverDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

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
