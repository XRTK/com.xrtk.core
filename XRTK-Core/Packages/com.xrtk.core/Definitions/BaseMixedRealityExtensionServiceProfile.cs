// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    /// <summary>
    /// The base profile to use for custom <see cref="IMixedRealityExtensionService"/>s
    /// </summary>
    public abstract class BaseMixedRealityExtensionServiceProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Currently registered IMixedRealityDataProvider configurations for this extension service.")]
        private MixedRealityServiceConfiguration[] registeredDataProviders = new MixedRealityServiceConfiguration[0];

        /// <summary>
        /// Currently registered <see cref="IMixedRealityDataProvider"/> configurations for this extension service.
        /// </summary>
        public MixedRealityServiceConfiguration[] RegisteredDataProviders
        {
            get => registeredDataProviders;
            internal set => registeredDataProviders = value;
        }
    }
}