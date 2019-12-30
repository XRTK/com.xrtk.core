using System;
using UnityEngine.EventSystems;

namespace XRTK.EventDatum.DiagnosticsSystem
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
            if (eventSystem == null)
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