// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event that has a source id.
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        /// <summary>
        /// Handedness of the <see cref="IMixedRealityInputSource"/>.
        /// </summary>
        public Handedness Handedness { get; private set; } = Handedness.None;

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputAction inputAction)
        {
            BaseInitialize(inputSource, inputAction);
            Handedness = handedness;
        }
    }

    /// <summary>
    /// Describes and input event with a specific type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputEventData<T> : InputEventData where T : struct
    {
        /// <summary>
        /// The input data of the event.
        /// </summary>
        public T InputData => InputAction.ReadValue<T>();

        /// <inheritdoc />
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }
    }
}
