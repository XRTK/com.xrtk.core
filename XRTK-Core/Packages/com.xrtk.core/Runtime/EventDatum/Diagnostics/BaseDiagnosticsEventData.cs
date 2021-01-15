// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;
using XRTK.Extensions;

namespace XRTK.EventDatum.Diagnostics
{
    /// <summary>
    /// The base event class to inherit from for diagnostics events.
    /// </summary>
    public abstract class BaseDiagnosticsEventData : BaseEventData
    {
        /// <summary>
        /// The time at which the event occurred.
        /// </summary>
        /// <remarks>
        /// The value will be in the device's configured time zone.
        /// </remarks>
        public DateTime EventTime { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        protected BaseDiagnosticsEventData(EventSystem eventSystem) : base(eventSystem)
        {
            if (eventSystem.IsNull())
            {
                throw new Exception($"{nameof(EventSystem)} cannot be null!");
            }
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the event data.
        /// </summary>
        protected void BaseInitialize()
        {
            Reset();
            EventTime = DateTime.UtcNow;
        }
    }
}