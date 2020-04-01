// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// Abstract base implementation for diagnostics data providers. Provides needed implementations to register and unregister
    /// diagnostics handlers.
    /// </summary>
    public abstract class BaseMixedRealityDiagnosticsDataProvider : BaseDataProvider, IMixedRealityDiagnosticsDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseMixedRealityDiagnosticsDataProvider(string name, uint priority, BaseMixedRealityProfile profile)
            : base(name, priority)
        {
        }
    }
}