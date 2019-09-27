// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Utilities;

namespace XRTK.Services
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Native Library System Profile", fileName = "NativeLibrariesConfigurationProfile", order = (int)CreateProfileMenuItemIndices.NativeLibraryDataProviders)]
    public class NativeLibrarySystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private NativeDataModelConfiguration[] nativeDataModelConfigurations = new NativeDataModelConfiguration[0];

        /// <summary>
        /// Currently registered native library data models.
        /// </summary>
        public NativeDataModelConfiguration[] NativeDataModelConfigurations
        {
            get => nativeDataModelConfigurations;
            internal set => nativeDataModelConfigurations = value;
        }
    }
}