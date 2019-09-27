// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityNativeDataProvider"/> and provides default properties for all native data providers.
    /// </summary>
    public abstract class BaseNativeDataProvider : BaseDataProvider, IMixedRealityNativeDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        protected BaseNativeDataProvider(string name, uint priority)
            : base(name, priority) { }

        /// <inheritdoc />
        public virtual string LibraryName { get; } = "__Internal";
    }
}