// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;

namespace XRTK.Interfaces.Providers.Controllers.Hands
{
    /// <summary>
    /// Controller definition, used to manage a hand controller.
    /// </summary>
    public interface IMixedRealityHandController : IMixedRealityController
    {
        /// <summary>
        /// Get the hands bounds of a given type, if they are available.
        /// </summary>
        /// <param name="handBounds">The requested hand bounds.</param>
        /// <param name="bounds">The bounds if available.</param>
        /// <returns>True, if bounds available.</returns>
        bool TryGetBounds(TrackedHandBounds handBounds, out Bounds[] bounds);

        /// <summary>
        /// Get the current pose of a joint of the hand.
        /// </summary>
        /// <remarks>
        /// Hand bones should be oriented along the Z-axis, with the Y-axis indicating the "up" direction,
        /// i.e. joints rotate primarily around the X-axis.
        /// </remarks>
        bool TryGetJointPose(TrackedHandJoint joint, out MixedRealityPose pose);
    }
}