﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IMixedRealityFocusHandler : IEventSystemHandler
    {
        /// <summary>
        /// The Focus Enter event is raised on this <see cref="UnityEngine.GameObject"/> whenever a <see cref="IMixedRealityPointer"/>'s focus enters this <see cref="UnityEngine.GameObject"/>'s <see cref="UnityEngine.Collider"/>.
        /// </summary>
        /// <param name="eventData"></param>
        void OnFocusEnter(FocusEventData eventData);

        /// <summary>
        /// The Focus Exit event is raised on this <see cref="UnityEngine.GameObject"/> whenever a <see cref="IMixedRealityPointer"/>'s focus leaves this <see cref="UnityEngine.GameObject"/>'s <see cref="UnityEngine.Collider"/>.
        /// </summary>
        /// <param name="eventData"></param>
        void OnFocusExit(FocusEventData eventData);
    }
}
