// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Interfaces;

namespace XRTK.Tests.Services
{
    internal interface ITestExtensionService1 : IMixedRealityExtensionService
    {
        /// <summary>
        /// The list of <see cref="IMixedRealityExtensionDataProvider"/>s registered and running with the system.
        /// </summary>
        IReadOnlyCollection<IMixedRealityExtensionDataProvider> DataProviders { get; }

        /// <summary>
        /// Registers the <see cref="ITestExtensionDataProvider1"/> with the <see cref="ITestService"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        bool RegisterDataProvider(ITestExtensionDataProvider1 dataProvider);

        /// <summary>
        /// UnRegisters the <see cref="ITestExtensionDataProvider1"/> with the <see cref="ITestService"/>.
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        bool UnRegisterDataProvider(ITestExtensionDataProvider1 dataProvider);
    }
}
