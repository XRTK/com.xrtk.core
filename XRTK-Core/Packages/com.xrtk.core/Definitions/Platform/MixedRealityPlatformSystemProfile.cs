// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.PlatformSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Platform System Profile", fileName = "MixedRealityPlatformSystemProfile", order = (int)CreateProfileMenuItemIndices.Platform)]
    public class MixedRealityPlatformSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private PlatformConfiguration[] platformConfigurations = new PlatformConfiguration[0];

        /// <summary>
        /// The currently registered <see cref="PlatformConfiguration"/>s for the application.
        /// </summary>
        public PlatformConfiguration[] PlatformConfigurations
        {
            get => platformConfigurations;
            internal set => platformConfigurations = value;
        }
    }
}