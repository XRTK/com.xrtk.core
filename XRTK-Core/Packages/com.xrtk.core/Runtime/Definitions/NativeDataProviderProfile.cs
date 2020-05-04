// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;

namespace XRTK.Services
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Native Data Provider Profile", fileName = "NativeDataProviderProfile", order = (int)CreateProfileMenuItemIndices.NativeLibraryDataProviders)]
    public class NativeDataProviderProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The project relative path to the dll.\nOnly used at edit time to find and load the native library.")]
        private string dllPath = string.Empty;

        /// <summary>
        /// The project relative path to the dll.
        /// </summary>
        /// <remarks>
        /// Only used at edit time to find and load the native library.
        /// </remarks>
        public string DllPath => dllPath;
    }
}