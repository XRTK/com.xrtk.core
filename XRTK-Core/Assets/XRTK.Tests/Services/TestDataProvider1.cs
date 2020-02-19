// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestDataProvider1 : BaseServiceWithConstructor, ITestDataProvider1
    {
        public TestDataProvider1(string name, uint priority = 10) : base(name, priority) { }

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