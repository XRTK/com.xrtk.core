// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes a <see cref="MixedRealityPointerEventData"/> with scroll data.
    /// </summary>
    public class MixedRealityPointerScrollEventData : MixedRealityPointerEventData
    {
        /// <summary>
        /// The distance this pointer has been scrolled since the last event was raised.
        /// </summary>
        public Vector2 ScrollDelta { get; private set; }

        /// <inheritdoc />
        public MixedRealityPointerScrollEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="inputAction"></param>
        /// <param name="scrollDelta"></param>
        /// <param name="inputSource"></param>
        public void Initialize(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, Vector2 scrollDelta, IMixedRealityInputSource inputSource = null)
        {
            Initialize(pointer, inputAction, inputSource);
            ScrollDelta = scrollDelta;
        }
    }
}