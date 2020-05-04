// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Interfaces
{
    public interface IMixedRealityNativeLibrarySystem : IMixedRealityService
    {
#if UNITY_EDITOR

        /// <summary>
        /// Opens a library at the path specified.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IntPtr OpenLibrary(string path);

        /// <summary>
        /// Closes a library using the specified handle.
        /// </summary>
        /// <param name="libraryHandle"></param>
        void CloseLibrary(IntPtr libraryHandle);

#endif
    }
}