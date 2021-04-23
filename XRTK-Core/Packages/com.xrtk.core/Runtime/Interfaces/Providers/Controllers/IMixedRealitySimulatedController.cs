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
        /// Gets a simulated root position in camera space for the simulated controller.
        /// </summary>
        /// <param name="depthMultiplier">Multiplier for z-axis movement.</param>
        /// <returns>Controller root position.</returns>
        Vector3 GetPosition(float depthMultiplier);
    }
}