// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The <see cref="IMixedRealityService"/> in charge of managing native library data providers.
    /// </summary>
    public class NativeLibrarySystem : BaseSystem, IMixedRealityNativeLibrarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public NativeLibrarySystem(NativeLibrarySystemProfile profile) : base(profile)
        {
        }

        #region IMixedRealityNativeLibrarySystem Implementation

#if UNITY_EDITOR

        private readonly Dictionary<IntPtr, BaseNativeDataProvider> nativeLibraries = new Dictionary<IntPtr, BaseNativeDataProvider>();

#if UNITY_EDITOR_WIN

        private const string Kernel32 = "kernel32";

        [DllImport(Kernel32)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport(Kernel32)]
        private static extern IntPtr GetProcAddress(IntPtr libraryHandle, string symbolName);

        [DllImport(Kernel32)]
        private static extern bool FreeLibrary(IntPtr libraryHandle);

#else // linux based

        private const string Internal = "__Internal";

        [DllImport(Internal)]
        private static extern IntPtr dlopen(string path, int flag);
     
        [DllImport(Internal)]
        private static extern IntPtr dlsym(IntPtr handle, string symbolName);
     
        [DllImport(Internal)]
        private static extern int dlclose(IntPtr handle);

#endif

        /// <inheritdoc />
        public IntPtr OpenLibrary(string path)
        {
#if UNITY_EDITOR_WIN
            var handle = LoadLibrary(path);
#else // linux based
            IntPtr handle = dlopen(path, 0);
#endif

            if (handle == IntPtr.Zero)
            {
                throw new Exception($"Couldn't open native library: {path}");
            }

            nativeLibraries.Add(handle, null);
            return handle;
        }

        /// <inheritdoc />
        public void CloseLibrary(IntPtr libraryHandle)
        {
            nativeLibraries.Remove(libraryHandle);

#if UNITY_EDITOR_WIN
            FreeLibrary(libraryHandle);
#else // linux based
            dlclose(libraryHandle);
#endif
        }

#endif // UNITY_EDITOR
    }

    #endregion IMixedRealityNativeLibrarySystem Implementation
}