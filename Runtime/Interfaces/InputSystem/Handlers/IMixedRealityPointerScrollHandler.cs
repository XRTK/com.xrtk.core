// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to pointer scroll input.
    /// </summary>
    public interface IMixedRealityPointerScrollHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer scroll is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerScroll(MixedRealityPointerScrollEventData eventData);
    }
}