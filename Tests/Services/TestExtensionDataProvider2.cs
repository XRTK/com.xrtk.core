// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestExtensionDataProvider2 : BaseExtensionDataProvider, ITestExtensionDataProvider2
    {
        public TestExtensionDataProvider2(ITestExtensionService2 parentService, string name = "Test Extension Data Provider 2", uint priority = 10, BaseMixedRealityExtensionDataProviderProfile profile = null)
            : base(name, priority, profile, parentService)
        {
        }

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