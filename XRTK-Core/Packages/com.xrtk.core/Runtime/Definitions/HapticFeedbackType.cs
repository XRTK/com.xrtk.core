// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions
{
    /// <summary>
    /// Pre-defined feedback types that are platform agnostic, so long as they're implemented by the platform.
    /// </summary>
    public enum HapticFeedbackType
    {
        /// <summary>
        /// 
        /// </summary>
        Continuous = 0,
        /// <summary>
        /// 
        /// </summary>
        Click,
        /// <summary>
        /// 
        /// </summary>
        Bump,
        /// <summary>
        /// 
        /// </summary>
        DoubleClick,
        /// <summary>
        /// 
        /// </summary>
        Buzz,
        /// <summary>
        /// 
        /// </summary>
        Tick,
        /// <summary>
        /// 
        /// </summary>
        ForceDown,
        /// <summary>
        /// 
        /// </summary>
        ForceUp,
        /// <summary>
        /// 
        /// </summary>
        ForceDwell,
        /// <summary>
        /// 
        /// </summary>
        SecondForceDown,
    }
}
