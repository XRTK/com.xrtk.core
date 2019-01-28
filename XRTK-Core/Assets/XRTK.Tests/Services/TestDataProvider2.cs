// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestDataProvider2 : BaseServiceWithConstructor, ITestDataProvider2
    {
        public TestDataProvider2(string name, uint priority = 10) : base(name, priority) { }

        public bool IsEnabled { get; private set; }

        public override void Enable()
        {
            base.Enable();
            IsEnabled = true;
        }

        public override void Disable()
        {
            base.Disable();
            IsEnabled = false;
        }
    }
}