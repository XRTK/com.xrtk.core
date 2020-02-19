// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;

namespace XRTK.Tests.Services
{
    internal class TestExtensionServiceProvider1 : TestExtensionService1, ITestExtensionServiceProvider1
    {
        public TestExtensionServiceProvider1(string name, uint priority = 10, MixedRealityRegisteredServiceProvidersProfile profile = null) : base(name, priority, profile)
        {
        }
    }
}