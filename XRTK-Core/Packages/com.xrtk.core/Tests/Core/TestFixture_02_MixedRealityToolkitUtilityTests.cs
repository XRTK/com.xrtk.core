// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Platforms;
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
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile = ScriptableObject.CreateInstance<MixedRealityRegisteredServiceProvidersProfile>();
        }

        #region Configuration Validation Tests

        private readonly List<IMixedRealityPlatform> testPlatforms = new List<IMixedRealityPlatform> { new EditorPlatform(), new WindowsStandalonePlatform() };

        [Test]
        public void Test_01_ConfirmRegisteredServiceProviderConfigurationNotPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestService1) };
            IMixedRealityServiceConfiguration<IMixedRealityService>[] newConfigs =
            {
                new MixedRealityServiceConfiguration<IMixedRealityService>(typeof(TestService1), "Test Registered Service 1", 2, testPlatforms, null)
            };

            Assert.IsFalse(profile.ValidateService(dataProviderTypes, newConfigs, false));
        }

        [Test]
        public void Test_02_ConfirmRegisteredServiceProviderConfigurationPresent()
        {
            SetupServiceLocator();
            var profile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            var dataProviderTypes = new[] { typeof(TestService1) };
            var newConfig = new MixedRealityServiceConfiguration<IMixedRealityService>(typeof(TestService1), "Test Registered Service 1", 2, testPlatforms, null);
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

            var controllerDataMappingProfile = ScriptableObject.CreateInstance<UnityInputControllerDataProfile>();

            Assert.IsFalse(controllerDataMappingProfile.ValidateControllerProfiles(controllerTypes, false));
        }

        #endregion Configuration Validation Tests
    }
}
