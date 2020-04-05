// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityHandController"/>s.
    /// </summary>
    public abstract class BaseHandDataProvider : BaseControllerDataProvider
    {
        protected BaseHandDataProvider(string name, uint priority, BaseHandDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }
    }
}