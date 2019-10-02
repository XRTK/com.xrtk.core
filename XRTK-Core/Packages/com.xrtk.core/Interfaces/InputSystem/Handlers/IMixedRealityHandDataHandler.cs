// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to get hand data updates.
    /// </summary>
    public interface IMixedRealityHandDataHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a hand is updated, the handler receives this notification.
        /// </summary>
        /// <param name="eventData">Contains information about the updated hand data.</param>
        void OnHandDataUpdated(InputEventData<HandData> eventData);
    }
}