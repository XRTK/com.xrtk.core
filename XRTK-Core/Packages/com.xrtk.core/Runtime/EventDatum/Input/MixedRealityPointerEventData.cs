// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes a pointer event that involves a tap, click, or touch.
    /// </summary>
    public class MixedRealityPointerEventData : BaseInputEventData
    {
        /// <summary>
        /// Pointer for the Input Event
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <inheritdoc />
        public MixedRealityPointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="context"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityPointer pointer, InputAction.CallbackContext context, IMixedRealityInputSource inputSource = null)
        {
            BaseInitialize(inputSource ?? pointer.InputSourceParent, context);
            Pointer = pointer;
        }
    }
}
