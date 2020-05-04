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

        public static bool TryInstallProfiles(string sourcePath, string destinationPath)
        {
            var anyFail = false;
            var profilePaths = Directory.EnumerateFiles(Path.GetFullPath(sourcePath), "*.asset", SearchOption.AllDirectories).ToList();

            for (var i = 0; i < profilePaths.Count; i++)
            {
                try
                {
                    profilePaths[i] = CopyAsset(sourcePath, profilePaths[i], $"{destinationPath}\\Profiles");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    anyFail = true;
                }
            }

            EditorApplication.delayCall += () => { AddConfigurations(profilePaths); };

            return !anyFail;
        }

        private static void AddConfigurations(List<string> profiles)
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            foreach (var profile in profiles)
            {
                var platformConfigurationProfile = AssetDatabase.LoadAssetAtPath<MixedRealityServiceConfigurationProfile>(profile);

                if (platformConfigurationProfile == null) { continue; }

                var rootProfile = MixedRealityToolkit.IsInitialized
                    ? MixedRealityToolkit.Instance.ActiveProfile
                    : ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitRootProfile>()[0];

                if (EditorUtility.DisplayDialog("We found a new Platform Configuration",
                    $"We found the {platformConfigurationProfile.name.ToProperCase()}. Would you like to add this platform configuration to your {rootProfile.name}?",
                    "Yes, Absolutely!",
                    "later"))
                {
                    foreach (var configuration in platformConfigurationProfile.Configurations)
                    {
                        var configurationType = configuration.InstancedType.Type;
                        Debug.Log(configuration.InstancedType.Type.Name);

                        switch (configurationType)
                        {
                            case Type cameraDataProvider when typeof(IMixedRealityCameraDataProvider).IsAssignableFrom(configurationType):
                                var cameraSystemProfile = rootProfile.CameraSystemProfile;
                                var cameraDataProviderConfiguration = new MixedRealityServiceConfiguration<IMixedRealityCameraDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                                if (cameraSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != cameraDataProviderConfiguration.InstancedType.Type))
                                {
                                    Debug.Log($"Added {configuration.Name}");
                                    cameraSystemProfile.RegisteredServiceConfigurations = cameraSystemProfile.RegisteredServiceConfigurations.AddItem(cameraDataProviderConfiguration);
                                }
                                break;

                            case Type inputDataProvider when typeof(IMixedRealityInputDataProvider).IsAssignableFrom(configurationType):
                                var inputSystemProfile = rootProfile.InputSystemProfile;
                                var inputDataProviderConfiguration = new MixedRealityServiceConfiguration<IMixedRealityInputDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                                if (inputSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != inputDataProviderConfiguration.InstancedType.Type))
                                {
                                    Debug.Log($"Added {configuration.Name}");
                                    inputSystemProfile.RegisteredServiceConfigurations = inputSystemProfile.RegisteredServiceConfigurations.AddItem(inputDataProviderConfiguration);
                                }
                                break;

                            case Type spatialDataProvider when typeof(IMixedRealitySpatialObserverDataProvider).IsAssignableFrom(configurationType):
                                var spatialAwarenessSystemProfile = rootProfile.SpatialAwarenessProfile;
                                var spatialObserverConfiguration = new MixedRealityServiceConfiguration<IMixedRealitySpatialObserverDataProvider>(configuration.InstancedType, configuration.Name, configuration.Priority, configuration.RuntimePlatforms, configuration.Profile);

                                if (spatialAwarenessSystemProfile.RegisteredServiceConfigurations.All(serviceConfiguration => serviceConfiguration.InstancedType.Type != spatialObserverConfiguration.InstancedType.Type))
                                {
                                    Debug.Log($"Added {configuration.Name}");
                                    spatialAwarenessSystemProfile.RegisteredServiceConfigurations = spatialAwarenessSystemProfile.RegisteredServiceConfigurations.AddItem(spatialObserverConfiguration);
                                }
                                break;
                        }
                    }

                    AssetDatabase.SaveAssets();
                    EditorApplication.delayCall += AssetDatabase.Refresh;
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
    }
}
