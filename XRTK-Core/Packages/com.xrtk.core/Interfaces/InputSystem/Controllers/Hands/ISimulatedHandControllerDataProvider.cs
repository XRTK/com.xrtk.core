// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Interfaces.InputSystem.Controllers.Hands
{
    public interface ISimulatedControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// The simulated update frequency in milliseconds mimics the hardware's ability to
        /// update controller tracking data. A value of 0ms will provide data
        /// updates every frame.
        /// </summary>
        double SimulatedUpdateFrequency { get; }

        /// <summary>
        /// Time after which uncontrolled controllers are hidden
        /// </summary>
        float ControllerHideTimeout { get; }

        /// <summary>
        /// Default distance of the controller from the camera.
        /// </summary>
        float DefaultDistance { get; }

        /// <summary>
        /// Depth change when scrolling the mouse wheel.
        /// </summary>
        float DepthMultiplier { get; }

        /// <summary>
        /// Apply random offset to the controller position.
        /// </summary>
        float JitterAmount { get; }

        /// <summary>
        /// Key to toggle persistent mode for the left controller.
        /// </summary>
        KeyCode ToggleLeftPersistentKey { get; }

        /// <summary>
        /// Key to simulate tracking of the left controller.
        /// </summary>
        KeyCode LeftControllerTrackedKey { get; }

        /// <summary>
        /// Key to toggle persistent mode for the right controller
        /// </summary>
        KeyCode ToggleRightPersistentKey { get; }

        /// <summary>
        /// Key to simulate tracking of the right controller.
        /// </summary>
        KeyCode RightControllerTrackedKey { get; }

        /// <summary>
        /// Angle per second when rotating the controller.
        /// </summary>
        float RotationSpeed { get; }
    }

    public interface ISimulatedHandControllerDataProvider : ISimulatedControllerDataProvider
    {
        /// <summary>
        /// Gets configured simulated hand controller pose definitions used to simulate
        /// different hand poses.
        /// </summary>
        IReadOnlyList<SimulatedHandControllerPoseData> HandPoseDefinitions { get; }

        /// <summary>
        /// Gets the simulated hand controller pose animation speed controlling
        /// how fast the hand will translate from one pose to another.
        /// </summary>
        float HandPoseAnimationSpeed { get; }
    }
}