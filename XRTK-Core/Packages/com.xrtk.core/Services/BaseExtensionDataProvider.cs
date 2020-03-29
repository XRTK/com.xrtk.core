// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The base extension data provider implements <see cref="IMixedRealityExtensionDataProvider"/> and provides default properties for all extension data providers.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityExtensionDataProvider"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseExtensionDataProvider : BaseServiceWithConstructor, IMixedRealityExtensionDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public BaseExtensionDataProvider(string name, uint priority, BaseMixedRealityExtensionDataProviderProfile profile) : base(name, priority) { }
    }
}