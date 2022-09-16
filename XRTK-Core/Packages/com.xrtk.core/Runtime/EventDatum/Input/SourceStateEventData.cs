// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Describes an source state event that has a source id.
    /// </summary>
    /// <remarks>Source State events do not have an associated <see cref="UnityEngine.InputSystem.InputActionReference"/>.</remarks>
    public class SourceStateEventData : BaseInputEventData
    {
        public IMixedRealityController Controller { get; private set; }

        /// <inheritdoc />
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="controller"></param>
        public void Initialize(IMixedRealityInputSource inputSource, IMixedRealityController controller)
        {
            // NOTE: Source State events do not have an associated Input Action.
            BaseInitialize(inputSource, default);
            Controller = controller;
        }
    }
}
