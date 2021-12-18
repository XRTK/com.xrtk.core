// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace XRTK.Utilities
{
    /// <summary>
    /// Utilities around Unity's XR SDK subsystem implementation for easy
    /// and cached access to important subsystems.
    /// </summary>
    public class XRSubsystemUtilities
    {
        private static XRInputSubsystem inputSubsystem = null;
        private static readonly List<XRInputSubsystem> xrInputSubsystems = new List<XRInputSubsystem>();
        private static XRMeshSubsystem meshSubsystem = null;
        private static readonly List<XRMeshSubsystem> xrMeshSubsystems = new List<XRMeshSubsystem>();
        private static XRDisplaySubsystem displaySubsystem = null;
        private static readonly List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();

        /// <summary>
        /// Gets the active <see cref="XRInputSubsystem"/> for the currently loaded
        /// XR plugin. The reference is lazy loaded once on first access and then cached
        /// for future use.
        /// </summary>
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
                if (inputSubsystem != null && inputSubsystem.running)
                {
                    return inputSubsystem;
                }

                inputSubsystem = null;
                SubsystemManager.GetInstances(xrInputSubsystems);

                for (var i = 0; i < xrInputSubsystems.Count; i++)
                {
                    var xrInputSubsystem = xrInputSubsystems[i];
                    if (xrInputSubsystem.running)
                    {
                        inputSubsystem = xrInputSubsystem;
                        break;
                    }
                }

                return inputSubsystem;
            }
        }

        /// <summary>
        /// Gets the active <see cref="XRMeshSubsystem"/> for the currently loaded
        /// XR plugin. The reference is lazy loaded once on first access and then cached
        /// for future use.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem
        {
            get
            {
                if (meshSubsystem != null && meshSubsystem.running)
                {
                    return meshSubsystem;
                }

                meshSubsystem = null;
                SubsystemManager.GetInstances(xrMeshSubsystems);

                for (var i = 0; i < xrMeshSubsystems.Count; i++)
                {
                    var xrMeshSubsystem = xrMeshSubsystems[i];
                    if (xrMeshSubsystem.running)
                    {
                        meshSubsystem = xrMeshSubsystem;
                        break;
                    }
                }

                return meshSubsystem;
            }
        }

        /// <summary>
        /// Gets the active <see cref="XRDisplaySubsystem"/> for the currently loaded
        /// XR plugin. The reference is lazy loaded once on first access and then cached
        /// for future use.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem != null && displaySubsystem.running)
                {
                    return displaySubsystem;
                }

                displaySubsystem = null;
                SubsystemManager.GetInstances(xrDisplaySubsystems);

                for (var i = 0; i < xrDisplaySubsystems.Count; i++)
                {
                    var xrDisplaySubsystem = xrDisplaySubsystems[i];
                    if (xrDisplaySubsystem.running)
                    {
                        displaySubsystem = xrDisplaySubsystem;
                        break;
                    }
                }

                return displaySubsystem;
            }
        }
    }
}
