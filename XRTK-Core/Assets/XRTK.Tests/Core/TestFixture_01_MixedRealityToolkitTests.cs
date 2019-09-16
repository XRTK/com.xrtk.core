// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;
using XRTK.Services;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XRTK.Tests.Services;
using XRTK.Utilities.Editor;

namespace XRTK.Tests.Core
{
    public class TestFixture_01_MixedRealityToolkitTests
    {
        #region Service Locator Tests

        [Test]
        public void Test_01_InitializeMixedRealityToolkit()
        {
            TestUtilities.CleanupScene();
            MixedRealityToolkit.ConfirmInitialized();

            Debug.Log(MixedRealityEditorSettings.MixedRealityToolkit_AbsoluteFolderPath);
            Debug.Log(MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath);

            // Tests
            GameObject gameObject = GameObject.Find(nameof(MixedRealityToolkit));
            Assert.AreEqual(nameof(MixedRealityToolkit), gameObject.name);
        }

        [Test]
        public void Test_02_TestNoMixedRealityConfigurationFound()
        {
            // Setup
            TestUtilities.CleanupScene();
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized);
            MixedRealityToolkit.ConfirmInitialized();
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);

            MixedRealityToolkit.Instance.ActiveProfile = null;

            // Tests
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
        }

        [Test]
        public void Test_03_CreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Tests
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        #endregion Service Locator Tests

        #region IMixedRealityDataprovider Tests

        [Test]
        public void Test_04_01_RegisterMixedRealityDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void Test_04_02_01_UnregisterMixedRealityDataProviderByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.UnregisterServicesOfType<ITestDataProvider1>();

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_02_02_UnregisterMixedRealityDataProviderByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Retrieve
            var extensionService1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve service
            var dataProvider = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Validate
            Assert.IsNotNull(dataProvider);

            var success = MixedRealityToolkit.UnregisterService<ITestDataProvider1>(dataProvider.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
        }

        [Test]
        public void Test_04_03_RegisterMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestDataProvider2("Test Data Provider 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_04_UnregisterMixedRealityDataProvidersByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestDataProvider2("Test Data Provider 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.GetService<ITestDataProvider1>();
            var extensionService2 = MixedRealityToolkit.GetService<ITestDataProvider2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success1 = MixedRealityToolkit.UnregisterServicesOfType<ITestDataProvider1>();
            var success2 = MixedRealityToolkit.UnregisterServicesOfType<ITestDataProvider2>();

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();
            var isService2Registered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_05_MixedRealityDataProviderDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test data provider 1
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Validate non-existent data provider
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            Assert.IsFalse(isServiceRegistered);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestDataProvider2).Name} service.");
        }

        [Test]
        public void Test_04_06_MixedRealityDataProviderDoesNotReturn()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            const string serviceName = "Test Data Provider";

            // Add test test data provider
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1(serviceName));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_04_07_ValidateMixedRealityDataProviderName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            var testName1 = "Test04-07-1";
            var testName2 = "Test04-07-2";

            // Add test data providers
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1(testName1));
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestDataProvider2(testName2, 10));

            // Retrieve 
            var dataProvider1 = (TestDataProvider1)MixedRealityToolkit.GetService<ITestDataProvider1>(testName1);
            var dataProvider2 = (TestDataProvider2)MixedRealityToolkit.GetService<ITestDataProvider2>(testName2);

            // Tests
            Assert.AreEqual(testName1, dataProvider1.Name);
            Assert.AreEqual(testName2, dataProvider2.Name);
        }

        [Test]
        public void Test_04_08_GetMixedRealityDataProviderCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test data provider 1
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestExtensionService1("Test04-08-1"));

            // Add test data provider 2
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestExtensionService2("Test04-08-2.1"));
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestExtensionService2("Test04-08-2.2"));

            // Retrieve all ITestDataProvider2 services
            var test2DataProviderServices = MixedRealityToolkit.GetActiveServices<ITestDataProvider2>();

            // Tests
            Assert.AreEqual(2, test2DataProviderServices.Count);
        }

        [Test]
        public void Test_04_09_GetAllMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestExtensionService1("Test16-1.1"));
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestExtensionService1("Test16-1.2"));

            // Add test 2 services
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestExtensionService2("Test16-2.1"));
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestExtensionService2("Test16-2.2"));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityDataprovider Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void Test_05_01_RegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_02_01_UnregisterMixedRealityExtensionServiceByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.UnregisterServicesOfType<ITestExtensionService1>();

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_02_02_UnregisterMixedRealityExtensionServiceByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register ITestExtensionService1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.UnregisterService<ITestExtensionService1>(extensionService1.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
        }

        [Test]
        public void Test_05_03_RegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_04_UnregisterMixedRealityExtensionServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(2, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.AreEqual(extensionServices.Count, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();
            var extensionService2 = MixedRealityToolkit.GetService<ITestExtensionService2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success = MixedRealityToolkit.UnregisterServicesOfType<IMixedRealityExtensionService>();

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();
            var isService2Registered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService1).Name} service.");
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
        }

        [Test]
        public void Test_05_05_MixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {typeof(ITestExtensionService2).Name} service.");
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void Test_05_06_MixedRealityExtensionServiceDoesNotReturnByName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            const string serviceName = "Test ExtensionService 1";

            // Add test ITestExtensionService1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1(serviceName));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_05_07_ValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test14-1"));

            // Add test ExtensionService 2
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test14-2"));

            // Retrieve Test ExtensionService 2-2
            var extensionService2 = (TestExtensionService2)MixedRealityToolkit.GetService<ITestExtensionService2>("Test14-2");

            // ExtensionService 2-2 Tests
            Assert.AreEqual("Test14-2", extensionService2.Name);

            // Retrieve Test ExtensionService 2-1
            var extensionService1 = (TestExtensionService1)MixedRealityToolkit.GetService<ITestExtensionService1>("Test14-1");

            // ExtensionService 2-1 Tests
            Assert.AreEqual("Test14-1", extensionService1.Name);
        }

        [Test]
        public void Test_05_08_GetMixedRealityExtensionServiceCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test ExtensionService 1
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test15-1"));

            // Add test ExtensionServices 2
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.2"));

            // Retrieve ExtensionService2
            var extensionServices = MixedRealityToolkit.GetActiveServices<ITestExtensionService2>();

            // Tests
            Assert.AreEqual(2, extensionServices.Count);
        }

        [Test]
        public void Test_05_09_GetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.2"));

            // Add test 2 services
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.2"));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.AreEqual(4, allExtensionServices.Count);
        }

        #endregion IMixedRealityExtensionService Tests

        #region TryServiceRetrieval Tests

        [Test]
        public void Test_06_01_TryRegisterMixedRealityDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Register
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Retrieve
            ITestDataProvider1 extensionService1 = null;
            bool result = MixedRealityToolkit.TryGetService<ITestDataProvider1>(out extensionService1);

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsTrue(result);
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void Test_06_02_TryRegisterMixedRealityDataProviderFail()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            LogAssert.Expect(LogType.Error, "Unable to find ITestDataProvider1 service.");

            // Retrieve
            ITestDataProvider1 extensionService1 = null;
            bool result = MixedRealityToolkit.TryGetService<ITestDataProvider1>(out extensionService1);

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.IsEmpty(MixedRealityToolkit.RegisteredMixedRealityServices);

            // Tests
            Assert.IsFalse(result);
            Assert.IsNull(extensionService1);
        }

        [Test]
        public void Test_06_03_TryRegisterMixedRealityDataProviderByName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            LogAssert.Expect(LogType.Error, "Unable to find Test Data Provider 2 service.");


            // Register
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test Data Provider 1"));

            // Retrieve
            ITestDataProvider1 extensionService1 = null;
            ITestDataProvider1 extensionService2 = null;

            bool resultTrue = MixedRealityToolkit.TryGetService<ITestDataProvider1>("Test Data Provider 1", out extensionService1);
            bool resultFalse = MixedRealityToolkit.TryGetService<ITestDataProvider1>("Test Data Provider 2", out extensionService2);

            // Tests
            Assert.IsEmpty(MixedRealityToolkit.ActiveSystems);
            Assert.AreEqual(1, MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsTrue(resultTrue, "Test Data Provider 1 found");
            Assert.IsFalse(resultFalse, "Test Data Provider 2 not found");
            Assert.IsNotNull(extensionService1, "Test Data Provider 1 service found");
            Assert.IsNull(extensionService2, "Test Data Provider 2 service not found");
        }

        #endregion TryServiceRetrieval Tests

        #region Service Enable/Disable Tests

        [Test]
        public void Test_07_01_EnableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test07-01-1.1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test07-01-1.2"));

            // Add test 2 services
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestDataProvider2("Test07-01-2.1", 10));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test07-01-2.2"));

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesOfType<ITestService>();

            // Tests
            var testServices = MixedRealityToolkit.GetActiveServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }
        }

        [Test]
        public void Test_07_02_DisableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene();

            // Add test 1 services
            MixedRealityToolkit.RegisterService<ITestDataProvider1>(new TestDataProvider1("Test07-01-1.1"));
            MixedRealityToolkit.RegisterService<ITestExtensionService1>(new TestExtensionService1("Test07-01-1.2"));

            // Add test 2 services
            MixedRealityToolkit.RegisterService<ITestDataProvider2>(new TestDataProvider2("Test07-01-2.1", 10));
            MixedRealityToolkit.RegisterService<ITestExtensionService2>(new TestExtensionService2("Test07-01-2.2"));

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesOfType<ITestService>();

            // Get all services
            var testServices = MixedRealityToolkit.GetActiveServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsTrue((service as ITestService).IsEnabled);
            }

            // Enable all test services
            MixedRealityToolkit.DisableAllServiceOfType<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service is ITestService);
                Assert.IsFalse((service as ITestService).IsEnabled);
            }
        }

        #endregion Service Enable/Disable Tests

        #region TearDown

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }

        #endregion TearDown
    }
}
