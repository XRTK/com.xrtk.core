// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Interfaces.Providers.Controllers
{
    public interface IMixedRealitySimulatedController : IMixedRealityController
    {
        /// <summary>
        /// Gets a simulated Yaw, Pitch and Roll delta for the current frame.
        /// </summary>
        /// <param name="rotationSpeed">The speed at which to rotate the simulated controller.</param>
        /// <returns>Updated controller rotation angles.</returns>
        Vector3 GetDeltaRotation(float rotationSpeed);

        /// <summary>
        /// Gets a simulated depth tracking (controller closer / further from tracking device) update, as well
        /// as the controller's simulated (x,y) position.
        /// </summary>
        /// <param name="depthMultiplier">Multiplier for z-axis movement.</param>
        /// <returns>Controller movement delta.</returns>
        Vector3 GetDeltaPosition(float depthMultiplier);
    }
}