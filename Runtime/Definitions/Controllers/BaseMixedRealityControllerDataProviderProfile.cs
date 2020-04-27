// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

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
        private MixedRealityControllerMappingProfile[] controllerMappingProfiles = new MixedRealityControllerMappingProfile[0];

        public MixedRealityControllerMappingProfile[] ControllerMappingProfiles
        {
            get => controllerMappingProfiles;
            internal set => controllerMappingProfiles = value;
        }

        public abstract ControllerDefinition[] GetDefaultControllerOptions();
    }
}