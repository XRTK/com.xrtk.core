// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// This enum is used to customize how/when users will look around in the Unity player
    /// using the mouse.
    /// </summary>
    public enum EditorCameraControlMouseButton
    {
        Left,       // Left mouse button
        Right,      // Right mouse button
        Middle,     // Middle or scroll wheel button
        Control,    // Control on keyboard
        Shift,      // Shift on keyboard
        Focused,    // When Unity player has focus
        None        // No mouse look functionality
    }
}