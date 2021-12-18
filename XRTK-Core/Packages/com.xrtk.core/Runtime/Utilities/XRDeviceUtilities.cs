// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Utilities
{
    /// <summary>
    /// Utilities to determine connected device capabilities and state.
    /// </summary>
    public static class XRDeviceUtilities
    {
        /// <summary>
        /// Gets whether an XR display device is currently connected.
        /// </summary>
        public static bool IsDevicePresent => XRSubsystemUtilities.DisplaySubsystem != null;

        /// <summary>
        /// Gets whether the device has an opaque display.
        /// </summary>
        public static bool IsDisplayOpaque
        {
            get
            {
                var displaySubsystem = XRSubsystemUtilities.DisplaySubsystem;
                if (displaySubsystem == null)
                {
                    // When no device is attached we are assuming the display
                    // device is the computer's display, which should be opaque.
                    return true;
                }

                return displaySubsystem.displayOpaque;
            }
        }
    }
}
