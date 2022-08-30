// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base controller data provider to inherit from when implementing <see cref="IMixedRealityControllerDataProvider"/>s
    /// </summary>
    public abstract class BaseControllerDataProvider : BaseDataProvider, IMixedRealityControllerDataProvider
    {
        /// <inheritdoc />
        protected BaseControllerDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (profile.IsNull())
            {
                throw new UnassignedReferenceException($"A {nameof(profile)} is required for {name}");
            }

            controllerProfiles = profile.ControllerProfiles;

            if (controllerProfiles == null ||
                controllerProfiles.Length == 0)
            {
                throw new UnassignedReferenceException($"{nameof(controllerProfiles)} has no defined controller mappings for {name}");
            }

            InputSystem = parentService;
        }

        protected readonly IMixedRealityInputSystem InputSystem;

        private readonly MixedRealityControllerProfile[] controllerProfiles;

        private readonly List<IMixedRealityController> activeControllers = new List<IMixedRealityController>();

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealityController> ActiveControllers => activeControllers;

        /// <inheritdoc />
        public MixedRealityControllerProfile GetControllerProfile(Type controllerType, Handedness handedness)
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

            for (int i = 0; i < controllerProfiles.Length; i++)
            {
                if (handedness == controllerProfiles[i].Handedness &&
                    controllerProfiles[i].ControllerType?.Type == controllerType)
                {
                    return controllerProfiles[i];
                }
            }

            Debug.LogError($"Failed to find a controller mapping for {controllerType.Name} with with handedness: {handedness}");
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
