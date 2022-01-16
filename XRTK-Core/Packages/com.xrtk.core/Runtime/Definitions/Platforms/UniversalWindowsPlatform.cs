// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the Windows Universal Platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("7DC72B4E-34F6-4B26-AFD7-CDE0C51F83A3")]
    public class UniversalWindowsPlatform : BasePlatform
    {
        private const string windowsXRDisplaySubsystemDescriptorId = "Windows Mixed Reality Display";
        private const string windowsXRInputSubsystemDescriptorId = "Windows Mixed Reality Input";

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
                var displaySubsystems = new List<XRDisplaySubsystem>();
                SubsystemManager.GetSubsystems(displaySubsystems);
                var windowsXRDisplaySubsystemDescriptorFound = false;

                for (var i = 0; i < displaySubsystems.Count; i++)
                {
                    var displaySubsystem = displaySubsystems[i];
                    if (displaySubsystem.SubsystemDescriptor.id.Equals(windowsXRDisplaySubsystemDescriptorId) &&
                        displaySubsystem.running)
                    {
                        windowsXRDisplaySubsystemDescriptorFound = true;
                    }
                }

                // The Windows XR Display Subsystem is not available / running,
                // Windows as a platform doesn't seem to be available.
                if (!windowsXRDisplaySubsystemDescriptorFound)
                {
                    return false;
                }

                var inputSubsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetSubsystems(inputSubsystems);
                var windowsXRInputSubsystemDescriptorFound = false;

                for (var i = 0; i < inputSubsystems.Count; i++)
                {
                    var inputSubsystem = inputSubsystems[i];
                    if (inputSubsystem.SubsystemDescriptor.id.Equals(windowsXRInputSubsystemDescriptorId) &&
                        inputSubsystem.running)
                    {
                        windowsXRInputSubsystemDescriptorFound = true;
                    }
                }

                // The Windows XR Input Subsystem is not available / running,
                // Windows XR as a platform doesn't seem to be available.
                if (!windowsXRInputSubsystemDescriptorFound)
                {
                    return false;
                }

                // Only if both, Display and Input Windows XR Subsystems are available
                // and running, the platform is considered available.
                return true;
            }
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } =
        {
            UnityEditor.BuildTarget.WSAPlayer
        };
#endif // UNITY_EDITOR
    }
}
