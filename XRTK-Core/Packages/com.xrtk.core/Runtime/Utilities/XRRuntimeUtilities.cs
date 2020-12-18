// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.XR;

#if UNITY_2020_1_OR_NEWER
using UnityEngine;
using System.Collections.Generic;
#endif

namespace XRTK.Utilities
{
    /// <summary>
    /// Helper utility to read the currenlty in use XR SDK management pipeline
    /// and device state.
    /// </summary>
    public static class XRRuntimeUtilities
    {
        public static bool XRDevicePresent
        {
            get
            {
#if UNITY_2020_1_OR_NEWER
                var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
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
#else
                return XRDevice.isPresent;
#endif
            }
        }
    }
}