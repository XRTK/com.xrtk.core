// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal interface ITestExtensionServiceProvider1 : ITestDataProvider1, ITestExtensionService1 { }

    internal class TestExtensionServiceProvider1 : TestExtensionService1, ITestExtensionServiceProvider1
    {
        public TestExtensionServiceProvider1(string name) : base(name)
        {
        }
    }

    internal class TestDataProvider1 : BaseServiceWithConstructor, ITestDataProvider1
    {
        public TestDataProvider1(string name) : base(name, 5) { }

        public bool IsEnabled { get; private set; }

        public override void Enable()
        {
            IsEnabled = true;
        }

        public override void Disable()
        {
            IsEnabled = false;
        }
    }
}