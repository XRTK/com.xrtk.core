// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using NUnit.Framework;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Services;

namespace XRTK.Tests.InputSystem
{
    public class TestFixture_04_InputActionTests
    {
        [SetUp]
        public void SetupTests()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);
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
            var profileAction = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions.FirstOrDefault(action => action.Id == 1);
            Assert.IsTrue(profileAction.ProfileGuid != default);
            Assert.IsTrue(oldActionWithNoGuid == profileAction);
        }

        [TearDown]
        public void Teardown()
        {
            TestUtilities.CleanupScene();
        }
    }
}