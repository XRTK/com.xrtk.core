// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement for hand mesh information.
    /// </summary>
    public interface IMixedRealityHandMeshHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a hand mesh is updated, this handler receives the event.
        /// </summary>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnMeshUpdated(InputEventData<HandMeshData> eventData);
    }
}