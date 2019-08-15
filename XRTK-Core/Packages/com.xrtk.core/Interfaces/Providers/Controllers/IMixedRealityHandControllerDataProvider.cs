// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Get a game object following the hand joint.
        /// </summary>
        Transform RequestJointTransform(TrackedHandJoint joint, Handedness handedness);

        /// <summary>
        /// Gets whether the specified hand is currently being tracked.
        /// </summary>
        /// <param name="handedness">Hand to get tracked state for.</param>
        /// <returns>True if specified hand is tracked.</returns>
        bool IsHandTracked(Handedness handedness);

        void RaiseHandJointsUpdated(Interfaces.InputSystem.IMixedRealityInputSource source, Handedness handedness, System.Collections.Generic.IDictionary<TrackedHandJoint, MixedRealityPose> jointPoses);

        void RaiseHandMeshUpdated(Interfaces.InputSystem.IMixedRealityInputSource source, Handedness handedness, XRTK.Providers.Controllers.Hands.HandMeshUpdatedEventData handMeshInfo);

        void Register(GameObject listener);

        void Unregister(GameObject listener);
    }
}