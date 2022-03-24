﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Basic interface for synchronizing to a controller pose.
    /// </summary>
    public interface IMixedRealityControllerPoseSynchronizer : IMixedRealitySourcePoseHandler,
        IMixedRealityInputHandler,
        IMixedRealityInputHandler<float>,
        IMixedRealityInputHandler<Vector2>,
        IMixedRealityInputHandler<Vector3>,
        IMixedRealityInputHandler<Quaternion>,
        IMixedRealityInputHandler<MixedRealityPose>
    {
        /// <summary>
        /// The <see cref="Transform"/> that will be synchronized with the controller data.
        /// </summary>
        /// <remarks>
        /// Defaults to the <see cref="Transform"/> that this component is attached to.
        /// </remarks>
        Transform PoseDriver { get; set; }

        /// <summary>
        /// The controller handedness to synchronize with.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// Should this <see cref="GameObject"/> clean itself up when it's controller is lost?
        /// </summary>
        /// <remarks>It's up to the implementation to properly destroy the <see cref="GameObject"/>'s this interface will implement.</remarks>
        bool DestroyOnSourceLost { get; set; }

        /// <summary>
        /// The current controller reference.
        /// </summary>
        IMixedRealityController Controller { get; set; }

        /// <summary>
        /// Should the Transform's position be driven from the source pose or from input handler?
        /// </summary>
        bool UseSourcePoseData { get; set; }

        /// <summary>
        /// The input action that will drive the Transform's pose, position, or rotation.
        /// </summary>
        MixedRealityInputAction PoseAction { get; set; }
    }
}
