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

        // TODO Move all this into the inspector's OnEnable method
        protected virtual void Awake()
        {
            if (!hasSetupDefaults)
            {
                var controllerDefinitions = GetControllerDefinitions();
                controllerMappingProfiles = new MixedRealityControllerMappingProfile[controllerDefinitions.Length];

                for (int i = 0; i < controllerMappingProfiles.Length; i++)
                {
                    var instance = CreateInstance(nameof(MixedRealityControllerMappingProfile)) as MixedRealityControllerMappingProfile;
                    Debug.Assert(instance != null);
                    // TODO Need to create the asset and save it to disk.
                    instance.ControllerType = controllerDefinitions[i].ControllerType;
                    instance.Handedness = controllerDefinitions[i].Handedness;
                    instance.UseCustomInteractions = controllerDefinitions[i].UseCustomInteractions;
                    instance.SetDefaultInteractionMapping();
                    controllerMappingProfiles[i] = instance;
                }

                hasSetupDefaults = true;
            }
        }

        public abstract ControllerDefinition[] GetControllerDefinitions();
    }
}