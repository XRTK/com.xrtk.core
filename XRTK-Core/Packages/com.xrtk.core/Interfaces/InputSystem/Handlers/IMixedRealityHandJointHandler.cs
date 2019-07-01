// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine.EventSystems;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement for hand joint information.
    /// </summary>
    public interface IMixedRealityHandJointHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a join is updated, this handler receives the event.
        /// </summary>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnJointUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData);
    }
}