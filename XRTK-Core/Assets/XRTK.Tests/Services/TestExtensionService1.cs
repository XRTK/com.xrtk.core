// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestExtensionService1 : BaseExtensionService, ITestExtensionService1
    {
        public TestExtensionService1(string name, uint priority = 10, BaseMixedRealityExtensionServiceProfile profile = null) : base(name, priority, profile) { }

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

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}