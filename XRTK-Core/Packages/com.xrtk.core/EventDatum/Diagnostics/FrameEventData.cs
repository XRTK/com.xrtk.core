// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace XRTK.EventDatum.DiagnosticsSystem
{
    /// <summary>
    /// The event data associated with frame events.
    /// </summary>
    public class FrameEventData : BaseDiagnosticsEventData
    {
        /// <summary>
        /// The frames per second.
        /// </summary>
        public int FramesPerSecond { get; private set; }

        /// <summary>
        /// Was this reading for the GPU?
        /// </summary>
        public bool IsGpuReading { get; private set; }

        /// <summary>
        /// The array of missed frames in the sample period.
        /// </summary>
        public bool[] MissedFrames { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public FrameEventData(EventSystem eventSystem)
            : base(eventSystem)
        {
        }

        /// <summary>
        /// Initializes the <see cref="FrameEventData"/>.
        /// </summary>
        /// <param name="framesPerSecond"></param>
        /// <param name="isGpuReading"></param>
        public void Initialize(int framesPerSecond, bool isGpuReading)
        {
            BaseInitialize();
            FramesPerSecond = framesPerSecond;
            IsGpuReading = isGpuReading;
        }

        /// <summary>
        /// Initializes the <see cref="FrameEventData"/>
        /// </summary>
        /// <param name="missedFrames"></param>
        public void Initialize(bool[] missedFrames)
        {
            BaseInitialize();
            MissedFrames = missedFrames;
        }
    }
}