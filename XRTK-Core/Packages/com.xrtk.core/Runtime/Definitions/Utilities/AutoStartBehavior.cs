// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// This enumeration identifies different ways to handle the startup behavior for a feature.
    /// </summary>
    public enum AutoStartBehavior
    {
        /// <summary>
        /// Automatically start the feature.
        /// </summary>
        AutoStart = 0,
        /// <summary>
        /// Delay the start of the feature until the user requests it to begin.
        /// </summary>
        ManualStart
    }
}
