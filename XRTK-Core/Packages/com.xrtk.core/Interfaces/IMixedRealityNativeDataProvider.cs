// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces
{
    /// <summary>
    /// Generic interface for native c++ data providers
    /// </summary>
    public interface IMixedRealityNativeDataProvider : IMixedRealityDataProvider
    {
#if UNITY_EDITOR

        /// <summary>
        /// The relative path to the library.
        /// </summary>
        string LibraryPath { get; }

#endif

        /// <summary>
        /// The name of the native library to load.
        /// </summary>
        string LibraryName { get; }
    }
}