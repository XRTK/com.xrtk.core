// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions;
using XRTK.Interfaces;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestSystemProfile : BaseMixedRealityServiceProfile<ITestDataProvider1>
    {
    }

    internal class TestSystem1 : BaseSystem, ITestSystem
    {
        private readonly HashSet<IMixedRealityDataProvider> dataProviders = new HashSet<IMixedRealityDataProvider>();

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityDataProvider> DataProviders => dataProviders;

        /// <inheritdoc />
        public TestSystem1(TestSystemProfile profile) : base(profile)
        {
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

    internal class TestSystem2 : BaseSystem, ITestSystem
    {
        /// <inheritdoc />
        public TestSystem2(TestSystemProfile profile) : base(profile)
        {
        }

        public IReadOnlyCollection<IMixedRealityDataProvider> DataProviders => throw new System.NotImplementedException();

        public bool RegisterDataProvider(ITestDataProvider1 dataProvider)
        {
            throw new System.NotImplementedException();
        }

        public bool UnRegisterDataProvider(ITestDataProvider1 dataProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}
