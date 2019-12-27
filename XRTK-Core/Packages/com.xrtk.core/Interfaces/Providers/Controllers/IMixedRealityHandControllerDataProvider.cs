// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Interfaces.Providers.Controllers
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityHandControllerDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Gets the pose of a hand joint, if it's currently tracked.
        /// </summary>
        /// <param name="joint">The joint to find the pose for.</param>
        /// <param name="handedness">Handedness of the hand the joint belongs to.</param>
        /// <param name="pose">If found, the pose following the hand joint.</param>
        /// <returns>True, if the joint pose was found.</returns>
        bool TryGetJointPose(Handedness handedness, TrackedHandJoint joint, out MixedRealityPose pose);

        /// <summary>
        /// Gets whether the specified hand is currently being tracked.
        /// </summary>
        /// <param name="handedness">Hand to get tracked state for.</param>
        /// <returns>True if specified hand is tracked.</returns>
        bool IsHandTracked(Handedness handedness);
    }
}