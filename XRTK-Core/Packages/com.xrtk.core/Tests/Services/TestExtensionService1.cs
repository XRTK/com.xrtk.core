// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestExtensionService1 : BaseExtensionService, ITestExtensionService1
    {
        private readonly HashSet<IMixedRealityExtensionDataProvider> dataProviders = new HashSet<IMixedRealityExtensionDataProvider>();

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityExtensionDataProvider> DataProviders => dataProviders;

        public TestExtensionService1(string name = "Test Extension Service 1", uint priority = 10, BaseMixedRealityExtensionServiceProfile profile = null)
            : base(name, priority, profile)
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

        public override void Destroy()
        {
            base.Destroy();
        }

        public bool RegisterDataProvider(ITestExtensionDataProvider1 dataProvider)
        {
            if (dataProviders.Contains(dataProvider))
            {
                return false;
            }

            dataProviders.Add(dataProvider);
            return true;
        }

        public bool UnRegisterDataProvider(ITestExtensionDataProvider1 dataProvider)
        {
            if (dataProviders.Contains(dataProvider))
            {
                dataProviders.Remove(dataProvider);
                return true;
            }
            return false;
        }
    }
}
