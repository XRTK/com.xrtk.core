// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace XRTK.Utilities
{
    /// <summary>
    /// Utilities to determine connected device capabilities and state.
    /// </summary>
    public static class XRDeviceUtilities
    {
        private static List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();

        /// <summary>
        /// Gets whether an XR display device is currently connected to the machine.
        /// </summary>
        public static bool IsDevicePresent
        {
            get
            {
                SubsystemManager.GetInstances(xrDisplaySubsystems);

                for (int i = 0; i < xrDisplaySubsystems.Count; i++)
                {
                    var xrDisplay = xrDisplaySubsystems[i];

                    if (xrDisplay.running)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets whether the device has an opaque display.
        /// </summary>
        public static bool IsDisplayOpaque
        {
            get
            {
                SubsystemManager.GetInstances(xrDisplaySubsystems);

                for (int i = 0; i < xrDisplaySubsystems.Count; i++)
                {
                    var xrDisplay = xrDisplaySubsystems[i];

                    if (xrDisplay.running)
                    {
                        return xrDisplay.displayOpaque;
                    }
                }

                // When no device is attached we are assuming the display
                // device is the computer's display, which should be opaque.
                return true;
            }
        }
    }
}
