// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestService1 : BaseServiceWithConstructor, ITestService
    {
        public TestService1(string name = "Test Service 1", uint priority = 0)
            : base(name, priority)
        {
        }

        /// <inheritdoc />
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