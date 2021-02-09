// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using XRTK.Definitions;
using XRTK.Editor.Utilities;
using XRTK.Interfaces;
using XRTK.Services;
using XRTK.Tests.Services;

namespace XRTK.Tests.Core
{
    public class TestFixture_01_MixedRealityToolkitTests
    {
        #region Service Locator Tests

        [Test]
        public void Test_01_InitializeMixedRealityToolkit()
        {
            // Disable throwing error/warning logs in the CI/CD pipeline.
            DevOpsLoggingUtility.LoggingEnabled = false;

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            MixedRealityToolkit.ConfirmInitialized();

            Debug.Log(PathFinderUtility.XRTK_Core_AbsoluteFolderPath);
            Debug.Log(PathFinderUtility.XRTK_Core_RelativeFolderPath);

            // Tests
            var gameObject = GameObject.Find(nameof(MixedRealityToolkit));
            Assert.AreEqual(nameof(MixedRealityToolkit), gameObject.name);
        }

        [Test]
        public void Test_02_TestNoMixedRealityProfileFound()
        {
            // Setup
            TestUtilities.CleanupScene();
            Assert.IsTrue(!MixedRealityToolkit.IsInitialized);
            MixedRealityToolkit.ConfirmInitialized();
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);

            MixedRealityToolkit.Instance.ActiveProfile = null;

            // Tests
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            Assert.IsNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsFalse(MixedRealityToolkit.HasActiveProfile);
            LogAssert.Expect(LogType.Error, $"No {nameof(MixedRealityToolkitRootProfile)} found, cannot initialize the {nameof(MixedRealityToolkit)}");
        }

        [Test]
        public void Test_03_CreateMixedRealityToolkit()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Tests
            Assert.AreEqual(0, MixedRealityToolkit.ActiveSystems.Count);
            Assert.AreEqual(0, MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        #endregion Service Locator Tests

        #region IMixedRealityService Tests

        [Test]
        public void Test_04_01_RegisterMixedRealityServiceAndDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            // Retrieve
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Retrieve
            var dataProvider1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(testService1);
            Assert.IsNotNull(dataProvider1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 2 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsNotNull(dataProvider1);
        }

        [Test]
        public void Test_04_02_01_UnregisterMixedRealityServiceAndDataProviderByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            // Retrieve
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Retrieve
            var dataProvider1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(testService1);
            Assert.IsNotNull(dataProvider1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 2 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Unregister
            var successService = MixedRealityToolkit.TryUnregisterServicesOfType<ITestService>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestService)} service.");

            var successDataProvider = MixedRealityToolkit.TryUnregisterServicesOfType<ITestDataProvider1>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider1)} service.");

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestService>();
            var isDataProviderRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(successService);
            Assert.IsFalse(successDataProvider);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsFalse(isDataProviderRegistered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_02_02_UnregisterMixedRealityDataProviderByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            // Retrieve
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Tests
            Assert.IsNotNull(testService1);

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Retrieve
            var dataProvider1 = MixedRealityToolkit.GetService<ITestDataProvider1>();

            // Tests
            Assert.IsNotNull(dataProvider1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 2 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Unregister
            var successService = MixedRealityToolkit.TryUnregisterService<ITestService>(testService1.Name);
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestService)} service.");
            var successDataProvider = MixedRealityToolkit.TryUnregisterService<ITestDataProvider1>(dataProvider1.Name);
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider1)} service.");

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestService>();
            var isDataProviderRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();

            // Tests
            Assert.IsTrue(successService);
            Assert.IsFalse(successDataProvider);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsFalse(isDataProviderRegistered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_03_RegisterMixedRealityDataProviders()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            // Retrieve
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));
            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(new TestDataProvider2(testService1));

            // Retrieve all registered IMixedRealityDataProviders
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsNotEmpty(extensionServices);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 3 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_04_UnregisterMixedRealityDataProvidersByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            // Retrieve
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Validate
            Assert.IsNotNull(testService1);

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));
            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(new TestDataProvider2(testService1));

            // Retrieve all data providers
            var dataProviders = MixedRealityToolkit.GetActiveServices<IMixedRealityDataProvider>();

            // Tests
            Assert.IsTrue(dataProviders.Count == 2);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 3 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.GetService<ITestDataProvider1>();
            var extensionService2 = MixedRealityToolkit.GetService<ITestDataProvider2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            // Unregister
            var successService = MixedRealityToolkit.TryUnregisterServicesOfType<ITestService>();
            var successDataProvider1 = MixedRealityToolkit.TryUnregisterServicesOfType<ITestDataProvider1>();
            var successDataProvider2 = MixedRealityToolkit.TryUnregisterServicesOfType<ITestDataProvider2>();

            // Tests
            Assert.IsTrue(successService);
            Assert.IsFalse(successDataProvider1);
            Assert.IsFalse(successDataProvider2);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestService>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestService)} service.");
            var isService1Registered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider1>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider1)} service.");
            var isService2Registered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider2>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider2)} service.");

            // Tests
            Assert.IsFalse(isServiceRegistered);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_04_05_MixedRealityDataProviderDoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Add test data provider 1
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Validate non-existent data provider 2
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestDataProvider2>();

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider2)} service.");
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void Test_04_06_MixedRealityDataProviderNameDoesNotReturn()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Register
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());

            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            const string serviceName = "Test Data Provider";

            // Add test test data provider
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1, serviceName));

            // Validate non-existent data provider
            MixedRealityToolkit.GetService<ITestDataProvider2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_04_07_ValidateMixedRealityDataProviderName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            const string testName1 = "Test04-07-1";
            const string testName2 = "Test04-07-2";

            // Add test data providers
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1, testName1));
            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(new TestDataProvider2(testService1, testName2));

            // Retrieve
            var dataProvider1 = MixedRealityToolkit.GetService<ITestDataProvider1>(testName1);
            var dataProvider2 = MixedRealityToolkit.GetService<ITestDataProvider2>(testName2);

            // Tests
            Assert.AreEqual(testName1, dataProvider1.Name);
            Assert.AreEqual(testName2, dataProvider2.Name);
        }

        [Test]
        public void Test_04_08_GetMixedRealityDataProviderCollectionByInterface()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test data provider 1
            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1("Test04-08-1"));
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            var failService = new TestDataProvider1(testService1, "Test04-08-2.1");

            // Add test data provider 2
            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(failService);
            LogAssert.Expect(LogType.Error, $"{failService.Name} does not implement {nameof(ITestDataProvider2)}");

            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(new TestDataProvider2(testService1, "Test04-08-2.2"));

            // Retrieve all ITestDataProvider2 services
            var test2DataProviderServices = MixedRealityToolkit.GetActiveServices<ITestDataProvider2>();

            // Tests
            Assert.IsTrue(1 == test2DataProviderServices.Count);
        }

        #endregion IMixedRealityService Tests

        #region IMixedRealityExtensionService Tests

        [Test]
        public void Test_05_01_RegisterMixedRealityExtensionService()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register ITestExtensionService1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 1 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_02_01_UnregisterMixedRealityExtensionServiceByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register ITestExtensionService1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 1 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.TryUnregisterServicesOfType<ITestExtensionService1>();

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestExtensionService1)} service.");

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_02_02_UnregisterMixedRealityExtensionServiceByTypeAndName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Register ITestExtensionService1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Retrieve ITestExtensionService1
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();

            // Tests
            Assert.IsNotNull(extensionService1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 1 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            var success = MixedRealityToolkit.TryUnregisterService<ITestExtensionService1>(extensionService1.Name);

            // Validate non-existent service
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestExtensionService1)} service.");

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isServiceRegistered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_03_RegisterMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Add test ExtensionService
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + extensionServices.Count == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_04_UnregisterMixedRealityExtensionServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Add test ExtensionService
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test ExtensionService 2"));

            // Retrieve all registered IMixedRealityExtensionServices
            var extensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsNotNull(MixedRealityToolkit.Instance.ActiveProfile);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + extensionServices.Count == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Retrieve services
            var extensionService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();
            var extensionService2 = MixedRealityToolkit.GetService<ITestExtensionService2>();

            // Validate
            Assert.IsNotNull(extensionService1);
            Assert.IsNotNull(extensionService2);

            var success = MixedRealityToolkit.TryUnregisterServicesOfType<IMixedRealityExtensionService>();

            // Validate non-existent service
            var isService1Registered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService1>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestExtensionService1)} service.");
            var isService2Registered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService2>();
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestExtensionService2)} service.");

            // Tests
            Assert.IsTrue(success);
            Assert.IsFalse(isService1Registered);
            Assert.IsFalse(isService2Registered);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_05_05_MixedRealityExtensionService2DoesNotExist()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test ExtensionService 1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test ExtensionService 1"));

            // Validate non-existent ExtensionService
            var isServiceRegistered = MixedRealityToolkit.IsServiceRegistered<ITestExtensionService2>();

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestExtensionService2)} service.");
            Assert.IsFalse(isServiceRegistered);
        }

        [Test]
        public void Test_05_06_MixedRealityExtensionServiceDoesNotReturnByName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            const string serviceName = "Test ExtensionService 1";

            // Add test ITestExtensionService1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1(serviceName));

            // Validate non-existent ExtensionService
            MixedRealityToolkit.GetService<ITestExtensionService2>(serviceName);

            // Tests
            LogAssert.Expect(LogType.Error, $"Unable to find {serviceName} service.");
        }

        [Test]
        public void Test_05_07_ValidateExtensionServiceName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test ExtensionService 1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test14-1"));

            // Add test ExtensionService 2
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test14-2"));

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
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test ExtensionService 1
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test15-1"));

            // Add test ExtensionServices 2
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.1"));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test15-2.2"));

            // Retrieve ExtensionService 2
            var extensionServices = MixedRealityToolkit.GetActiveServices<ITestExtensionService2>();

            // Tests
            Assert.AreEqual(2, extensionServices.Count);
        }

        [Test]
        public void Test_05_09_GetAllMixedRealityExtensionServices()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test 1 services
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.1"));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1("Test16-1.2"));

            // Add test 2 services
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.1"));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2("Test16-2.2"));

            // Retrieve all extension services.
            var allExtensionServices = MixedRealityToolkit.GetActiveServices<IMixedRealityExtensionService>();

            // Tests
            Assert.IsTrue(allExtensionServices.Count == 4);
        }

        #endregion IMixedRealityExtensionService Tests

        #region Service Retrieval Tests

        [Test]
        public void Test_06_01_TryRegisterMixedRealityDataProvider()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Retrieve
            var result = MixedRealityToolkit.TryGetService<ITestDataProvider1>(out var extensionService1);

            // Tests
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 2 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);

            // Tests
            Assert.IsTrue(result);
            Assert.IsNotNull(extensionService1);
        }

        [Test]
        public void Test_06_02_TryRegisterMixedRealityDataProviderFail()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            // Retrieve
            var result = MixedRealityToolkit.TryGetService<ITestDataProvider1>(out var extensionService1);
            LogAssert.Expect(LogType.Error, $"Unable to find {nameof(ITestDataProvider1)} service.");

            // Tests
            Assert.IsFalse(result);
            Assert.IsNull(extensionService1);
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
        }

        [Test]
        public void Test_06_03_TryRegisterMixedRealityDataProviderByName()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);
            var activeSystemCount = MixedRealityToolkit.ActiveSystems.Count;
            var activeServiceCount = MixedRealityToolkit.RegisteredMixedRealityServices.Count;

            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Register
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));

            // Retrieve
            var resultTrue = MixedRealityToolkit.TryGetService<ITestDataProvider1>("Test Data Provider 1", out var extensionService1);
            var resultFalse = MixedRealityToolkit.TryGetService<ITestDataProvider1>("Test Data Provider 2", out var extensionService2);

            // Tests
            LogAssert.Expect(LogType.Error, "Unable to find Test Data Provider 2 service.");
            Assert.IsTrue(activeSystemCount == MixedRealityToolkit.ActiveSystems.Count);
            Assert.IsTrue(activeServiceCount + 2 == MixedRealityToolkit.RegisteredMixedRealityServices.Count);
            Assert.IsTrue(resultTrue, "Test Data Provider 1 found");
            Assert.IsFalse(resultFalse, "Test Data Provider 2 not found");
            Assert.IsNotNull(extensionService1, "Test Data Provider 1 service found");
            Assert.IsNull(extensionService2, "Test Data Provider 2 service not found");
        }

        #endregion Service Retrieval Tests Tests

        #region Service Enable/Disable Tests

        [Test]
        public void Test_07_01_EnableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            MixedRealityToolkit.TryRegisterService<ITestService>(new TestService1());
            var testService1 = MixedRealityToolkit.GetService<ITestService>();

            // Add test 1 services
            MixedRealityToolkit.TryRegisterService<ITestDataProvider1>(new TestDataProvider1(testService1));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1());

            // Add test 2 services
            MixedRealityToolkit.TryRegisterService<ITestDataProvider2>(new TestDataProvider2(testService1));
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2());

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesOfType<ITestService>();

            // Tests
            var testServices = MixedRealityToolkit.GetActiveServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service != null);
                Assert.IsTrue(service.IsEnabled);
            }
        }

        [Test]
        public void Test_07_02_DisableServicesByType()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            // Add test 1 services
            MixedRealityToolkit.TryRegisterService<ITestExtensionService1>(new TestExtensionService1());
            var testService1 = MixedRealityToolkit.GetService<ITestExtensionService1>();
            MixedRealityToolkit.TryRegisterService<ITestExtensionDataProvider1>(new TestExtensionDataProvider1(testService1));

            // Add test 2 services
            MixedRealityToolkit.TryRegisterService<ITestExtensionService2>(new TestExtensionService2());
            var testService2 = MixedRealityToolkit.GetService<ITestExtensionService2>();
            MixedRealityToolkit.TryRegisterService<ITestExtensionDataProvider2>(new TestExtensionDataProvider2(testService2));

            // Enable all test services
            MixedRealityToolkit.EnableAllServicesOfType<ITestService>();

            // Get all services
            var testServices = MixedRealityToolkit.GetActiveServices<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service != null);
                Assert.IsTrue(service.IsEnabled);
            }

            // Enable all test services
            MixedRealityToolkit.DisableAllServiceOfType<ITestService>();

            foreach (var service in testServices)
            {
                Assert.IsTrue(service != null);
                Assert.IsFalse(service.IsEnabled);
            }
        }

        #endregion Service Enable/Disable Tests

        #region Mixed Reality System Tests

        [Test]
        public void Test_08_01_TestSystemsCannotBeRegisteredTwice()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(false);

            var testProfile = ScriptableObject.CreateInstance<TestSystemProfile>();

            // Register test system 1
            var testSystem1Success = MixedRealityToolkit.TryRegisterService<ITestSystem>(new TestSystem1(testProfile));
            var testGetSystem1Success = MixedRealityToolkit.TryGetSystem<ITestSystem>(out var testSystem1);

            Assert.IsTrue(testSystem1 != null);
            Assert.IsTrue(testSystem1Success);
            Assert.IsTrue(testGetSystem1Success);

            // Register test system 2
            var testSystem2Success = MixedRealityToolkit.TryRegisterService<ITestSystem>(new TestSystem2(testProfile));

            LogAssert.Expect(LogType.Error, $"There's already a {nameof(ITestSystem)}.{nameof(TestSystem1)} registered!");
            Assert.IsFalse(testSystem2Success);
        }

        #endregion Mixed Reality System Tests

        #region TearDown

        [TearDown]
        public void CleanupMixedRealityToolkitTests()
        {
            TestUtilities.CleanupScene();
        }

        #endregion TearDown
    }
}
