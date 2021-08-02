// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;

namespace XRTK.Tests.Services
{
    internal interface ITestService1 : IMixedRealityService
    {
        bool IsEnabled { get; }
    }
}
