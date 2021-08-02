// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;

namespace XRTK.Tests.InputSystem
{
    public class TestFixture_03_InputSystemTests
    {
        [Test]
        public void Test01_CreateMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            MixedRealityToolkit.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out var inputSystem);

            // Tests
            Assert.IsNotEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsNotNull(inputSystem);
            Assert.AreEqual(activeSystemCount + 1, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(activeServiceCount + 1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test02_TestGetMixedRealityInputSystem()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            MixedRealityToolkit.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out var service);

            Assert.IsNotEmpty(MixedRealityToolkit.ActiveSystems);

            // Retrieve Input System
            var inputSystem = MixedRealityToolkit.GetService<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(service);
            Assert.IsNotNull(inputSystem);
            Assert.IsTrue(ReferenceEquals(service, inputSystem));
        }

        [Test]
        public void Test03_TestMixedRealityInputSystemDoesNotExist()
        {
            // Initialize without the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Check for Input System
            var inputSystemExists = MixedRealityToolkit.IsServiceRegistered<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(IMixedRealityInputSystem)} service.");
        }

        [Test]
        public void Test04_TestMixedRealityInputSystemExists()
        {
            // Initialize with the default profile configuration
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add Input System
            MixedRealityToolkit.Instance.ActiveProfile.AddConfiguration(InputSystemTestUtilities.TestInputSystemConfiguration);
            MixedRealityToolkit.TryCreateAndRegisterService(InputSystemTestUtilities.TestInputSystemConfiguration, out _);

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
