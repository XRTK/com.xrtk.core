// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityControllerDataProvider"/>s
    /// </summary>
    public abstract class BaseControllerDataProvider : BaseDataProvider, IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Creates a new instance of the data provider.
        /// </summary>
        /// <param name="name">Name of the data provider as assigned in the configuration profile.</param>
        /// <param name="priority">Data provider priority controls the order in the service registry.</param>
        /// <param name="profile">Controller data provider profile assigned to the provider instance in the configuration inspector.</param>
        protected BaseControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile)
            : base(name, priority)
        {
            if (profile == null)
            {
                throw new UnassignedReferenceException($"A {nameof(profile)} is required for {name}");
            }

            controllerMappingProfiles = profile.ControllerMappingProfiles;
        }

        private readonly MixedRealityControllerMappingProfile[] controllerMappingProfiles;

        private readonly List<IMixedRealityController> activeControllers = new List<IMixedRealityController>();

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityController> ActiveControllers => activeControllers;

        /// <inheritdoc />
        public MixedRealityControllerMappingProfile GetControllerMappingProfile(Type controllerType, Handedness handedness)
        {
            if (controllerType == null)
            {
                Debug.LogError($"{nameof(controllerType)} is null!");
                return null;
            }

            if (!typeof(IMixedRealityController).IsAssignableFrom(controllerType))
            {
                Debug.LogError($"{controllerType.Name} does not implement {nameof(IMixedRealityController)}");
                return null;
            }

            // TODO provide a way to choose profiles with additional args instead of returning the first one found.

            for (int i = 0; i < controllerMappingProfiles.Length; i++)
            {
                if (handedness == controllerMappingProfiles[i].Handedness &&
                    controllerMappingProfiles[i].ControllerType?.Type == controllerType)
                {
                    return controllerMappingProfiles[i];
                }
            }

            return null;
        }

        protected void AddController(IMixedRealityController controller)
        {
            activeControllers.Add(controller);
        }

        protected void RemoveController(IMixedRealityController controller)
        {
            if (controller != null)
            {
                activeControllers.Remove(controller);
            }
        }
    }
}