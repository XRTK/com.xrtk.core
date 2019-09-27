// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
        /// <param name="profile"></param>
        protected BaseNativeDataProvider(string name, uint priority, NativeDataProviderProfile profile)
            : base(name, priority)
        {
            if (profile == null)
            {
                throw new ArgumentException($"Missing the native data provider profile for {base.Name}!");
            }

#if UNITY_EDITOR
            LibraryPath = profile.DllPath;
#endif
        }

#if UNITY_EDITOR

        /// <inheritdoc />
        public string LibraryPath { get; }

#endif

        /// <inheritdoc />
        public virtual string LibraryName { get; } = "__Internal";
    }
}