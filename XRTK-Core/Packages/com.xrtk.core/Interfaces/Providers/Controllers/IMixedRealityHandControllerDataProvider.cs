// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem.Handlers;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Gets a transform following the hand joint.
        /// </summary>
        /// <param name="joint">The joint to find the transform for.</param>
        /// <param name="handedness">Handedness of the hand the joint belongs to.</param>
        /// <param name="jointTransform">If found, the transform following the hand joint.</param>
        /// <returns>True, if the joint transform was found.</returns>
        bool TryGetJointTransform(TrackedHandJoint joint, Handedness handedness, out Transform jointTransform);

        /// <summary>
        /// Gets whether the specified hand is currently being tracked.
        /// </summary>
        /// <param name="handedness">Hand to get tracked state for.</param>
        /// <returns>True if specified hand is tracked.</returns>
        bool IsHandTracked(Handedness handedness);

        /// <summary>
        /// Registers a hand joint update handler. The handler will then
        /// receive updates.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        void Register(IMixedRealityHandJointHandler handler);

        /// <summary>
        /// Unregisters a previosuly registered hand joint update handler.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        void Unregister(IMixedRealityHandJointHandler handler);

        /// <summary>
        /// Registers a hand mesh update handler. The handler will then
        /// receive updates.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        void Register(IMixedRealityHandMeshHandler handler);

        /// <summary>
        /// Unregisters a previosuly registered hand mesh update handler.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        void Unregister(IMixedRealityHandMeshHandler handler);
    }
}