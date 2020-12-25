// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Editor.Extensions;
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

        public static void InitializeMixedRealityToolkitScene(bool useDefaultProfile)
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

            MixedRealityToolkitRootProfile configuration;
            if (useDefaultProfile)
            {
                configuration = GetDefaultMixedRealityProfile<MixedRealityToolkitRootProfile>();
                Debug.Assert(configuration.TeleportSystemProfile != null);
                configuration.TeleportSystemProfile.TeleportProvider = typeof(TestTeleportProvider);
            }
            else
            {
                configuration = ScriptableObject.CreateInstance<MixedRealityToolkitRootProfile>();
            }

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Root Profile");
            MixedRealityToolkit.Instance.ResetProfile(configuration);
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
        }

        public static T GetDefaultMixedRealityProfile<T>() where T : BaseMixedRealityProfile
        {
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals(typeof(T).Name));
        }
    }
}