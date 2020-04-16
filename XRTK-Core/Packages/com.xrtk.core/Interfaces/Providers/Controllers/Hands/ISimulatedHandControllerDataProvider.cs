// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using XRTK.Definitions.Controllers.Hands;

namespace XRTK.Interfaces.Providers.Controllers.Hands
{
    public interface ISimulatedHandControllerDataProvider : ISimulatedControllerDataProvider, IMixedRealityHandControllerDataProvider
    {
        /// <summary>
        /// Gets configured simulated hand controller pose definitions used to simulate
        /// different hand poses.
        /// </summary>
        IReadOnlyList<HandControllerPoseDefinition> HandPoseDefinitions { get; }

        /// <summary>
        /// Gets the simulated hand controller pose animation speed controlling
        /// how fast the hand will translate from one pose to another.
        /// </summary>
        float HandPoseAnimationSpeed { get; }
    }
}