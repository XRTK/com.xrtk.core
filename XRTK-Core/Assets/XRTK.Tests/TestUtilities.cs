// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Inspectors.Extensions;
using XRTK.Services;

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
            InitializeMixedRealityToolkit();

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);

            var configuration = useDefaultProfile
                ? GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>()
                : ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

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