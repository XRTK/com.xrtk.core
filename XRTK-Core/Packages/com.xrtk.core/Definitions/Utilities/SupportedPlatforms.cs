// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// The supported platforms for Mixed Reality Toolkit Components and Features.
    /// </summary>
    [Flags]
    public enum SupportedPlatforms
    {
        /// <summary>
        /// Editor.
        /// </summary>
        Editor = 1 << 0,
        /// <summary>
        /// Windows Standalone platforms.
        /// </summary>
        WindowsStandalone = 1 << 1,
        /// <summary>
        /// Mac OSX standalone platforms.
        /// </summary>
        MacStandalone = 1 << 2,
        /// <summary>
        /// Linux standalone platforms.
        /// </summary>
        LinuxStandalone = 1 << 3,
        /// <summary>
        /// Windows UWP platforms.
        /// </summary>
        WindowsUniversal = 1 << 4,
        /// <summary>
        /// Magic Leap platform.
        /// </summary>
        Lumin = 1 << 5,
        /// <summary>
        /// Android mobile platforms
        /// </summary>
        Android = 1 << 6,
        /// <summary>
        /// Mac iOS mobile platforms.
        /// </summary>
        iOS = 1 << 7
    }
}