// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace XRTK.EventDatum.DiagnosticsSystem
{
    /// <summary>
    /// The event data associated with console events.
    /// </summary>
    public class ConsoleEventData : BaseDiagnosticsEventData
    {
        /// <summary>
        /// The log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The log stack trace.
        /// </summary>
        public string StackTrace { get; private set; }

        /// <summary>
        /// the <see cref="LogType"/>.
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public ConsoleEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        /// <summary>
        /// Initializes the console event data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="logType"></param>
        public void Initialize(string message, string stackTrace, LogType logType)
        {
            BaseInitialize();
            Message = message;
            StackTrace = stackTrace;
            LogType = logType;
        }
    }
}