// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The base extension service implements <see cref="IMixedRealityExtensionService"/> and provides default properties for all extension services.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityExtensionService"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseExtensionService : BaseServiceWithConstructor, IMixedRealityExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public BaseExtensionService(string name, uint priority, BaseMixedRealityExtensionServiceProfile profile) : base(name, priority) { }
    }
}
