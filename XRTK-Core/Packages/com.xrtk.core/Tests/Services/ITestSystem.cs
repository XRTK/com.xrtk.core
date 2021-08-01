// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Interfaces;

namespace XRTK.Tests.Services
{
    internal interface ITestSystem : IMixedRealitySystem
    {
        /// <summary>
        /// The list of <see cref="IMixedRealityDataProvider"/>s registered and running with the system.
        /// </summary>
        IReadOnlyCollection<IMixedRealityDataProvider> DataProviders { get; }

        /// <summary>
        /// Registers the <see cref="ITestDataProvider1"/> with the <see cref="ITestService"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        bool RegisterDataProvider(ITestDataProvider1 dataProvider);

        /// <summary>
        /// UnRegisters the <see cref="ITestDataProvider1"/> with the <see cref="ITestService"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        bool UnRegisterDataProvider(ITestDataProvider1 dataProvider);
    }
}
