// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that involves a tap, click, or touch.
    /// </summary>
    public class MixedRealityPointerEventData : BaseInputEventData
    {
        /// <summary>
        /// Pointer for the Input Event
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <summary>
        /// Number of Clicks, Taps, or Presses that triggered the event.
        /// </summary>
        public int Count { get; private set; }

        /// <inheritdoc />
        public MixedRealityPointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputSource"></param>
        /// <param name="count"></param>
        public void Initialize(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null, int count = 0)
        {
            BaseInitialize(inputSource ?? pointer.InputSourceParent, inputAction);
            Pointer = pointer;
            Count = count;
        }
    }
}
