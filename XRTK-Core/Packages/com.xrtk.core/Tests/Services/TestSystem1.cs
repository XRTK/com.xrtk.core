// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Services;

namespace XRTK.Tests.Services
{
    internal class TestSystemProfile : BaseMixedRealityProfile
    {
    }

    internal class TestSystem1 : BaseSystem, ITestSystem
    {
        /// <inheritdoc />
        public TestSystem1(TestSystemProfile profile) : base(profile)
        {
        }
    }

    internal class TestSystem2 : BaseSystem, ITestSystem
    {
        /// <inheritdoc />
        public TestSystem2(TestSystemProfile profile) : base(profile)
        {
        }
    }
}