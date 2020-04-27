// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes a <see cref="MixedRealityPointerEventData"/> with dragging data.
    /// </summary>
    public class MixedRealityPointerDragEventData : MixedRealityPointerEventData
    {
        /// <summary>
        /// The distance this pointer has been dragged since the last event was raised.
        /// </summary>
        public Vector3 DragDelta { get; private set; }

        /// <inheritdoc />
        public MixedRealityPointerDragEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="dragDelta"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, Vector3 dragDelta, IMixedRealityInputSource inputSource = null)
        {
            Initialize(pointer, inputAction, inputSource);
            DragDelta = dragDelta;
        }
    }
}