// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// Enum which describes how an object is to be flattened.
    /// </summary>
    public enum FlattenMode
    {
        DoNotFlatten = 0,
        /// <summary>
        /// Flatten the X axis
        /// </summary>
        FlattenX,
        /// <summary>
        /// Flatten the Y axis
        /// </summary>
        FlattenY,
        /// <summary>
        /// Flatten the Z axis
        /// </summary>
        FlattenZ,
        /// <summary>
        /// Flatten the smallest relative axis if it falls below threshold
        /// </summary>
        FlattenAuto,
    }
}