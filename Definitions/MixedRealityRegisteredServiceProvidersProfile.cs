// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Registered Service Providers Profile", fileName = "MixedRealityRegisteredServiceProvidersProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class MixedRealityRegisteredServiceProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = new MixedRealityServiceConfiguration[0];

        /// <summary>
        /// Currently registered system and manager configurations.
        /// </summary>
        public MixedRealityServiceConfiguration[] Configurations
        {
            get => configurations;
            internal set => configurations = value;
        }
    }
}