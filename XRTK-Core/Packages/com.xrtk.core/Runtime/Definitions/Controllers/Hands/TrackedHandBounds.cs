// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Controllers.Hands
{
    /// <summary>
    /// Available hand bounds types.
    /// </summary>
    public enum TrackedHandBounds
    {
        None = 0,
        /// <summary>
        /// The thumb bounds.
        /// </summary>
        Thumb,
        /// <summary>
        /// The index finger bounds.
        /// </summary>
        IndexFinger,
        /// <summary>
        /// The middle finger bounds.
        /// </summary>
        MiddleFinger,
        /// <summary>
        /// The ring finger bounds.
        /// </summary>
        RingFinger,
        /// <summary>
        /// The pinky bounds.
        /// </summary>
        Pinky,
        /// <summary>
        /// The palm bounds.
        /// </summary>
        Palm,
        /// <summary>
        /// The full hand bounds.
        /// </summary>
        Hand
    }
}