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
        public void Test_01_ConfirmExtensionServiceProviderConfigurationNotPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestExtensionService1) };
            var newConfigs = new[]
            {
                new MixedRealityServiceConfiguration<IMixedRealityExtensionService>(typeof(TestExtensionService1), "Test Extension Service 1", 2,SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null)
            };

            Assert.IsFalse(profile.ValidateService(dataProviderTypes, newConfigs, false));
        }

        [Test]
        public void Test_02_ConfirmExtensionServiceProviderConfigurationPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestExtensionService1) };
            var newConfig = new MixedRealityServiceConfiguration<IMixedRealityExtensionService>(typeof(TestExtensionService1), "Test Extension Service 1", 2, SupportedPlatforms.WindowsStandalone | SupportedPlatforms.Editor, null);
            Debug.Assert(newConfig != null);
            var newConfigs = profile.RegisteredServiceConfigurations.AddItem(newConfig);
            Debug.Assert(newConfigs != null);
            profile.RegisteredServiceConfigurations = newConfigs;
            Assert.IsTrue(profile.ValidateService(dataProviderTypes, newConfigs, false));
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
