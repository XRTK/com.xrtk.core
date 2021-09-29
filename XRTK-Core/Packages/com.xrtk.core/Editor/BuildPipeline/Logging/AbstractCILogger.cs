// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Editor.BuildPipeline.Logging
{
    /// <summary>
    /// Base abstract logger to use when creating custom loggers.
    /// </summary>
    public abstract class AbstractCILogger : ICILogger
    {
        private readonly ILogHandler defaultLogger;

        protected AbstractCILogger()
        {
            defaultLogger = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;
        }

        /// <inheritdoc />
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            var skip = false;

            foreach (var arg in args)
            {
                if (arg is string message)
                {
                    skip = CILoggingUtility.IgnoredLogs.Any(message.Contains);
                }
            }

            if (CILoggingUtility.LoggingEnabled && !skip)
            {
                switch (logType)
                {
                    case LogType.Log:
                        break;
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        format = $"{Error}{format}";
                        break;
                    case LogType.Warning:
                        format = $"{Warning}{format}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
                }
            }

            defaultLogger.LogFormat(logType, context, format, args);
        }

        /// <inheritdoc />
        public void LogException(Exception exception, Object context)
        {
            defaultLogger.LogException(exception, context);
        }

        /// <inheritdoc />
        public virtual string Error => string.Empty;

        /// <inheritdoc />
        public virtual string Warning => string.Empty;
    }
}
