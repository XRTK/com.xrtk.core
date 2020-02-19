// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Providers.Controllers.OpenVR;
using XRTK.Services;
using XRTK.Tests.Services;
using XRTK.Utilities;

namespace XRTK.Tests.Core
{
    public class TestFixture_02_MixedRealityToolkitUtilityTests
    {
        private void SetupServiceLocator()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();
            MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile = ScriptableObject.CreateInstance<MixedRealityRegisteredServiceProvidersProfile>();
        }

        #region Configuration Validation Tests

        [Test]
        public void Test_01_ConfirmDataProviderConfigurationNotPresent()
        {
            SetupServiceLocator();

            var dataProviderTypes = new[] { typeof(TestExtensionServiceProvider1) };
            IMixedRealityServiceConfiguration[] dataProviderConfiguration =
            {
                new MixedRealityServiceConfiguration(typeof(TestExtensionServiceProvider1), "Test Data Provider 1", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null)
            };

            Assert.IsFalse(MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.ValidateService(dataProviderTypes, (IMixedRealityServiceConfiguration<IMixedRealityExtensionService>[])dataProviderConfiguration, false));
        }

        [Test]
        public void Test_02_ConfirmDataProviderConfigurationPresent()
        {
            SetupServiceLocator();
            var dataProviderTypes = new[] { typeof(TestExtensionServiceProvider1) };
            IMixedRealityServiceConfiguration[] dataProviderConfiguration =
            {
                new MixedRealityServiceConfiguration(typeof(TestExtensionServiceProvider1), "Test Data Provider 1", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null)
            };

            var castedDataProviderConfiguration = new IMixedRealityServiceConfiguration<IMixedRealityExtensionService>[dataProviderConfiguration.Length];
            castedDataProviderConfiguration[0] = (IMixedRealityServiceConfiguration<IMixedRealityExtensionService>)dataProviderConfiguration[0];

            MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.RegisteredServiceConfigurations = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.RegisteredServiceConfigurations.AddItem(castedDataProviderConfiguration[0]);
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.ValidateService(dataProviderTypes, castedDataProviderConfiguration, false));
        }

        [Test]
        public void Test_03_ConfirmControllerMappingConfigurationNotPresent()
        {
            SetupServiceLocator();
            var controllerTypes = new[] { typeof(GenericOpenVRController) };
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfiles>();
            Assert.IsFalse(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles.ValidateControllerProfiles(controllerTypes, false));
        }

        #endregion Configuration Validation Tests
    }
}
