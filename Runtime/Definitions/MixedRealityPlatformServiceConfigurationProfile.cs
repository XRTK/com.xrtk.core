// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Platform Service Configurations", fileName = "MixedRealityPlatformServiceConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class MixedRealityPlatformServiceConfigurationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private RuntimePlatformEntry platformEntries = new RuntimePlatformEntry();

        public RuntimePlatformEntry PlatformEntries => platformEntries;

        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        public MixedRealityServiceConfiguration[] Configurations => configurations;
    }
}