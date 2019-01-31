// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;
using XRTK.Services.InputSystem;

namespace XRTK.Tests.InputSystem
{
    public class TestFixture_03_InputSystemTests
    {
        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = InputSystemTestUtilities.SetupInputSystemProfile();
            MixedRealityToolkit.RegisterService<IMixedRealityInputSystem>(new MixedRealityInputSystem(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile));

            // Tests
            Assert.IsNotEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = InputSystemTestUtilities.SetupInputSystemProfile();
            MixedRealityToolkit.RegisterService<IMixedRealityInputSystem>(new MixedRealityInputSystem(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile));

            // Retrieve Input System
            var inputSystem = MixedRealityToolkit.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test03_TestMixedRealityInputSystemDoesNotExist()
        {
            // Initialize without the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(IMixedRealityInputSystem).Name} service.");
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            // Initialize with the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene();
            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = InputSystemTestUtilities.SetupInputSystemProfile();
            MixedRealityToolkit.RegisterService<IMixedRealityInputSystem>(new MixedRealityInputSystem(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile));

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }
    }
}