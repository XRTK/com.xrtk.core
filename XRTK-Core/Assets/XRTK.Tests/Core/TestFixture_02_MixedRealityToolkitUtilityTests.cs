// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Providers.Controllers.OpenVR;
using XRTK.Services;
using XRTK.Tests.Services;
using XRTK.Utilities;

namespace XRTK.Tests.Core
{
    public class TestFixture_02_MixedRealityToolkitUtilityTests
    {
        #region Configuration Validation Tests

        [Test]
        public void Test_01_ConfirmDataProviderConfigurationNotPresent()
        {
            var dataProviderTypes = new[] { typeof(TestDataProvider1) };
            var dataProviderConfiguration = new[] { new ControllerDataProviderConfiguration(typeof(TestDataProvider1), "Test Controller Data Provider", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null) };

            TestUtilities.InitializeMixedRealityToolkitScene();

            InitializeDefaultInputSystemProfile();

            Assert.IsFalse(ValidateConfiguration.ValidateDataProviders(dataProviderTypes, dataProviderConfiguration, false));
        }

        [Test]
        public void Test_02_ConfirmDataProviderConfigurationPresent()
        {
            var dataProviderTypes = new[] { typeof(TestDataProvider1) };
            var dataProviderConfiguration = new[] { new ControllerDataProviderConfiguration(typeof(TestDataProvider1), "Test Controller Data Provider", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null) };

            TestUtilities.InitializeMixedRealityToolkitScene();

            InitializeDefaultInputSystemProfile();

            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerDataProvidersProfile.RegisteredControllerDataProviders = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerDataProvidersProfile.RegisteredControllerDataProviders.AddItem<ControllerDataProviderConfiguration>(dataProviderConfiguration[0]);

            Assert.IsTrue(ValidateConfiguration.ValidateDataProviders(dataProviderTypes, dataProviderConfiguration, false));
        }

        [Test]
        public void Test_03_ConfirmControllerMappingConfigurationNotPresent()
        {
            var controllerTypes = new[] { typeof(GenericOpenVRController) };
            var dataProviderConfiguration = new[] { new ControllerDataProviderConfiguration(typeof(TestDataProvider1), "Test Controller Data Provider", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null) };

            TestUtilities.InitializeMixedRealityToolkitScene();

            InitializeDefaultInputSystemProfile();

            Assert.IsFalse(ValidateConfiguration.ValidateControllerProfiles(controllerTypes, false));
        }

        private void InitializeDefaultInputSystemProfile()
        {
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.ControllerDataProvidersProfile = ScriptableObject.CreateInstance<MixedRealityControllerDataProvidersProfile>();
            inputSystemProfile.ControllerMappingProfiles = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfiles>();
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = inputSystemProfile;
        }
        #endregion Configuration Validation Tests
    }
}
