// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRTK.Definitions.Controllers
{
    /// <summary>
    /// Provides additional configuration options for controller data providers.
    /// </summary>
    public abstract class BaseMixedRealityControllerDataProviderProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private bool hasSetupDefaults = false;

        /// <summary>
        /// Has the default mappings been initialized?
        /// </summary>
        protected bool HasSetupDefaults => hasSetupDefaults;

        [SerializeField]
        [FormerlySerializedAs("controllerMappingProfiles")]
        private MixedRealityControllerProfile[] controllerProfiles = Array.Empty<MixedRealityControllerProfile>();

        public MixedRealityControllerProfile[] ControllerProfiles
        {
            get => controllerProfiles;
            internal set => controllerProfiles = value;
        }

        public abstract ControllerDefinition[] GetDefaultControllerOptions();
    }
}
