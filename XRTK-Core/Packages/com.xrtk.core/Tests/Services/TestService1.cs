// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestService1 : BaseServiceWithConstructor, ITestService
    {
        private readonly HashSet<IMixedRealityDataProvider> dataProviders = new HashSet<IMixedRealityDataProvider>();

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityDataProvider> DataProviders => dataProviders;

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

        public bool RegisterDataProvider(ITestDataProvider1 dataProvider)
        {
            if (dataProviders.Contains(dataProvider))
            {
                return false;
            }

            dataProviders.Add(dataProvider);
            return true;
        }

        public bool UnRegisterDataProvider(ITestDataProvider1 dataProvider)
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
