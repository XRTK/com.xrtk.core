// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.PlatformSystem;
using XRTK.Definitions.Utilities;
using XRTK.Inspectors.Extensions;
using XRTK.Services;
using XRTK.Services.PlatformSystem;

namespace XRTK.Tests
{
    public static class TestUtilities
    {
        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        public static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void InitializeMixedRealityToolkitScene(bool useDefaultProfile = false)
        {
            // Setup
            CleanupScene();
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized);
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);

            var configuration = useDefaultProfile
                ? GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>()
                : ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

            configuration.PlatformSystemProfile = ScriptableObject.CreateInstance<MixedRealityPlatformSystemProfile>();
            configuration.PlatformSystemType = new SystemType(typeof(MixedRealityPlatformSystem));

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Configuration Profile");
            MixedRealityToolkit.Instance.ResetConfiguration(configuration);
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
        }

        public static T GetDefaultMixedRealityProfile<T>() where T : BaseMixedRealityProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals($"Default{typeof(T).Name}"));
        }
    }
}