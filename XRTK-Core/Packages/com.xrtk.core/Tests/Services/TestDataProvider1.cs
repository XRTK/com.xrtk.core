// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestDataProvider1 : BaseDataProvider, ITestDataProvider1
    {
        public TestDataProvider1(ITestService parentService, string name = "Test Data Provider 1", uint priority = 1, BaseMixedRealityProfile profile = null)
            : base(name, priority, profile, parentService)
        {
            testSystem = parentService;
        }

        private readonly ITestService testSystem = null;

        public bool IsEnabled { get; private set; }

        public override void Enable()
        {
            IsEnabled = true;
        }

        public override void Disable()
        {
            IsEnabled = false;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            testSystem.RegisterDataProvider(this);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            testSystem.UnRegisterDataProvider(this);
        }
    }
}
