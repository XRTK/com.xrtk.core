// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using NUnit.Framework;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.Tests.InputSystem
{
    public class TestFixture_04_InputActionTests
    {
        [SetUp]
        public void SetupTests()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
        }

        [Test]
        public void Test_01_TestCodeGeneratedActions()
        {
            var pressAction = new MixedRealityInputAction(1, "Pressed", AxisType.Digital);
            var selectAction = new MixedRealityInputAction(2, "Select", AxisType.Digital);

            Assert.IsTrue(selectAction != pressAction);
            Assert.IsTrue(pressAction != MixedRealityInputAction.None);
            Assert.IsTrue(selectAction != MixedRealityInputAction.None);
        }

        [Test]
        public void Test_02_TestBackwardsCompatibility()
        {
            var oldActionWithNoGuid = new MixedRealityInputAction(default, 1, "Select", AxisType.Digital);
            var profileWithGuid = new MixedRealityInputAction(Guid.NewGuid(), 1, "Select", AxisType.Digital);
            Assert.IsTrue(profileWithGuid.ProfileGuid != default);
            Assert.IsTrue(oldActionWithNoGuid == profileWithGuid);
        }

        [TearDown]
        public void Teardown()
        {
            TestUtilities.CleanupScene();
        }
    }
}