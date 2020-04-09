// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit data provider definition, used to instantiate and manage controllers and joysticks
    /// </summary>
    public interface IMixedRealityControllerDataProvider : IMixedRealityInputDataProvider
    {
        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required)
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMixedRealityController> ActiveControllers { get; }

        /// <summary>
        /// Gets the registered controller mapping profile for the provided <see cref="IMixedRealityController"/>
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="handedness"></param>
        /// <returns></returns>
        /// <remarks>
        /// Currently you can register more than one controller type and handedness into the
        /// <see cref="BaseMixedRealityControllerDataProviderProfile"/>, but this method will only return the first one found.
        /// </remarks>
        MixedRealityControllerMappingProfile GetControllerMappingProfile(Type controllerType, Handedness handedness);
    }
}