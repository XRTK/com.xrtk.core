﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for handling mouse pointers.
    /// </summary>
    public interface IMixedRealityMousePointer : IMixedRealityPointer
    {
        /// <summary>
        /// Should the mouse cursor be hidden when no active input is received?
        /// </summary>
        bool HideCursorWhenInactive { get; }

        /// <summary>
        /// What is the movement threshold to reach before un-hiding mouse cursor?
        /// </summary>
        float MovementThresholdToUnHide { get; }

        /// <summary>
        /// How long should it take before the mouse cursor is hidden?
        /// </summary>
        float HideTimeout { get; }

        /// <summary>
        /// Defines the mouse cursor speed.
        /// Multiplier that gets applied to the mouse delta before converting to world space.
        /// </summary>
        float Speed { get; }
    }
}