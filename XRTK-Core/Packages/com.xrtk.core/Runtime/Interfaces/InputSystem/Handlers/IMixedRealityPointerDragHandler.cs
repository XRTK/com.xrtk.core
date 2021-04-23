// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to pointer drag input.
    /// </summary>
    public interface IMixedRealityPointerDragHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer drag begin event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDragBegin(MixedRealityPointerDragEventData eventData);

        /// <summary>
        /// When a pointer dragged event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDrag(MixedRealityPointerDragEventData eventData);

        /// <summary>
        /// When a pointer drag end event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerDragEnd(MixedRealityPointerDragEventData eventData);
    }
}