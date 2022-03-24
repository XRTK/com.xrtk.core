﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;
using XRTK.Definitions.InputSystem;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;

namespace XRTK.EventDatum.Input
{
    /// <summary>
    /// Base class of all Input Events.
    /// </summary>
    public abstract class BaseInputEventData : BaseEventData
    {
        /// <summary>
        /// The time at which the event occurred.
        /// </summary>
        /// <remarks>
        /// The value will be in the device's configured time zone.
        /// </remarks>
        public DateTime EventTime { get; private set; }

        /// <summary>
        /// The source the input event originates from.
        /// </summary>
        public IMixedRealityInputSource InputSource { get; private set; }

        /// <summary>
        /// The id of the source the event is from, for instance the hand id.
        /// </summary>
        public uint SourceId { get; private set; }

        /// <summary>
        /// The Input Action for this event.
        /// </summary>
        public MixedRealityInputAction MixedRealityInputAction { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem">Typically will be <see cref="EventSystem.current"/></param>
        protected BaseInputEventData(EventSystem eventSystem) : base(eventSystem)
        {
            if (eventSystem.IsNull())
            {
                throw new Exception($"{nameof(EventSystem)} cannot be null!");
            }
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        protected void BaseInitialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction)
        {
            Reset();
            EventTime = DateTime.UtcNow;
            InputSource = inputSource;
            MixedRealityInputAction = inputAction;
            SourceId = InputSource.SourceId;
        }
    }
}
