// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// Enum which describes cardinal axes
    /// </summary>
    [Flags]
    public enum CardinalAxis
    {
        // Note: Because of the way Unity handles flags
        // We didn't add a default enum definition nor everything.
        // Everything = -1
        // None = 0,
        X = 1,
        Y = 2,
        Z = 4,
    }
}