// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace XRTK.EventDatum.DiagnosticsSystem
{
    /// <summary>
    /// The event data associated with missed frame events.
    /// </summary>
    public class MissedFrameEventData : BaseDiagnosticsEventData
    {
        public bool[] MissedFrames { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MissedFrameEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        /// <summary>
        /// Initializes the <see cref="MissedFrameEventData"/>
        /// </summary>
        /// <param name="missedFrames"></param>
        public void Initialize(bool[] missedFrames)
        {
            BaseInitialize();
            MissedFrames = missedFrames;
        }
    }
}